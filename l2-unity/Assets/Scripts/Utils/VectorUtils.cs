using UnityEngine;

public class VectorUtils : MonoBehaviour {
    public static Vector3 convertToUnity(Vector3 ueVector) {
        return new Vector3(ueVector.y, ueVector.z, ueVector.x) * (1f / 52.5f);
    }

    public static Vector3 convertToUnityUnscaled(Vector3 ueVector) {
        return new Vector3(ueVector.y, ueVector.z, ueVector.x);
    }

    public static Vector3 scaleToUnity(Vector3 ueVector) {
        return ueVector * (1f / 52.5f);
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

    public static Vector3 floorToNearest(Vector3 vector, float step) {
        return new Vector3(NumberUtils.FloorToNearest(vector.x, step),
            NumberUtils.FloorToNearest(vector.y, step),
            NumberUtils.FloorToNearest(vector.z, step));
    }

    public static Vector3 To2D(Vector3 pos) {
        return new Vector3(pos.x, 0, pos.z);
    }
}
