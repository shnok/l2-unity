using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace rTexImporter
{
    public class rTexPackerImporterMenu
    {
        [MenuItem("Assets/rTexPacker/Create sprite sheets")]
        public static void ImportSprites()
        {
            string texturePath = AssetDatabase.GUIDToAssetPath(Selection.assetGUIDs[0]);
            rTexPackerImporter.CreateSpriteSheets(texturePath, (Texture2D)Selection.activeObject);
        }

        [MenuItem("Assets/rTexPacker/Create sprite sheets", true)]
        private static bool ValidateType()
        {
            return Selection.activeObject.GetType() == typeof(Texture2D);
        }
    }
}

