using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NumberUtils {

    public static float ScaleToUnity(int value) {
        return value * (1f / 52.5f);
    }

    public static float FloorToNearest(float value, float step) {
        return step * Mathf.Floor(value / step);
    }
}
