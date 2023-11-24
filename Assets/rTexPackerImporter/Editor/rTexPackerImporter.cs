using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Xml.Serialization;
using UnityEditor;
using UnityEditor.U2D.Sprites;

namespace rTexImporter
{
    public class rTexPackerImporter
    {
        public enum DataType { rtpa, json, xml };
        public static DataType dataType;

        public static Texture2D texture2D;
        public static string filePath;
        public static AtlasData atlasData;

        public static void CreateSpriteSheets(string path, Texture2D texture)
        {
            texture2D = texture;
            Debug.Log("Create sprite sheets...");
            filePath = path.Split('.')[0];
            GetDataType();
            switch (dataType)
            {
                case DataType.rtpa: atlasData = ReadDataFromRTPA(); break;
                case DataType.json: atlasData = ReadDataFromJSON(); break;
                case DataType.xml: atlasData = ReadDataFromXML(); break;
                default:
                    break;
            }

            if (atlasData == null)
            {
                Debug.LogError("Atlas data is null");
                return;
            }

            if (!atlasData.atlas.isFont) ImportSpriteSheet(atlasData.sprites); 
            else
            {
                //TODO: generate fontdata
            }

            atlasData = null;
            texture2D = null;
            filePath = "";

            Debug.Log("Sprite sheets created success!");
        }

        private static void GetDataType()
        {
            if (File.Exists(filePath + ".rtpa"))
            {
                dataType = DataType.rtpa;
            }
            else if (File.Exists(filePath + ".json"))
            {
                dataType = DataType.json;
            }
            else if (File.Exists(filePath + ".xml"))
            {
                dataType = DataType.xml;
            }
        }

        private static AtlasData ReadDataFromJSON()
        {
            string dataAsText = ReadFile(".json");
            return JsonUtility.FromJson<AtlasData>(dataAsText);
        }
        private static AtlasData ReadDataFromXML()
        {
            AtlasData data = new AtlasData();

            string[] dataAsLinesText = ReadFileLines(".xml");
            int index = -1;
            for (int i = 0; i < dataAsLinesText.Length; i++)
            {
                string lineText = dataAsLinesText[i];
                if (lineText.Contains("<AtlasTexture"))
                {
                    index = i + 1;
                    data.atlas = ReadAtlasInfoXML(lineText);
                    break;
                }
            }

            if (index < 0 || data.atlas.spriteCount <= 0)
            {
                Debug.LogError("Atlas data not found...");
                return null;
            }

            data.sprites = new rSprite[data.atlas.spriteCount];
            int spriteIndex = 0;

            for (int i = index; i < dataAsLinesText.Length; i++)
            {
                string lineText = dataAsLinesText[i];
                if (lineText.Contains("<Sprite"))
                {
                    data.sprites[spriteIndex] = ReadSpriteInfoXML(lineText);
                    spriteIndex++;
                }
            }
            return data;
        }
        private static Atlas ReadAtlasInfoXML(string lineText)
        {
            Atlas atlas = new Atlas();

            int value = 0;

            string[] words = lineText.Split('\'');

            atlas.imagePath = words[1];
            if (int.TryParse(words[3], out value)) atlas.width = value;
            else Debug.LogError("Parse width error");
            if (int.TryParse(words[5], out value)) atlas.height = value;
            else Debug.LogError("Parse height error");
            if (int.TryParse(words[7], out value)) atlas.spriteCount = value;
            else Debug.LogError("Parse spriteCount error");
            if (int.TryParse(words[9], out value))
            {
                if (value == 1) atlas.isFont = true;
                else atlas.isFont = false;
            }
            else Debug.LogError("Parse isFont error");
            if (int.TryParse(words[11], out value)) atlas.fontSize = value;
            else Debug.LogError("Parse fontSize error");


            return atlas;
        }
        private static rSprite ReadSpriteInfoXML(string lineText)
        {
            rSprite sprite = new rSprite();

            int value = 0;

            string[] words = lineText.Split('\'');

            sprite.nameId = words[1];
            if (int.TryParse(words[3], out value)) sprite.origin.x = value;
            else Debug.LogError("Parse originX error");
            if (int.TryParse(words[5], out value)) sprite.origin.y = value;
            else Debug.LogError("Parse originY error");
            if (int.TryParse(words[7], out value)) sprite.position.x = value;
            else Debug.LogError("Parse positionX error");
            if (int.TryParse(words[9], out value)) sprite.position.y = value;
            else Debug.LogError("Parse positionY error");
            if (int.TryParse(words[11], out value)) sprite.sourceSize.width = value;
            else Debug.LogError("Parse sourceSizeWidth error");
            if (int.TryParse(words[13], out value)) sprite.sourceSize.height = value;
            else Debug.LogError("Parse sourceSizeHeight error");
            if (int.TryParse(words[15], out value)) sprite.padding = value;
            else Debug.LogError("Parse padding error");
            if (int.TryParse(words[17], out value))
            {
                if (value == 1) sprite.trimmed = true;
                else sprite.trimmed = false;
            }
            else Debug.LogError("Parse trimmed error");
            if (int.TryParse(words[19], out value)) sprite.trimRec.x = value;
            else Debug.LogError("Parse trimRecX error");
            if (int.TryParse(words[21], out value)) sprite.trimRec.y = value;
            else Debug.LogError("Parse trimRecY error");
            if (int.TryParse(words[23], out value)) sprite.trimRec.width = value;
            else Debug.LogError("Parse trimRecWidth error");
            if (int.TryParse(words[25], out value)) sprite.trimRec.height = value;
            else Debug.LogError("Parse trimRecHeight error");

            return sprite;
        }

