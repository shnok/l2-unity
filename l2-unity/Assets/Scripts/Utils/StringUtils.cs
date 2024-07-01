using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class StringUtils
{
    public static string GenerateRandomString() {
        var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        var stringChars = new char[8];
        var random = new System.Random();

        for(int i = 0; i < stringChars.Length; i++) {
            stringChars[i] = chars[random.Next(chars.Length)];
        }

        var finalString = new String(stringChars);
        return finalString;
    }

    public static Color HexToColor(string hex) {
        // Remove the alpha channel from the string (last two characters)
        hex = hex.Substring(0, 6);

        byte r = byte.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
        byte g = byte.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
        byte b = byte.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);

        // Convert the color values to the range [0, 1]
        float rf = r / 255f;
        float gf = g / 255f;
        float bf = b / 255f;

        return new Color(rf, gf, bf);
    }


    public static string ByteArrayToString(byte[] array) {
        if (array == null || array.Length == 0)
            return "[]";

        StringBuilder sb = new StringBuilder();
        sb.Append("[");
        for (int i = 0; i < array.Length; i++) {
            if (i > 0)
                sb.Append(", ");

            // Convert byte value to string representation
            sbyte value = (sbyte)array[i];
            sb.Append(value.ToString());
        }
        sb.Append("]");

        return sb.ToString();
    }

    public static string ByteArrayToIpAddress(byte[] ipArray) {
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < ipArray.Length; i++) {
            sb.Append(ipArray[i]);
            if(i < ipArray.Length - 1) {
                sb.Append(".");
            }
        }
        return sb.ToString();
    }
}
