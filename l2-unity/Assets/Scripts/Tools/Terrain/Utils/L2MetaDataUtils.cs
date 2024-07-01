#if (UNITY_EDITOR) 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class L2MetaDataUtils {
    public static float ParseFloatFromInfo(string info) {
        int equalsIndex = info.IndexOf('=');
        string valueString = info.Substring(equalsIndex + 1, info.Length - equalsIndex - 1);
        return float.Parse(valueString);
    }

    public static bool ParseBoolFromInfo(string info) {
        int equalsIndex = info.IndexOf('=');
        string valueString = info.Substring(equalsIndex + 1, info.Length - equalsIndex - 1);
        return valueString.Equals("1");
    }

    public static int ParseIntFromInfo(string info) {
        int equalsIndex = info.IndexOf('=');
        string valueString = info.Substring(equalsIndex + 1, info.Length - equalsIndex - 1);
        return int.Parse(valueString);
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

    public static Vector3 ParseRotation(string line) {
        int equalsIndex = line.IndexOf('=');
        string valueString = line.Substring(equalsIndex + 1, line.Length - equalsIndex - 2);
        string[] valueParts = valueString.Split(',');

        float x = 0, y = 0, z = 0;
        foreach (string s in valueParts) {
            if (s.Contains("Pitch")) {
                x = int.Parse(s.Split("=")[1]);
            } else if (s.Contains("Yaw")) {
                y = int.Parse(s.Split("=")[1]);
            } else if (s.Contains("Roll")) {
                z = int.Parse(s.Split("=")[1]);
            }
        }

        return new Vector3(x, y, z);
    }

    public static Vector3 ParseVector3Poly(string line) {
        string[] valueParts = line.Split(',');

        float x = 0, y = 0, z = 0;
        x = float.Parse(valueParts[0]);
        y = float.Parse(valueParts[1]);
        z = float.Parse(valueParts[2]);

        return new Vector3(x, y, z);
    }

    public static string[] ParsePolyFlags(int value) {
        if (value == 0) {
            return new string[] { "PF_Default" };
        }

        Dictionary<string, int> flags = new Dictionary<string, int>();
        flags.Add("PF_Invisible", 1);
        flags.Add("PF_Masked", 2);
        flags.Add("PF_Translucent", 4);
        flags.Add("PF_NotSolid", 8);
        flags.Add("PF_Environment", 16);
        flags.Add("PF_ForceViewZone", 16);
        flags.Add("PF_Semisolid", 32);
        flags.Add("PF_Modulated", 64);
        flags.Add("PF_FakeBackdrop", 128);
        flags.Add("PF_TwoSided", 256);
        flags.Add("PF_AutoUPan", 512);
        flags.Add("PF_AutoVPan", 1024);
        flags.Add("PF_NoSmooth", 2048);
        flags.Add("PF_BigWavy", 4096);
        flags.Add("PF_SpecialPoly", 4096);
        flags.Add("PF_SmallWavy", 8192);
        flags.Add("PF_Flat", 16384);
        flags.Add("PF_LowShadowDetail", 32768);
        flags.Add("PF_NoMerge", 65536);
        flags.Add("PF_CloudWavy", 131072);
        flags.Add("PF_DirtyShadows", 262144);
        flags.Add("PF_BrightCorners", 524288);
        flags.Add("PF_SpecialLit", 1048576);
        flags.Add("PF_Gouraud", 2097152);
        flags.Add("PF_NoBoundRejection", 2097152);
        flags.Add("PF_Unlit", 4194304);
        flags.Add("PF_HighShadowDetail", 8388608);
        flags.Add("PF_Memorized", 16777216);
        flags.Add("PF_RenderHint", 16777216);
        flags.Add("PF_Selected", 33554432);
        flags.Add("PF_Portal", 67108864);
        flags.Add("PF_Mirrored", 134217728);
        flags.Add("PF_Highlighted", 268435456);
        flags.Add("unused", 536870912);
        flags.Add("PF_FlatShaded", 1073741824);
        flags.Add("PF_EdProcessed", 1073741824);
        flags.Add("PF_RenderFog", 1073741824);
        /*flags.Add("PF_EdCut", 2147483648);
        flags.Add("PF_Occlude", 2147483648);*/

        List<string> setFlags = new List<string>();

        foreach (var entry in flags) {
            if ((value & entry.Value) != 0) {
                setFlags.Add(entry.Key);
            }
        }

        foreach (string flag in setFlags) {
           // Debug.Log(flag);
        }

        return setFlags.ToArray();
    }
}
#endif