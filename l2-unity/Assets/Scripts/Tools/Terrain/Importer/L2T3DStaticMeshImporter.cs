#if (UNITY_EDITOR) 
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

public class L2T3DStaticMeshImporter : AssetImporter {

    private static int missingTexturesCount = 0;

    [MenuItem("Shnok/1. [StaticMeshes] Import Textures and models")]
    static void ImportStaticMeshes() {
        string title = "Select StaticMeshes list";
        string directory = Path.Combine(Application.dataPath, "Data/Maps");
        string extension = "txt,t3d";

        string dataFolder = @"D:\Stock\Projects\L2-Unity\umodel_win32\export";
        bool overwrite = false;

        string fileToProcess = EditorUtility.OpenFilePanel(title, directory, extension);

        if(!string.IsNullOrEmpty(fileToProcess)) {
            Debug.Log("Selected file: " + fileToProcess);
            L2TerrainInfo terrainInfo = L2T3DInfoParser.LoadStaticMeshInfo(fileToProcess);
            List<string> files = ProcessStaticMeshInfo(dataFolder, terrainInfo.staticMeshes);
            //List<string> files = ProcessDataFile(dataFolder, fileToProcess);
            ImportFiles(dataFolder, files, overwrite);
        }
    }

    private static List<string> ProcessStaticMeshInfo(string dataFolder, List<L2StaticMesh> staticMeshes) {
        List<string> files = new List<string>();
        int staticMeshCount = 0;
        int textureInfoCount = 0;
        int missingMeshCount = 0;
        int missingTextureInfoCount = 0;

        foreach (L2StaticMesh mesh in staticMeshes) {
            if (mesh.staticMesh == null || mesh.staticMesh.Length == 0) continue;
            string[] parts = mesh.staticMesh.Split('.');
            string folder = parts[0];
            string file = parts[parts.Length - 1];

            string staticMeshPath = Path.Combine(dataFolder, folder, file + ".fbx");

            if (!File.Exists(staticMeshPath)) {
                missingMeshCount++;
                Debug.LogWarning("Mesh missing:" + staticMeshPath);
            } else {
                staticMeshCount++;
                files.Add(staticMeshPath);
                string textureInfoPath = Path.Combine(dataFolder, folder, "StaticMesh", file + ".props.txt");
                if (!File.Exists(textureInfoPath)) {
                    missingTextureInfoCount++;
                    Debug.LogWarning("Texture info missing:" + textureInfoPath);
                } else {
                    List<string> textures = ParseTextureInfo(dataFolder, textureInfoPath);
                    textureInfoCount+= textures.Count;
                    files.AddRange(textures);
                }
            }

       
        }

        files = files.Distinct().ToList();

        //files.ForEach((f) => Debug.Log(f)); 

        Debug.Log($"{staticMeshCount} staticmesh(es) FBX will be imported.");
        Debug.Log($"{missingMeshCount} missing staticmesh(es) FBX.");
        Debug.Log($"{textureInfoCount} texture(s) will be imported.");
        Debug.Log($"{missingTextureInfoCount} missing staticmesh(es) textureinfo.");

        Debug.LogWarning($"{files.Count} distinct files(s) will be imported.");

        return files;
    }

    //private static List<string> ProcessDataFile(string dataFolder, string fileToProcess) {
    //    List<string> files = new List<string>();

    //    using (StreamReader reader = new StreamReader(fileToProcess)) {
    //        string line;
    //        while ((line = reader.ReadLine()) != null) {
    //            Debug.Log("Processing:" + line);

    //            string[] parts = line.Split('.');
    //            string folder = parts[0];
    //            string file = parts[parts.Length - 1];

    //            string staticMeshPath = Path.Combine(dataFolder, folder, file + ".fbx");

    //            if (!File.Exists(staticMeshPath)) {
    //                Debug.LogWarning("Mesh missing:" + staticMeshPath);
    //            } else {
    //                files.Add(staticMeshPath);
    //                string textureInfoPath = Path.Combine(dataFolder, folder, "StaticMesh", file + ".props.txt");
    //                if (!File.Exists(textureInfoPath)) {
    //                    Debug.LogWarning("Texture info missing:" + textureInfoPath);
    //                } else {
    //                    files.AddRange(ParseTextureInfo(textureInfoPath));
    //                }
    //            }
    //        }
    //    }

    //    files = files.Distinct().ToList();
    //    Debug.LogWarning("Total files to import: " + files.Count);

    //    return files;
    //}

