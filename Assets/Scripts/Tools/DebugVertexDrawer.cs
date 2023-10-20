using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class VertexDebugger : MonoBehaviour {
    // Define the vertex positions and normals
    Vector3[] vertices = new Vector3[]
    {
        new Vector3(0, 0, 0),
        new Vector3(1, 0, 0),
        new Vector3(1, 1, 0),
        new Vector3(0, 1, 0),
    };

    Vector3[] normals = new Vector3[]
    {
        new Vector3(0, 0, 1),
        new Vector3(0, 0, 1),
        new Vector3(0, 0, 1),
        new Vector3(0, 0, 1),
    };

    void OnDrawGizmos() {
        Gizmos.color = Color.green;

        // Draw the polygon
        for(int i = 0; i < vertices.Length; i++) {
            Gizmos.DrawLine(vertices[i], vertices[(i + 1) % vertices.Length]);
            Gizmos.DrawLine(vertices[i], vertices[i] + normals[i]);
        }
    }

    float ueToUnityScale = (1f / 52.5f);
    void createMesh(Vector3[] vertices) {
        for(int i = 0; i < vertices.Length; i++) {
            vertices[i] = new Vector3(vertices[i].y, vertices[i].z, vertices[i].x) * ueToUnityScale;
        }
   
        GameObject customShapeObject = new GameObject("CustomShape");

        // Create a MeshFilter and MeshRenderer for the custom shape
        MeshFilter meshFilter = customShapeObject.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = customShapeObject.AddComponent<MeshRenderer>();


        // Define the triangles (assuming you want to create a quad)
        int[] triangles = new int[]
        {
            0, 1, 2,
            2, 3, 0
        };

        // Create a new Mesh
        Mesh customMesh = new Mesh();
        customMesh.vertices = vertices;
        customMesh.triangles = triangles;

        // Assign the mesh to the MeshFilter
        meshFilter.mesh = customMesh;

        meshRenderer.material = AssetDatabase.LoadAssetAtPath<Material>("Assets/Prefab/Blue.mat");

        Vector3 pos = new Vector3(240934, -3754, -84380);
        Vector3 pivot = new Vector3(-243, -3, -127);
        customShapeObject.transform.position = new Vector3(pos.x - pivot.x, pos.y - pivot.y, pos.z - pivot.z) * ueToUnityScale;
    }
    void Start() {
        Vector3[] vertexSet1 = new Vector3[] {
            new Vector3(-126.291695f, -243.29361f, 2.0f),
            new Vector3(-126.291695f, -243.2936f, 4.0f),
            new Vector3(272.5163f, -642.10156f, 4.0f),
            new Vector3(272.51627f, -642.10156f, 2.0f)
        };

        Vector3[] vertexSet2 = new Vector3[] {
            new Vector3(272.51627f, -642.10156f, 2.0f),
            new Vector3(272.5163f, -642.10156f, 4.0f),
            new Vector3(54.727646f, -859.89056f, 4.0f),
            new Vector3(54.727646f, -859.89056f, 2.0f)
        };

        Vector3[] vertexSet3 = new Vector3[] {
            new Vector3(54.727646f, -859.89056f, 2.0f),
            new Vector3(54.727646f, -859.89056f, 4.0f),
            new Vector3(-344.08044f, -461.08252f, 4.0f),
            new Vector3(-344.08044f, -461.08252f, 2.0f)
        };

        Vector3[] vertexSet4 = new Vector3[] {
            new Vector3(-344.08044f, -461.08252f, 2.0f),
            new Vector3(-344.08044f, -461.08252f, 4.0f),
            new Vector3(-126.291695f, -243.2936f, 4.0f),
            new Vector3(-126.291695f, -243.29361f, 2.0f)
        };

        Vector3[] vertexSet5 = new Vector3[] {
            new Vector3(272.5163f, -642.10156f, 4.0f),
            new Vector3(-126.291695f, -243.2936f, 4.0f),
            new Vector3(-344.08044f, -461.08252f, 4.0f),
            new Vector3(54.727646f, -859.89056f, 4.0f)
        };

        Vector3[] vertexSet6 = new Vector3[] {
            new Vector3(-126.291695f, -243.29361f, 2.0f),
            new Vector3(272.51627f, -642.10156f, 2.0f),
            new Vector3(54.727646f, -859.89056f, 2.0f),
            new Vector3(-344.08044f, -461.08252f, 2.0f)
        };


        createMesh(vertexSet1);
        createMesh(vertexSet2);
        createMesh(vertexSet3);
        createMesh(vertexSet4);
        createMesh(vertexSet5);
        createMesh(vertexSet6);


    }
}
