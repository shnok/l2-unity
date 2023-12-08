using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomUtils
{
    public static bool ShouldEventHappen(int probabilityPercentage) {
        // Validate the input probability
        if(probabilityPercentage < 0 || probabilityPercentage > 100) {
            throw new System.ArgumentException("Probability percentage must be between 0 and 100.");
        }

        // Generate a random value between 0 and 1
        float randomValue = Random.Range(0f, 1f);

        // Check if the random value is less than the desired probability
        return randomValue < probabilityPercentage / 100f;
    }
}
