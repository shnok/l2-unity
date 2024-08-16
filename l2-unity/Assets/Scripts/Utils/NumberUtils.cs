using System;
using UnityEngine;

public class NumberUtils {

    public static float ScaleToUnity(int value) {
        return value * (1f / 52.5f);
    }

    public static float FloorToNearest(float value, float step) {
        return step * Mathf.Floor(value / step);
    }

    public static float FromIntToFLoat(int value) {
        // Convert integer to byte array
        byte[] byteArray = BitConverter.GetBytes(value);

        // Convert byte array to float
        float floatValue = BitConverter.ToSingle(byteArray, 0);

        return floatValue;
    }
}