        private static AtlasData ReadDataFromRTPA()
        {
            AtlasData data = new AtlasData();

            string[] dataAsLinesText = ReadFileLines(".rtpa");

            int index = -1;
            for (int i = 0; i < dataAsLinesText.Length; i++)
            {
                string lineText = dataAsLinesText[i];
                if (lineText[0] == 'a')
                {
                    index = i+1;
                    data.atlas = ReadAtlasInfoRTPA(lineText);
                    break;
                }                
            }

            if (index < 0 || data.atlas.spriteCount <= 0)
            {
                Debug.LogError("Atlas data not found...");
                return null;
            }

            data.sprites = new rSprite[data.atlas.spriteCount];
            int spriteIndex = 0;

            for (int i = index; i < dataAsLinesText.Length; i++)
            {
                string lineText = dataAsLinesText[i];
                if (lineText[0] == 's')
                {
                    data.sprites[spriteIndex] = ReadSpriteInfoRTPA(lineText);
                    spriteIndex++;
                }
            }
            return data;
        }
        private static Atlas ReadAtlasInfoRTPA(string lineText)
        {
            Atlas atlas = new Atlas();

            int value = 0;

            string[] words = lineText.Split(' ');

            atlas.imagePath = words[1];
            if (int.TryParse(words[2], out value)) atlas.width = value;
            else Debug.LogError("Parse width error");
            if (int.TryParse(words[3], out value)) atlas.height = value;
            else Debug.LogError("Parse height error");
            if (int.TryParse(words[4], out value)) atlas.spriteCount = value;
            else Debug.LogError("Parse spriteCount error");
            if (int.TryParse(words[5], out value))
            {
                if (value == 1) atlas.isFont = true;
                else atlas.isFont = false;
            }
            else Debug.LogError("Parse isFont error");
            if (int.TryParse(words[6], out value)) atlas.fontSize = value;
            else Debug.LogError("Parse fontSize error");

            return atlas;
        }
        private static rSprite ReadSpriteInfoRTPA(string lineText)
        {
            rSprite sprite = new rSprite();

            int value = 0;

            string[] words = lineText.Split(' ');
        
            sprite.nameId = words[1];
            if (int.TryParse(words[2], out value)) sprite.origin.x = value;
            else Debug.LogError("Parse originX error");
            if (int.TryParse(words[3], out value)) sprite.origin.y = value;
            else Debug.LogError("Parse originY error");
            if (int.TryParse(words[4], out value)) sprite.position.x = value;
            else Debug.LogError("Parse positionX error");
            if (int.TryParse(words[5], out value)) sprite.position.y = value; 
            else Debug.LogError("Parse positionY error");
            if (int.TryParse(words[6], out value)) sprite.sourceSize.width = value;
            else Debug.LogError("Parse sourceSizeWidth error");
            if (int.TryParse(words[7], out value)) sprite.sourceSize.height = value;
            else Debug.LogError("Parse sourceSizeHeight error");
            if (int.TryParse(words[8], out value)) sprite.padding = value;
            else Debug.LogError("Parse padding error");
            if (int.TryParse(words[9], out value))
            {
                if (value == 1) sprite.trimmed = true;
                else sprite.trimmed = false;
            }
            else Debug.LogError("Parse trimmed error");
            if (int.TryParse(words[10], out value)) sprite.trimRec.x = value;
            else Debug.LogError("Parse trimRecX error");
            if (int.TryParse(words[11], out value)) sprite.trimRec.y = value; 
            else Debug.LogError("Parse trimRecY error");
            if (int.TryParse(words[12], out value)) sprite.trimRec.width = value; 
            else Debug.LogError("Parse trimRecWidth error");
            if (int.TryParse(words[13], out value)) sprite.trimRec.height = value;
            else Debug.LogError("Parse trimRecHeight error");

            return sprite;
        }

