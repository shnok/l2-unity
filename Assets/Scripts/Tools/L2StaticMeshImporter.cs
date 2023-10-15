using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

public class L2StaticMeshImporter
{
    [MenuItem("Shnok/Import StaticMeshes")]
    static void OpenFileSelectionDialog() {
        bool overwrite = false;
        string title = "Select StaticMeshes list";
        string directory = Path.Combine(Application.dataPath, "Data/Maps"); 
        string extension = "txt"; 

        string inputFolder = "G:/Stock/Projects/L2-Unity/Tools/umodel_win32/export";

        string filePath = EditorUtility.OpenFilePanel(title, directory, extension);

        List<string> files = new List<string>();

        if(!string.IsNullOrEmpty(filePath)) {
            Debug.Log("Selected file: " + filePath);
            using(StreamReader reader = new StreamReader(filePath)) {
                string line;
                while((line = reader.ReadLine()) != null) {
                    Debug.Log("Processing:" + line);

                    string[] parts = line.Split('.');
                    string folder = parts[0];
                    string file = parts.Length > 2 ? parts[2] : parts[1];

                    string staticMeshPath = Path.Combine(inputFolder, folder, file + ".fbx");
                    if(!File.Exists(staticMeshPath)) {
                        Debug.LogWarning("Mesh missing:" + staticMeshPath);
                    } else {
                        string textureInfoPath = Path.Combine(inputFolder, folder, "StaticMesh", file + ".props.txt");
                        if(!File.Exists(textureInfoPath)) {
                            Debug.LogWarning("Texture info missing:" + textureInfoPath);
                        } else {
                            files.Add(staticMeshPath);
                            files.AddRange(ParseTextureInfo(textureInfoPath));
                        }
                    }
                }
            }
        }

        files = files.Distinct().ToList();
        Debug.LogWarning("Total files to import: " + files.Count);

        ImportFiles(inputFolder, files, overwrite);
    }

    static List<string> ParseTextureInfo(string path) {
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

        string baseFolder = GetParentFolder(meshFolder);

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
                        Debug.LogError("Could find not texture at " + texturePath);
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
                    Debug.LogError("Could find not props for " + name);
                    continue;
                }
            }

            filesToExport.Add(materialInfoProps);

            using(StreamReader reader = new StreamReader(materialInfoProps)) {
                string line;
                while((line = reader.ReadLine()) != null) {
                    if(line.StartsWith("Diffuse") || line.StartsWith("Material")) {
                        string value = line.Split("=")[1].Trim();
                        if(value.StartsWith("Texture")) {
                            string texRef = value.Substring(8);
                            texRef = texRef.Substring(0, texRef.Length - 1);
                            string[] texRefEntries = texRef.Split('.');
                            string textureToImport = texRefEntries[texRefEntries.Length - 1];

                            string texturePath = Path.Combine(GetParentFolder(materialInfoProps), textureToImport + ".png");
                            if(File.Exists(texturePath)) {
                                filesToExport.Add(texturePath);
                            } else {
                                Debug.LogError("Could not find texture at " + texturePath);
                            }
                        }
                    }
                }
            }
        }

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

    static string GetParentFolder(string path) {
        string currentDirectory = path;
        if(Path.GetExtension(path) != string.Empty) {
            currentDirectory = Path.GetDirectoryName(path);
        }
        
        string parentFolderFullPath = Directory.GetParent(currentDirectory).FullName;
        string parentFolderRelativePath = Path.Combine("Assets", Path.GetRelativePath(Application.dataPath, parentFolderFullPath));
        return parentFolderRelativePath;
    }

    static string GetFolderName(string path) {
        string folderName = Path.GetFileName(path);
        return folderName;
    }

    static string FixPath(string baseFolder, string fileName, bool shader) {
        string pathToTest = string.Empty;
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
            string searchedFile = FindInSubDirectories(baseFolder, fileName);
            Debug.LogWarning("Searched and found " + searchedFile);
            pathToTest = searchedFile;
        }

        return pathToTest;
    }

    static string FindInSubDirectories(string baseFolder, string fileName) {
        try {
            string[] files = Directory.GetFiles(baseFolder, fileName, SearchOption.AllDirectories);

            if(files.Length > 0) {
                Debug.Log("File(s) found:");
                foreach(string file in files) {
                    return file;
                }
            } else {
                Debug.Log("File not found in the specified directory.");
            }
        } catch(DirectoryNotFoundException) {
            Debug.Log("Directory not found.");
        } catch(Exception ex) {
            Debug.LogError("An error occurred: " + ex.Message);
        }

        return null;
    }

    static void ImportFiles(string inputFolder, List<string> files, bool overwrite) {
        foreach(var file in files) {
            if(!File.Exists(file)) {
                Debug.LogError("File " + file + " doesn't exist.");
                continue;
            }

            string relativePath = Path.GetRelativePath(inputFolder, file);

            string dest = null;
            if(Path.GetExtension(file).ToLower() == ".fbx") {
                dest = Path.Combine("Assets", "Data", "StaticMeshes", relativePath);
                Debug.Log(dest);
            } else if(Path.GetExtension(file).ToLower() == ".png") {
                dest = Path.Combine("Assets", "Data", "Textures", relativePath);
                Debug.Log(dest);
            } else if(Path.GetExtension(file).ToLower() == ".txt") {
                dest = Path.Combine("Assets", "Data", "Textures", relativePath);
                Debug.Log(dest);
            }

            if(dest != null) {

                if(!Directory.Exists(Path.GetDirectoryName(dest))) {
                    Directory.CreateDirectory(Path.GetDirectoryName(dest));
                }

                try {
                    File.Copy(file, dest, overwrite);
                } catch(IOException e) {
                    Debug.LogWarning(dest + " already exists.");
                }
                
            }
        }
    }
}

