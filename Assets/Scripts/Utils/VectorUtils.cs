using UnityEngine;

public class VectorUtils : MonoBehaviour {
    public static Vector3 convertToUnity(Vector3 ueVector) {
        return new Vector3(ueVector.y, ueVector.z, ueVector.x) * (1f / 52.5f);
    }

    public static Vector3 convertToUnityUnscaled(Vector3 ueVector) {
        return new Vector3(ueVector.y, ueVector.z, ueVector.x);
    }

    public static Vector2 rotateVector2(Vector2 vector, float angle) {
        float radians = angle * Mathf.Deg2Rad;
        return rotateVector2Rad(vector, radians);
    }

    public static Vector2 rotateVector2Rad(Vector2 vector, float radians) {
        float sin = Mathf.Sin(radians);
        float cos = Mathf.Cos(radians);

        float newX = vector.x * cos - vector.y * sin;
        float newY = vector.x * sin + vector.y * cos;

        return new Vector2(newX, newY);
    }

    public static float floorToNearest(float value, float step) {
        return step * Mathf.Floor(value / step);
    }

    public static Vector3 floorToNearest(Vector3 vector, float step) {
        return new Vector3(floorToNearest(vector.x, step),
            floorToNearest(vector.y, step),
            floorToNearest(vector.z, step));
    }
}
