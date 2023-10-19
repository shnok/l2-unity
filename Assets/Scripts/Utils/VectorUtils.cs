using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VectorUtils : MonoBehaviour
{
    public static Vector3 convertToUnity(Vector3 ueVector) {
        return new Vector3(ueVector.y, ueVector.z, ueVector.x) * (1f/52.5f);
    }
}
