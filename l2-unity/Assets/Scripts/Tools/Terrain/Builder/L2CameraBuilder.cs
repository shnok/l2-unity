#if (UNITY_EDITOR) 
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using UnityEngine.ProBuilder;

public class L2CameraBuilder {

    [MenuItem("Shnok/8. [Camera] (T3D) Build cameras")]
    static void ImportBrushTexturesT3D() {
        string title = "Select T3D file";
        string directory = Path.Combine(Application.dataPath, "Data/Maps");
        string extension = "t3d";

        string fileToProcess = EditorUtility.OpenFilePanel(title, directory, extension);

        if (!string.IsNullOrEmpty(fileToProcess)) {
            Debug.Log("Selected file: " + fileToProcess);
            List<L2InterpolationPoint> interpolationPoints = L2T3DInfoParser.ParseCameraInfo(fileToProcess);

            GameObject container = new GameObject("Cameras");

            foreach (L2InterpolationPoint point in interpolationPoints) {
                GameObject go = new GameObject(point.name);
                go.transform.position = VectorUtils.ConvertPosToUnity(point.position);
                go.transform.eulerAngles = VectorUtils.ConvertRotToUnity(point.eulerAngles);
                Camera cam = go.AddComponent<Camera>();
                cam.usePhysicalProperties = true;
                cam.fieldOfView = 60;
                cam.allowHDR = false;
                cam.allowMSAA = false;

                go.transform.parent = container.transform;
            }

            SortChildrenByNameTag(container);
        }
    }

    public static void SortChildrenByNameTag(GameObject obj) {
        List<Transform> children = new List<Transform>();
        for (int i = obj.transform.childCount - 1; i >= 0; i--) {
            Transform child = obj.transform.GetChild(i);

            children.Add(child);
            child.transform.SetParent(null, false);
        }

        children.Sort((Transform t1, Transform t2) => { return t1.name.CompareTo(t2.name); });

        foreach (Transform child in children) {
            child.transform.SetParent(obj.transform, false);
        }
    }

}
#endif