#if (UNITY_EDITOR) 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class L2MetaDataUtils {
    public static float ParseFloatFromInfo(string info) {
        int equalsIndex = info.IndexOf('=');
        string valueString = info.Substring(equalsIndex + 1, info.Length - equalsIndex - 2);
        return float.Parse(valueString);
    }

    public static bool ParseBoolFromInfo(string info) {
        int equalsIndex = info.IndexOf('=');
        string valueString = info.Substring(equalsIndex + 1, info.Length - equalsIndex - 1);
        return valueString.Equals("1");
    }

    public static string[] GetFolderAndFileFromInfo(string value) {
        string textureName = value.Contains("=") ? value.Split('=')[1] : value;

        textureName = textureName.Replace("Texture'", string.Empty);
        textureName = textureName.Replace("StaticMesh'", string.Empty);
        /*textureName = textureName.Replace(".Texture", string.Empty);
		textureName = textureName.Replace("Height.", string.Empty);*/
        textureName = textureName.Replace("'", string.Empty);

        string[] result = textureName.Split('.');

        if (result.Length == 2) {
            return result;
        } else if (result.Length > 2) {
            return new string[2] { result[0], result[2] };
        }

        return result;
    }

    public static Vector3 ParseVector3(string line) {
        int equalsIndex = line.IndexOf('=');
        string valueString = line.Substring(equalsIndex + 1, line.Length - equalsIndex - 2);
        string[] valueParts = valueString.Split(',');

        float x = 0, y = 0, z = 0;
        foreach (var part in valueParts) {
          
            if(part.Contains("X")) {
                x = float.Parse(part.Substring(part.IndexOf('=') + 1));
            }
            if (part.Contains("Y")) {
                y = float.Parse(part.Substring(part.IndexOf('=') + 1));
            }
            if (part.Contains("Z")) {
                z = float.Parse(part.Substring(part.IndexOf('=') + 1));
            }
        }

        return new Vector3(x, y, z);
    }
}
#endif