#if (UNITY_EDITOR) 
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.ProBuilder;

public class L2BrushBuilder {
    [MenuItem("Shnok/[Brush] (JSON) Build brushes")]
    static void ImportBrushTextures() {
        string title = "Select Brush list";
        string directory = Path.Combine(Application.dataPath, "Data/Maps");
        string extension = "json";

        string fileToProcess = EditorUtility.OpenFilePanel(title, directory, extension);

        if(!string.IsNullOrEmpty(fileToProcess)) {
            Debug.Log("Selected file: " + fileToProcess);

            Brush[] brushes = L2JSONBrushImporter.ParseBrushFile(fileToProcess);

            Build(brushes);
        }
    }

    [MenuItem("Shnok/5. [Brush] (T3D) Build brushes")]
    static void ImportBrushTexturesT3D() {
        string title = "Select T3D file";
        string directory = Path.Combine(Application.dataPath, "Data/Maps");
        string extension = "t3d";

        string fileToProcess = EditorUtility.OpenFilePanel(title, directory, extension);

        if (!string.IsNullOrEmpty(fileToProcess)) {
            Debug.Log("Selected file: " + fileToProcess);
            Brush[] brushes = L2T3DInfoParser.ParseBrushInfo(fileToProcess).ToArray();
            Build(brushes);
        }
    }

    static void Build(Brush[] brushes) {
        GameObject brushContainer = new GameObject("Brushes");

        foreach(Brush b in brushes) {
            if(b.position == Vector3.zero) {
                Debug.LogWarning(b.name + " position is null");
                continue;
            }
            if(b.polyFlags != null) {
                List<string> polyFlags = new List<string>(b.polyFlags);
                if (polyFlags.Contains("PF_Invisible")) {
                    continue;
                }
                if (polyFlags.Contains("PF_NotSolid")) {
                    continue;
                }
            }

            GameObject brush = new GameObject(b.name);
            brush.transform.parent = brushContainer.transform;
            brush.transform.position = VectorUtils.ConvertPosToUnity(b.position) - VectorUtils.ConvertPosToUnity(b.prePivot);

            Debug.Log(b.name);
            Model model = b.model;
            Poly poly = model.poly;

            for(int i = 0; i < poly.polyData.Length; i++) {
                PolyData polyData = poly.polyData[i];

                // Adjust verticles
                for(int p = 0; p < polyData.vertices.Length; p++) {
                    polyData.vertices[p] = VectorUtils.ConvertPosToUnity(polyData.vertices[p]);
                }

                List<string> pPolyFlags = new List<string>(polyData.polyFlags);

                // Skip invisible faces
                if(pPolyFlags.Contains("PF_Unlit") || pPolyFlags.Contains("PF_Invisible") || pPolyFlags.Contains("PF_NotSolid")) {
                    continue;
                }
                
                if(b.csgOper == "CSG_Subtract") {
                    // Only draw bottom face
                    // if (i != poly.polyData.Length - 1) {
                    //continue;
                    //}
                }

                //GameObject mesh = createMesh(b.csgOper, polyData);

                GameObject mesh = createProbuilderMesh(b.csgOper, polyData, i);
                mesh.transform.parent = brush.transform;
                mesh.transform.localPosition = Vector3.zero;
            }
        }

    }

    static GameObject createProbuilderMesh(string csgOper, PolyData polyData, int index) {

        Material material = GetMaterialForTexture(polyData.texture);

        Vector3 adjustedNormal = VectorUtils.ConvertToUnityUnscaled(polyData.normal);
        Vector3 adjustedU = VectorUtils.ConvertToUnityUnscaled(polyData.textureU);
        Vector3 adjustedV = VectorUtils.ConvertToUnityUnscaled(polyData.textureV);

        Quaternion rt = Quaternion.FromToRotation(Vector3.forward, adjustedNormal);
        Vector3 rotatedU = rt * adjustedU;
        Vector3 rotatedV = rt * adjustedV;
        if(adjustedNormal.y < 0) {
            rotatedU.x = -rotatedU.x;
        } else {
            rotatedU.y = -rotatedU.y;
        }

        // Create Vertices
        List<Vertex> vertexList = new List<Vertex>();
        foreach(var v in polyData.vertices) {
            Vertex vertex = new Vertex();
            vertex.position = v;
            vertexList.Add(vertex);
        }

        // Create faces
        Face[] faces = new Face[1];
        Face face = new Face(GenerateTris(csgOper, polyData));

        AutoUnwrapSettings aus = new AutoUnwrapSettings {
            anchor = AutoUnwrapSettings.Anchor.UpperLeft,
            scale = Vector2.one,
            offset = Vector2.zero,
            rotation = 0,
            fill = AutoUnwrapSettings.Fill.Tile,
            useWorldSpace = false
        };
        face.uv = aus;
        face.manualUV = true;
        faces[0] = face;

        // Create empty sharedVertices and sharedTextures lists
        List<SharedVertex> sharedVertices = new List<SharedVertex>();
        List<SharedVertex> sharedTextures = new List<SharedVertex>();

        // Create Mesh
        ProBuilderMesh pbm = ProBuilderMesh.Create(
            vertexList,
            faces,
            sharedVertices,
            sharedTextures
        );

        List<Vector4> uvs = new List<Vector4>();
        for(int i = 0; i < vertexList.Count; i++) {
            uvs.Add(new Vector4(rotatedU.x, rotatedU.y, rotatedV.x, rotatedV.y));
        }
        pbm.SetUVs(0, uvs);
        pbm.Refresh();

        Material[] sharedMaterials = pbm.GetComponent<Renderer>().sharedMaterials;
        sharedMaterials[0] = material;
        pbm.GetComponent<Renderer>().sharedMaterials = sharedMaterials;

        pbm.ToMesh();
        pbm.Refresh();
        pbm.RebuildWithPositionsAndFaces(polyData.vertices, faces);

        pbm.gameObject.transform.name = index.ToString();
        return pbm.gameObject;
    }

    private static int[] GenerateTris(string csgOper, PolyData polyData) {
        int[] triangles;

        if(csgOper == "CSG_Subtract") {
            if(polyData.vertices.Length == 4) {
                triangles = new int[6]
                {
                    2, 1, 0, // First triangle (top-left, top-right, bottom-left)
                    0, 3, 2  // Second triangle (bottom-left, top-right, bottom-right)
                };
            } else {
                triangles = new int[3]
                {
                    2, 1, 0, // First triangle (top-left, top-right, bottom-left)
                };
            }
        } else {
            if(polyData.vertices.Length == 4) {
                triangles = new int[6]
                {
                    0, 1, 2, // First triangle (top-left, top-right, bottom-left)
                    2, 3, 0  // Second triangle (bottom-left, top-right, bottom-right)
                };
            } else {
                triangles = new int[3]
                {
                    0, 1, 2, // First triangle (top-left, top-right, bottom-left)
                };
            }
        }

        return triangles;
    }

    private static Material GetMaterialForTexture(string texture) {
        string materialPath = TextureUtils.GetMaterialPath(texture);

        Material material = AssetDatabase.LoadAssetAtPath<Material>(materialPath);
        if(material == null) {
            material = AssetDatabase.LoadAssetAtPath<Material>("Assets/Prefab/Red.mat");
            Debug.LogError("Missing material for " + texture);
        }

        return material;
    }
}
#endif