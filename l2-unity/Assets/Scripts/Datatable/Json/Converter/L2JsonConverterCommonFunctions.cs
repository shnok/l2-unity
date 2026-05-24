using System.Globalization;
using UnityEngine;

public class L2JsonConverterCommonFunctions
{
    public static Vector3 ParseVector3(string vectorString)
    {
        if (string.IsNullOrEmpty(vectorString))
            return Vector3.zero;

        vectorString = vectorString.Trim('(', ')');
        string[] components = vectorString.Split(',');

        Vector3 result = Vector3.zero;
        foreach (string component in components)
        {
            string[] keyValue = component.Split('=');
            if (keyValue.Length == 2)
            {
                float value = float.Parse(keyValue[1], System.Globalization.NumberStyles.Float, CultureInfo.InvariantCulture);
                switch (keyValue[0].Trim().ToUpper())
                {
                    case "X": result.z = value; break;
                    case "Y": result.x = value; break;
                    case "Z": result.y = value; break;
                }
            }
        }

        return result;
    }
}