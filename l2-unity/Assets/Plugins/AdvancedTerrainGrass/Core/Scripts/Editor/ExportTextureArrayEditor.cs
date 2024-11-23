using UnityEngine;
using UnityEditor;
using System.IO;

public class ExportTextureArrayEditor {
    [MenuItem("Assets/Create Texture2DArray from Selection")]
    static void ExportResource() {
        // Selection is unsorted and does not reflect the sort of the displayed filenames of unity
        //Object[] selection = Selection.GetFiltered(typeof(Object), SelectionMode.DeepAssets);
        Object[] selection = Selection.GetFiltered(typeof(Object), SelectionMode.Unfiltered);

        if (selection.Length > 1) {

            // https://forum.unity3d.com/threads/selection-getfiltered-sorting.82295/
            string[] relativeFullSelectionPaths = new string[selection.Length];
            for (int i = 0; i < selection.Length; i++) {
                relativeFullSelectionPaths[i] = AssetDatabase.GetAssetPath(selection[i]);
            }
            // Sort selection array by path keys
            System.Array.Sort(relativeFullSelectionPaths, selection);
            string pathfull = new FileInfo(AssetDatabase.GetAssetPath(selection[0])).FullName;
            
            if (Directory.Exists(pathfull)) {
                EditorUtility.DisplayDialog(
                    "Creating a Texture2DArray has failed.", 
                    "You have to select at least 2 texture files.",
                    "Close");
                return;
            }

            // FileInfo reversed IBM format back to Unity
            pathfull = pathfull.Replace("\\", "/");

            string name = selection[0].name.ToLower();
            var pathfull_l = pathfull.ToLower();
            pathfull = pathfull.Substring(0, pathfull_l.LastIndexOf("/" + name));

            Texture2D[] textures = new Texture2D[selection.Length];

            for (int i = 0; i < textures.Length; i++) {
                textures[i] = (Texture2D)selection[i];
            }

            // Check for fail on different properties
            string error = "";
            for (int i = 1; i < textures.Length; i++) {
                error += (textures[0].width != textures[i].width) ?             textures[i].name + " has a divergent width of: " + textures[i].width +"\n" : "";
                error += (textures[0].height != textures[i].height) ?           textures[i].name + " has a divergent height of: " + textures[i].height +"\n" : "";
                error += (textures[0].format != textures[i].format) ?           textures[i].name + " uses a divergent format: " + textures[i].format + "\n" : "";
                error += (textures[0].anisoLevel != textures[i].anisoLevel) ?   textures[i].name + " uses a divergent aniso level.\n" : "";
                error += (textures[0].dimension != textures[i].dimension) ?     textures[i].name + " has a divergent dimension.\n" : "";
                //error += (textures[0].mipmapCount != textures[i].mipmapCount) ? textures[i].name + " has a divergent mipmap count.\n" : "";
                //error += (textures[0].wrapMode != textures[i].wrapMode) ?       textures[i].name + " uses a divergent wrap mode.\n" : "";
            }
            if (error.Length > 0) {
                EditorUtility.DisplayDialog("Creating a Texture2DArray has failed.", 
                    "The selected textures do not fit.\n\n" +
                    "Base texture: "+ textures[0].name + ", " + textures[0].width + "x" + textures[0].height + "px, " + textures[0].format+ "\n\n" + error, "Close");
                return;
            }

        //  Save textureArray
            pathfull = EditorUtility.SaveFilePanel("Save Texture2DArray", pathfull, "Texture2DArray", "asset");

        //  Create relative Path
            pathfull_l = pathfull.ToLower();
            pathfull = pathfull.Substring(pathfull_l.IndexOf("/assets/") + 1);
            if (pathfull.Length != 0) {

            //  We use bc7 only for normals and combined maps. So we use this to determine color space.
                bool isLinear = false; 
                if (textures[0].format == TextureFormat.BC7 ) {
                    isLinear = true;
                }
            
                Texture2DArray targetTextureArray = new Texture2DArray(textures[0].width, textures[0].height, textures.Length, textures[0].format, true, isLinear);
                int width = textures[0].width;
                int height = textures[0].height;
                TextureFormat texFormat = textures[0].format;

                int mipmapcount = textures[0].mipmapCount;
                int arrayDepth = textures.Length;
                
                Texture2D tex0;
                for (int i = 0; i < mipmapcount; i++) {   // all mip levels
                    int layer = 0;
                    for (int s = 0; s < arrayDepth; s++) {
                        tex0 = textures[s];
                        Graphics.CopyTexture(
                            tex0,                         // source tex
                            0,                            // source element (layer) ????
                            i,                            // source mip
                            targetTextureArray,           // destination tex
                            layer,                        // destination element (layer)
                            i                             // desination mip
                        );
                        layer++;
                    }
                }
                targetTextureArray.Apply(false);          // Do nor rebuilt mip levels as it is not supported

            //  Linar color space most likely means that we deal with normal maps.
                if (isLinear) {
                    targetTextureArray.filterMode = FilterMode.Trilinear;      
                }
                     
            //  Simply overwriting an existing array will break refereeces. So we have to check first.
                Texture2DArray tempArray = (Texture2DArray)AssetDatabase.LoadAssetAtPath(pathfull, typeof(Texture2DArray));
            //  A) Update existing asset.
                if (tempArray != null) {
                    EditorUtility.CopySerialized(targetTextureArray, tempArray);
                    AssetDatabase.SaveAssets();
                }
            //  B) Create a new one.
                else {
                    AssetDatabase.CreateAsset(targetTextureArray, pathfull);
                    AssetDatabase.ImportAsset(pathfull, ImportAssetOptions.ForceUpdate);
                }
            }
        } 
        else {
            EditorUtility.DisplayDialog("Creating a Texture2DArray has failed.", 
                "You have to select at least 2 texture files.",
                "Close");
        }
    }
}