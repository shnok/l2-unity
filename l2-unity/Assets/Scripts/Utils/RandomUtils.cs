using UnityEngine;

public class RandomUtils
{
    public static bool ShouldEventHappen(int probabilityPercentage)
    {
        // Validate the input probability
        if (probabilityPercentage < 0 || probabilityPercentage > 100)
        {
            throw new System.ArgumentException("Probability percentage must be between 0 and 100.");
        }

        // Generate a random value between 1 and 100
        float randomValue = Random.Range(1, 101);

        // Check if the random value is less than the desired probability
        return randomValue < probabilityPercentage;
    }
}
