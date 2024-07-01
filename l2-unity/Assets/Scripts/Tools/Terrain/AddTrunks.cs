#if (UNITY_EDITOR)
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class AddTrunks
{

    [MenuItem("Shnok/[Debug] Add trunks to trees")]
    private static void AddTrunksToTrees()
    {
        string treeName = "speaking_tree_s";
        GameObject[] foundObjects = GameObject.FindObjectsByType<GameObject>(FindObjectsSortMode.None);
        foreach(var gameObj in foundObjects) {
            if (!gameObj.activeSelf)
                continue;

            if(gameObj.name.Contains(treeName)) {
                gameObj.GetComponent<MeshCollider>().enabled = false;

                GameObject cylinder = GameObject.CreatePrimitive(PrimitiveType.Cylinder);

                float cylinderHeight = 1.5f;
                // Set the cylinder's position
                cylinder.transform.position = gameObj.transform.position + Vector3.up * cylinderHeight / 2f;

                // Adjust the cylinder's scale to set its size
                cylinder.transform.localScale = new Vector3(1.25f, cylinderHeight, 1.25f);

                cylinder.AddComponent<MeshCollider>();
                cylinder.GetComponent<MeshRenderer>().enabled = false;

                cylinder.transform.parent = gameObj.transform;
                cylinder.transform.name = "trunk";
                  
            }
        }
    }

}
#endif
