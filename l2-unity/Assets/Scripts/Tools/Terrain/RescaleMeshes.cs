using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class RescaleMeshes : MonoBehaviour
{
    [MenuItem("Shnok/[Debug] Rescale Meshes")]
    static void GenerateTerrain() {
        Transform t = GameObject.Find("StaticMeshes").transform;

        for (int i = 0; i < t.childCount; i++) {
            Transform child = t.GetChild(i);
            if(child.localScale.x < 0.1) {
                child.localScale = new Vector3(child.localScale.x * 52.5f, child.localScale.y * 52.5f, child.localScale.z * 52.5f);
            }
        }

    }

    [MenuItem("Shnok/[Debug] Rescale Trunks")]
    static void RescaleTrunks() {
        Transform t = GameObject.Find("StaticMeshes").transform;

        GameObject[] foundObjects = GameObject.FindObjectsByType<GameObject>(FindObjectsSortMode.None);
        foreach (var gameObj in foundObjects) {
            if (!gameObj.activeSelf)
                continue;

            if (gameObj.name == "trunk") {
               
                gameObj.transform.localScale = new Vector3(gameObj.transform.localScale.x / 52.5f, gameObj.transform.localScale.y / 52.5f, gameObj.transform.localScale.z / 52.5f);
                gameObj.transform.localPosition = new Vector3(gameObj.transform.localPosition.x / 52.5f, gameObj.transform.localPosition.y / 52.5f, gameObj.transform.localPosition.z / 52.5f);
            }
        }

    }
}