    static List<string> ParseTextureInfo(string baseFolder, string path) {
        List<string> filesToExport = new List<string>();

        string inputText = File.ReadAllText(path);
        string meshFolder = GetParentFolder(path);
        string textureFolderName = GetFolderName(meshFolder);
        if(textureFolderName.ToLower().EndsWith("_s")) {
            textureFolderName = textureFolderName.Substring(0, textureFolderName.Length - 2) + "_t";
        }
        if(textureFolderName.ToLower().EndsWith("_us")) {
            textureFolderName = textureFolderName.Substring(0, textureFolderName.Length - 3) + "_tx";
        }

        string textureFolder = Path.Combine(baseFolder, textureFolderName);
        List<string> textures = GetTextureNames(inputText, @"Texture'([^']+)");
        foreach(var texture in textures) {
            //Debug.Log("Texture:       " + texture);
            string[] parts = texture.Split('.');
            string name = parts[parts.Length - 1];

            string texturePath = Path.Combine(meshFolder, name + ".png");
            if(!File.Exists(texturePath)) {
                texturePath = Path.Combine(textureFolder, name + ".png");
                if(!File.Exists(texturePath)) {
                    texturePath = FixPath(baseFolder, name + ".png", false);
                    if(!File.Exists(texturePath)) {
                        missingTexturesCount++;
                        Debug.LogWarning("Could find not texture at " + texturePath);
                        continue;
                    }
                }
            }

            filesToExport.Add(texturePath);

        }

        string materialInfoFolder = Path.Combine(textureFolder, "Materials");
        List<string> shaders = GetTextureNames(inputText, @"Shader'([^']+)");
        foreach(var shader in shaders) {
            //Debug.Log("Shader:       " + shader);
            string[] parts = shader.Split('.');
            string name = parts[parts.Length - 1];

            string materialInfoProps = Path.Combine(materialInfoFolder, name + ".props.txt");
            if(!File.Exists(materialInfoProps)) {
                materialInfoProps = FixPath(baseFolder, name + ".props.txt", true);
                if(!File.Exists(materialInfoProps)) {
                    continue;
                }
            }

            filesToExport.Add(materialInfoProps);

            using(StreamReader reader = new StreamReader(materialInfoProps)) {
                string line;
                while((line = reader.ReadLine()) != null) {
                    if(line.StartsWith("Diffuse") || line.StartsWith("Material")) {
                        string value = line.Split("=")[1].Trim();
                        if (value.StartsWith("Texture")) {
                            string texRef = value.Substring(8);
                            texRef = texRef.Substring(0, texRef.Length - 1);
                            string[] texRefEntries = texRef.Split('.');
                            string textureToImport = texRefEntries[texRefEntries.Length - 1];

                            string texturePath = Path.Combine(GetParentFolder(materialInfoProps), textureToImport + ".png");
                            if (File.Exists(texturePath)) {
                                filesToExport.Add(texturePath);
                            } else {
                                Debug.LogError("Could not find texture at " + texturePath + " props file: " + materialInfoProps);
                            }
                        }
                    }
                }
            }
        }

        Debug.Log($"Found {filesToExport.Count} texture(s) for props file {path}.");

        return filesToExport;
    }

    static List<string> GetTextureNames(string inputText, string pattern) {

        List<string> items = new List<string>();

        Regex regex = new Regex(pattern);

        MatchCollection matches = regex.Matches(inputText);

        foreach(Match match in matches) {
            if(!items.Contains(match.Value)) {
                items.Add(match.Value
                    .Replace("Texture'", string.Empty)
                    .Replace("Shader'", string.Empty));
            }
        }

        return items;
    }

    static string FixPath(string baseFolder, string fileName, bool shader) {
        string pathToTest = string.Empty;
        string origFileName = fileName;
        if(shader) {
            fileName = "Materials/" + fileName;
        }

        if(fileName.ToLower().StartsWith("interior_")) {
            string[] interiorParts = fileName.Split('_');
            string folder = interiorParts[0] + "_" + interiorParts[1] + (interiorParts[2].StartsWith("ch") ? "CH_T" : "_T");
            pathToTest = Path.Combine(baseFolder, folder, fileName);
        }

        if(!File.Exists(pathToTest)) {
            pathToTest = Path.Combine(baseFolder, "FX_E_T", fileName);
        }
        if(!File.Exists(pathToTest)) {
            pathToTest = Path.Combine(baseFolder, "SI_V_T", fileName);
        }
        if(!File.Exists(pathToTest)) {
            pathToTest = Path.Combine(baseFolder, "speakingfighter_t", fileName);
        }
        if(!File.Exists(pathToTest)) {
            pathToTest = Path.Combine(baseFolder, "Gludio_Port_T", fileName);
        }
        if(!File.Exists(pathToTest)) {
            string searchedFile = FindInSubDirectories(baseFolder, origFileName);
            //Debug.LogWarning("Searched and found " + searchedFile);

            if (searchedFile == null && shader) {
                Debug.LogWarning("Could not find material props at path " + pathToTest);
            }

            pathToTest = searchedFile;
        }

        return pathToTest;
    }
}
#endif