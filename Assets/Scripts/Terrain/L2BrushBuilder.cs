using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class L2BrushBuilder : MonoBehaviour
{
    public Brush[] test;
    private void Start() {
        test = L2BrushImporter.ParseBrushFile("D:\\Stock\\Projects\\L2-Unity\\Tools\\l2brushexport\\17_25_Classic.json");
        Debug.Log(test.GetValue(0).ToString());
    }

    [MenuItem("Shnok/[Brush] Parse")]
    static void ReadJson() {
       L2BrushImporter.ParseBrushFile("D:\\Stock\\Projects\\L2-Unity\\Tools\\l2brushexport\\17_25_Classic.json");
    }

    [MenuItem("Shnok/[Brush] Build")]
    static void Build()
    {
        Brush[] brushes = L2BrushImporter.ParseBrushFile("D:\\Stock\\Projects\\L2-Unity\\Tools\\l2brushexport\\17_25_Classic.json");

        GameObject brushContainer = new GameObject("Brushes");

        foreach(Brush b in brushes) {
            if(b.position == Vector3.zero) {
                Debug.LogWarning(b.name + " position is null");
                continue;
            }
            List<string> polyFlags = new List<string>(b.polyFlags);
            if(polyFlags.Contains("PF_Invisible")) {
                Debug.LogWarning(b.name + " position is invisible");
                continue;
            }
            if(b.csgOper == "CSG_Subtract") {
                Debug.LogWarning(b.name + " position is CSG_Subtract");
                continue;
            }

            GameObject brush = new GameObject(b.name);
            brush.transform.parent = brushContainer.transform;
            brush.transform.position = VectorUtils.convertToUnity(b.position) - VectorUtils.convertToUnity(b.prePivot);

            Model model = b.model;
            Poly poly = model.poly;

            for(int i = 0; i < poly.polyData.Length; i++) {
                GameObject mesh = createMesh(poly.polyData[i].vertices);
                mesh.transform.parent = brush.transform;
                mesh.transform.localPosition = Vector3.zero;
            }
        }

    }

    static GameObject createMesh(Vector3[] vertices) {
        float ueToUnityScale = (1f / 52.5f);

        for(int i = 0; i < vertices.Length; i++) {
            vertices[i] = VectorUtils.convertToUnity(vertices[i]);
        }

        GameObject customShapeObject = new GameObject("CustomShape");

        MeshFilter meshFilter = customShapeObject.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = customShapeObject.AddComponent<MeshRenderer>();

        int[] triangles = new int[0];

        if(vertices.Length == 4) {
            triangles = new int[6]
            {
                0, 1, 2, // First triangle (top-left, top-right, bottom-left)
                2, 3, 0  // Second triangle (bottom-left, top-right, bottom-right)
            };
        } else if(vertices.Length == 3) {
            triangles = new int[3]
            {
                0, 1, 2, // First triangle (top-left, top-right, bottom-left)
            };
        }

        // Create a new Mesh
        Mesh customMesh = new Mesh();
        customMesh.vertices = vertices;
        customMesh.triangles = triangles;

        // Assign the mesh to the MeshFilter
        meshFilter.mesh = customMesh;

        meshRenderer.material = AssetDatabase.LoadAssetAtPath<Material>("Assets/Prefab/Blue.mat");

        return customShapeObject;
    }
}
