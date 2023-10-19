using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PolyData
{
    public Vector3 origin;
    public Vector3 normal;
    public Vector3 textureU;
    public Vector3 textureV;
    public int vertexCount;
    public Vector3[] vertices;
}
