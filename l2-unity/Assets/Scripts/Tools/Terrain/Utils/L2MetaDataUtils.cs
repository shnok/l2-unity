#if (UNITY_EDITOR) 
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class L2MetaDataUtils
{
    public static float ParseFloatFromInfo(string info)
    {
        int equalsIndex = info.IndexOf('=');
        string valueString = info.Substring(equalsIndex + 1, info.Length - equalsIndex - 1);
        return float.Parse(valueString);
    }

    public static bool ParseBoolFromInfo(string info)
    {
        int equalsIndex = info.IndexOf('=');
        string valueString = info.Substring(equalsIndex + 1, info.Length - equalsIndex - 1);
        return valueString.Equals("1");
    }

    public static int ParseIntFromInfo(string info)
    {
        int equalsIndex = info.IndexOf('=');
        string valueString = info.Substring(equalsIndex + 1, info.Length - equalsIndex - 1);
        return int.Parse(valueString);
    }

    public static string[] GetFolderAndFileFromInfo(string value)
    {
        string textureName = value.Contains("=") ? value.Split('=')[1] : value;

        textureName = textureName.Replace("Texture'", string.Empty);
        textureName = textureName.Replace("StaticMesh'", string.Empty);
        /*textureName = textureName.Replace(".Texture", string.Empty);
		textureName = textureName.Replace("Height.", string.Empty);*/
        textureName = textureName.Replace("'", string.Empty);

        string[] result = textureName.Split('.');

        if (result.Length == 2)
        {
            return result;
        }
        else if (result.Length > 2)
        {
            return new string[2] { result[0], result[2] };
        }

        return result;
    }

    public static Vector3 ParseVector3(string line)
    {
        int equalsIndex = line.IndexOf('=');
        string valueString = line.Substring(equalsIndex + 1, line.Length - equalsIndex - 2);
        string[] valueParts = valueString.Split(',');

        float x = 0, y = 0, z = 0;
        foreach (var part in valueParts)
        {

            if (part.Contains("X"))
            {
                x = float.Parse(part.Substring(part.IndexOf('=') + 1));
            }
            if (part.Contains("Y"))
            {
                y = float.Parse(part.Substring(part.IndexOf('=') + 1));
            }
            if (part.Contains("Z"))
            {
                z = float.Parse(part.Substring(part.IndexOf('=') + 1));
            }
        }

        return new Vector3(x, y, z);
    }

    public static Vector3 ParseRotation(string line)
    {
        int equalsIndex = line.IndexOf('=');
        string valueString = line.Substring(equalsIndex + 1, line.Length - equalsIndex - 2);
        string[] valueParts = valueString.Split(',');

        float x = 0, y = 0, z = 0;
        foreach (string s in valueParts)
        {
            if (s.Contains("Pitch"))
            {
                x = int.Parse(s.Split("=")[1]);
            }
            else if (s.Contains("Yaw"))
            {
                y = int.Parse(s.Split("=")[1]);
            }
            else if (s.Contains("Roll"))
            {
                z = int.Parse(s.Split("=")[1]);
            }
        }

        return new Vector3(x, y, z);
    }

    public static bool ParseBool(string line)
    {
        string[] valueParts = line.Split('=');
        if (valueParts[1].ToLower() == "true")
        {
            return true;
        }

        return false;
    }

    public static float ParseFloat(string line)
    {
        string[] valueParts = line.Split('=');
        return float.Parse(valueParts[1]);
    }

    public static int ParseInt(string line)
    {
        string[] valueParts = line.Split('=');
        return int.Parse(valueParts[1]);
    }

    public static ColorScale ParseColorScale(string line)
    {
        int equalsIndex = line.IndexOf('=');
        string valueString = line.Substring(equalsIndex + 1, line.Length - equalsIndex - 1);


        float relativeTime = 0;
        int r = 0;
        int g = 0;
        int b = 0;
        int a = 0;
        string colorString = "";
        string relativeTimeString = "";

        if (valueString.Contains("RelativeTime") && valueString.Contains("Color"))
        {
            int id = valueString.IndexOf(',');
            colorString = valueString.Substring(id + 1, valueString.Length - id - 1);
            relativeTimeString = valueString.Substring(0, id);
        }
        else if (valueString.Contains("Color"))
        {
            colorString = valueString;
        }
        else
        {
            relativeTimeString = valueString;
        }

        ColorScale colorScale = new ColorScale();

        if (relativeTimeString.Length > 0)
        {
            float.TryParse(relativeTimeString.Replace("RelativeTime=", ""), out relativeTime);
        }

        if (colorString.Length > 0)
        {
            colorString = colorString.Replace("Color=", "");

            string[] rgba = colorString.Split(",");

            for (int i = 0; i < rgba.Length; i++)
            {
                string[] keyval = rgba[i].Split("=");
                int val = int.Parse(keyval[1]);

                if (keyval[0] == "R")
                {
                    r = val;
                }
                else if (keyval[0] == "G")
                {
                    g = val;
                }
                else if (keyval[0] == "B")
                {
                    b = val;
                }
                else if (keyval[0] == "A")
                {
                    a = val;
                }
            }

            // FUNCTION USED TO PARSE SYSTEM MESSAGE COLORS
            /*
             case "color":
                            systemMessage.Color = value;
                            string r = systemMessage.Color.Substring(0, 2);
                            string g = systemMessage.Color.Substring(2, 2);
                            string b = systemMessage.Color.Substring(4, 2);
                            string a = systemMessage.Color.Substring(6, 2);
                            systemMessage.Color =  b + g + r + "B0";
                            */
        }

        colorScale.relativeTime = relativeTime;
        colorScale.r = r;
        colorScale.g = g;
        colorScale.b = b;
        colorScale.a = a;

        return colorScale;
    }

    public static SizeScale ParseSizeScale(string line)
    {
        int equalsIndex = line.IndexOf('=');
        string valueString = line.Substring(equalsIndex + 1, line.Length - equalsIndex - 2);

        SizeScale sizeScale = new SizeScale();

        string[] timeSize = valueString.Split(",");
        Debug.Log(line);
        for (int i = 0; i < timeSize.Length; i++)
        {
            string[] keyVal = timeSize[i].Split("=");
            if (keyVal[0] == "RelativeTime")
            {
                sizeScale.relativeTime = float.Parse(keyVal[1]);
            }
            else
            {
                sizeScale.relativeSize = float.Parse(keyVal[1]);
            }
        }

        return sizeScale;
    }

    public static Range3D ParseRange3D(string line)
    {
        int equalsIndex = line.IndexOf('=');
        line = line.Substring(equalsIndex + 1, line.Length - equalsIndex - 2);

        Range3D range3D = new Range3D();

        line = line.Replace(",Max", "|Max");

        string[] xyz = line.Split(",");

        for (int i = 0; i < xyz.Length; i++)
        {
            bool x = xyz[i].Contains("X=");
            bool y = xyz[i].Contains("Y=");
            bool z = xyz[i].Contains("Z=");

            xyz[i] = xyz[i].Replace("X=", "").Replace("Y=", "").Replace("Z=", "");

            Range range = ParseRange(xyz[i]);
            if (x) range3D.x = range;
            if (y) range3D.y = range;
            if (z) range3D.z = range;
        }

        return range3D;
    }

    public static Range ParseRange(string line)
    {
        line = line.Replace(",Max", "|Max");

        string[] minMax = line.Split("|");

        Debug.LogWarning(line);
        Range range = new Range();
        float min = 0;
        float max = 0;
        for (int q = 0; q < minMax.Length; q++)
        {
            if (minMax[q].Contains("Min="))
            {
                minMax[q] = minMax[q].Replace("Min=", "");
                min = float.Parse(minMax[q]);
            }
            else
            {
                minMax[q] = minMax[q].Replace("Max=", "");
                max = float.Parse(minMax[q]);
            }
        }

        range.min = min;
        range.max = max;

        return range;
    }

    public static Vector3 ParseVector3Poly(string line)
    {
        string[] valueParts = line.Split(',');

        float x = 0, y = 0, z = 0;
        x = float.Parse(valueParts[0]);
        y = float.Parse(valueParts[1]);
        z = float.Parse(valueParts[2]);

        return new Vector3(x, y, z);
    }

    public static string[] ParsePolyFlags(int value)
    {
        if (value == 0)
        {
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

        foreach (var entry in flags)
        {
            if ((value & entry.Value) != 0)
            {
                setFlags.Add(entry.Key);
            }
        }

        foreach (string flag in setFlags)
        {
            // Debug.Log(flag);
        }

        return setFlags.ToArray();
    }
}
#endif