#if (UNITY_EDITOR) 
using System.Collections.Generic;
using UnityEngine;

public class TerrainToMesh : MonoBehaviour {
    public Terrain terrain;
    public GameObject dest;
    public int meshSubdivisions = 255;

    // Start is called before the first frame update
    void Start() {
        // BuildMeshBase();
        // ConvertTerrain();
        // AssetDatabase.ExportPackage("Assets/Data/Maps/17_25/TerrainData", "TerrainPackage.unitypackage");
    }

    public void BuildMeshBase() {
        Mesh planeMesh = new Mesh();
        SetVertices(planeMesh, meshSubdivisions);
        SetTriangles(planeMesh, meshSubdivisions);
        SetUVs(planeMesh, meshSubdivisions);

        var meshFilter = dest.AddComponent<MeshFilter>();
        meshFilter.mesh = planeMesh;
        var meshRenderer = dest.AddComponent<MeshRenderer>();
        Material material = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        meshRenderer.material = material;

        Vector3 meshScale = new Vector3(terrain.terrainData.size.x, 1, terrain.terrainData.size.z);
        dest.transform.localScale = meshScale;
        dest.transform.position = terrain.transform.position;
    }

    private void SetVertices(Mesh mesh, int subdivisions) {
        Vector3[] vertices = new Vector3[(subdivisions + 1) * (subdivisions + 1)];
        float step = 1f / subdivisions;
        for(int i = 0, z = 0; z <= subdivisions; z++) {
            for(int x = 0; x <= subdivisions; x++) {
                vertices[i] = new Vector3(x * step, 0, z * step);
                i++;
            }
        }
        mesh.vertices = vertices;
    }

    /*private void SetTriangles(Mesh mesh, int subdivisions) {
        int[] triangles = new int[subdivisions * subdivisions * 6];
        int vert = 0;
        int tris = 0;
        for(int z = 0; z < subdivisions; z++) {
            for(int x = 0; x < subdivisions; x++) {
                triangles[tris + 0] = vert + 0;
                triangles[tris + 1] = vert + subdivisions + 1;
                triangles[tris + 2] = vert + 1;
                triangles[tris + 3] = vert + 1;
                triangles[tris + 4] = vert + subdivisions + 1;
                triangles[tris + 5] = vert + subdivisions + 2;
                vert++;
                tris += 6;
            }
            vert++;
        }
        mesh.triangles = triangles;
    }*/

    private void SetTriangles(Mesh mesh, int subdivisions) {
        int[] triangles = new int[subdivisions * subdivisions * 6];
        int vert = 0;
        int tris = 0;
        for(int z = 0; z < subdivisions; z++) {
            for(int x = 0; x < subdivisions; x++) {
                triangles[tris + 0] = vert + 0;
                triangles[tris + 1] = vert + subdivisions + 1;
                triangles[tris + 2] = vert + 1;
                triangles[tris + 3] = vert + 1;
                triangles[tris + 4] = vert + subdivisions + 1;
                triangles[tris + 5] = vert + subdivisions + 2;
                vert++;
                tris += 6;
            }
            vert++; // Move to the next row
        }
        mesh.triangles = triangles;
    }

    private void SetUVs(Mesh mesh, int subdivisions) {
        Vector2[] uvs = new Vector2[mesh.vertices.Length];
        for(int i = 0, z = 0; z <= subdivisions; z++) {
            for(int x = 0; x <= subdivisions; x++) {
                uvs[i] = new Vector2((float)x / subdivisions, (float)z / subdivisions);
                i++;
            }
        }
        mesh.uv = uvs;
    }


    public void ConvertTerrain(bool optimize) {
        // get the bounds of the terrain
        var bounds = terrain.terrainData.bounds;

        var mf = dest.GetComponent<MeshFilter>();
        var mesh = mf.mesh;

        List<Vector3> newVerts = new List<Vector3>();
        foreach(var vert in mesh.vertices) {
            var wPos = dest.transform.localToWorldMatrix * vert;
            var newVert = vert;
            newVert.y = terrain.SampleHeight(wPos);
            newVerts.Add(newVert);
        }
        mesh.SetVertices(newVerts.ToArray());
        mesh.RecalculateNormals();
        mesh.RecalculateTangents();
        mesh.RecalculateBounds();

        if(optimize) {
            mesh.Optimize();
        }
    }
}
#endif