        private static string ReadFile(string extension)
        {
            return File.ReadAllText(filePath + extension);
        }
        private static string[] ReadFileLines(string extension)
        {
            return File.ReadAllLines(filePath + extension);
        }

        public static void ImportSpriteSheet(rSprite[] sprites)
        {
            string texturePath = AssetDatabase.GetAssetPath(texture2D);

            //TextureImporter ti = AssetDatabase.LoadAssetAtPath<TextureImporter>(texturePath);
            TextureImporter ti = TextureImporter.GetAtPath(texturePath) as TextureImporter;

            ti.isReadable = true;
            ti.spriteImportMode = SpriteImportMode.Single;
            List<SpriteMetaData> newData = new List<SpriteMetaData>();            

            for (int i = 0; i < sprites.Length; i++)
            { 
                newData.Add(GetSpriteMetaData(sprites[i]));
            }


           // ti.spritesheet = newData.ToArray();
            //TODO REPLACE WITH BOTTOM
            ISpriteEditorDataProvider spriteEditorDataProvider;
            var factory = new SpriteDataProviderFactories();
            factory.Init();
            spriteEditorDataProvider = factory.GetSpriteEditorDataProviderFromObject(ti);
            spriteEditorDataProvider.InitSpriteEditorDataProvider();

            var spriteRects = spriteEditorDataProvider.GetSpriteRects();

            for(int i = 0; i < spriteRects.Length; i++) {
                spriteRects[i].pivot = newData[i].pivot;
                spriteRects[i].alignment = SpriteAlignment.Custom;
                spriteRects[i].rect = newData[i].rect;
                spriteRects[i].name = newData[i].name;
            }

           /* foreach(var rect in spriteRects) {

                rect.pivot = newData[0].pivot;
                rect.alignment = SpriteAlignment.Custom;
                rect.rect = newData[0].rect;
            }*/

            spriteEditorDataProvider.SetSpriteRects(spriteRects);
            spriteEditorDataProvider.Apply();

            ti.spriteImportMode = SpriteImportMode.Multiple;
            AssetDatabase.ImportAsset(texturePath, ImportAssetOptions.ForceUpdate);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private static SpriteMetaData GetSpriteMetaData(rSprite sprite)
        {
            SpriteMetaData data = new SpriteMetaData();

            //data.pivot = new Vector2(sprite.origin.x, sprite.origin.y);
            data.pivot = new Vector2(0.5f, 0.5f);
            data.alignment = 9;
            data.name = sprite.nameId;

            data.rect = new Rect(
                sprite.position.x + sprite.padding,
                (atlasData.atlas.height - sprite.position.y - sprite.sourceSize.height - sprite.padding),
                sprite.sourceSize.width,
                sprite.sourceSize.height);

            if (sprite.trimmed)
            {
                data.rect.y = (atlasData.atlas.height - sprite.position.y - sprite.trimRec.height - sprite.padding);
                data.rect.width = sprite.trimRec.width;
                data.rect.height = sprite.trimRec.height;
            }

            return data;
        }
    }
}