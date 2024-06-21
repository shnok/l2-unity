#if (UNITY_EDITOR) 
using UnityEngine;

[System.Serializable]
public class PolyData {
    public int polyIndex;
    public string texture;
    public string[] polyFlags;
    public Vector3 origin;
    public Vector3 normal;
    public Vector3 textureU;
    public Vector3 textureV;
    public int vertexCount;
    public Vector3[] vertices;

    public override string ToString() {
        return $"polyIndex: {polyIndex}, texture: {texture}, origin: {origin}, normal: {normal}, textureU: {textureU}, textureV: {textureV}, vertexCount: {vertexCount}";
    }
}
#endif