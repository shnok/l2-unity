using UnityEngine;

public class VectorUtils : MonoBehaviour
{
    public static Vector3 ConvertPosToUnity(Vector3 ueVector)
    {
        return new Vector3(ueVector.y, ueVector.z, ueVector.x) * (1f / 52.5f);
    }

    public static Vector3 ConvertRotToUnity(Vector3 ueRot)
    {
        return new Vector3(
                            -360.00f * ueRot.x / 65536,
                            360.00f * ueRot.y / 65536,
                            -360.00f * ueRot.z / 65536
                        );
    }

    public static float ConvertRotToUnity(float value)
    {
        return 360.00f * value / 65536;
    }

    public static float ConvertRotToUnreal(float value)
    {
        return value / 360.00f * 65536;
    }

    public static Vector3 ConvertToUnityUnscaled(Vector3 ueVector)
    {
        return new Vector3(ueVector.y, ueVector.z, ueVector.x);
    }

    public static Vector3 ScaleToUnity(Vector3 ueVector)
    {
        return ueVector * (1f / 52.5f);
    }

    public static Vector2 RotateVector2(Vector2 vector, float angle)
    {
        float radians = angle * Mathf.Deg2Rad;
        return RotateVector2Rad(vector, radians);
    }

    public static Vector2 RotateVector2Rad(Vector2 vector, float radians)
    {
        float sin = Mathf.Sin(radians);
        float cos = Mathf.Cos(radians);

        float newX = vector.x * cos - vector.y * sin;
        float newY = vector.x * sin + vector.y * cos;

        return new Vector2(newX, newY);
    }

    public static Vector3 FloorToNearest(Vector3 vector, float step)
    {
        return new Vector3(NumberUtils.FloorToNearest(vector.x, step),
            NumberUtils.FloorToNearest(vector.y, step),
            NumberUtils.FloorToNearest(vector.z, step));
    }

    public static Vector3 To2D(Vector3 pos)
    {
        return new Vector3(pos.x, 0, pos.z);
    }

    public static float Distance2D(Vector3 from, Vector3 to)
    {
        return Vector3.Distance(To2D(from), To2D(to));
    }

    public static bool IsVectorZero2D(Vector3 vector)
    {
        return vector.x == 0 && vector.z == 0;
    }

    public static float CalculateMoveDirectionAngle(Vector3 from, Vector3 to)
    {
        // Calculate the direction vector (destination - current position)
        float directionX = to.x - from.x;
        float directionZ = to.z - from.z;

        return CalculateMoveDirectionAngle(directionX, directionZ);
    }

    public static float CalculateMoveDirectionAngle(float directionX, float directionZ)
    {
        float angle = Mathf.Atan2(directionX, directionZ) * Mathf.Rad2Deg;
        return angle;
    }
}
