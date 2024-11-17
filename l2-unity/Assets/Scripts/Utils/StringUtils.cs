using System;
using System.Text;
using UnityEngine;

public class StringUtils
{
    public static string GenerateRandomString()
    {
        var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        var stringChars = new char[8];
        var random = new System.Random();

        for (int i = 0; i < stringChars.Length; i++)
        {
            stringChars[i] = chars[random.Next(chars.Length)];
        }

        var finalString = new String(stringChars);
        return finalString;
    }

    public static string ByteArrayToString(byte[] array)
    {
        if (array == null || array.Length == 0)
            return "[]";

        StringBuilder sb = new StringBuilder();
        sb.Append("[");
        for (int i = 0; i < array.Length; i++)
        {
            if (i > 0)
                sb.Append(", ");

            // Convert byte value to string representation
            sbyte value = (sbyte)array[i];
            sb.Append(value.ToString());
        }
        sb.Append("]");

        return sb.ToString();
    }

    public static string ByteArrayToIpAddress(byte[] ipArray)
    {
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < ipArray.Length; i++)
        {
            sb.Append(ipArray[i]);
            if (i < ipArray.Length - 1)
            {
                sb.Append(".");
            }
        }
        return sb.ToString();
    }

    public static string Base64Encode(string plainText)
    {
        if (plainText == null)
        {
            return "";
        }

        var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
        return System.Convert.ToBase64String(plainTextBytes);
    }
}
