using System;
using System.Collections;
using System.Collections.Generic;
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
}
