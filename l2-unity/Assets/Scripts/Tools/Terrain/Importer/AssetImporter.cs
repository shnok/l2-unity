#if (UNITY_EDITOR) 
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public abstract class AssetImporter {
    protected static void ImportFiles(string inputFolder, List<string> files, bool overwrite) {
        foreach(var filePath in files) {
            string path = filePath;
            if(!File.Exists(path)) {
                string pathRectified = path.Replace(inputFolder, String.Empty);
                string[] split = pathRectified.Split("\\");
                path = Path.Combine(inputFolder, split[1], "Texture", split[2]);

                if(!File.Exists(path)) {
                    Debug.LogError("File " + path + " doesn't exist.");
                    continue;
                }
            }

            string relativePath = Path.GetRelativePath(inputFolder, path.Replace("\\Texture", string.Empty));
            string destination = null;
            if(Path.GetExtension(path).ToLower() == ".fbx") {
                destination = Path.Combine("Assets", "Resources", "Data", "StaticMeshes", relativePath);
                Debug.Log(destination);
            } else if(Path.GetExtension(path).ToLower() == ".png") {
                destination = Path.Combine("Assets", "Resources", "Data", "Textures", relativePath);
                Debug.Log(destination);
            } else if(Path.GetExtension(path).ToLower() == ".txt") {
                destination = Path.Combine("Assets", "Resources", "Data", "Textures", relativePath);
                Debug.Log(destination);
            }

            if(destination != null) {
                if(!Directory.Exists(Path.GetDirectoryName(destination))) {
                    Directory.CreateDirectory(Path.GetDirectoryName(destination));
                }

                try {
                    File.Copy(path, destination, overwrite);
                } catch(IOException e) {
                    Debug.LogWarning(destination + " already exists or " + e.Message);
                }

            }
        }
    }

    protected static string FindInSubDirectories(string baseFolder, string fileName) {
        try {
            string[] files = Directory.GetFiles(baseFolder, fileName, SearchOption.AllDirectories);

            if(files.Length > 0) {
                foreach(string file in files) {
                    return file;
                }
            } else {
                Debug.Log("File not found in the specified directory.");
            }
        } catch(DirectoryNotFoundException e) {
            Debug.Log("Directory not found: " + e.Message);
        } catch(Exception ex) {
            Debug.LogError("An error occurred: " + ex.Message);
        }

        return null;
    }

    protected static string GetParentFolder(string path) {
        string currentDirectory = path;
        if(Path.GetExtension(path) != string.Empty) {
            currentDirectory = Path.GetDirectoryName(path);
        }

        string parentFolderFullPath = Directory.GetParent(currentDirectory).FullName;
        string parentFolderRelativePath = Path.Combine("Assets", Path.GetRelativePath(Application.dataPath, parentFolderFullPath));
        return parentFolderRelativePath;
    }

    protected static string GetFolderName(string path) {
        string folderName = Path.GetFileName(path);
        return folderName;
    }
}
#endif
