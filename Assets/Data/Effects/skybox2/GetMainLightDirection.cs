using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class GetMainLightDirection : MonoBehaviour
{
    [SerializeField] public Material skyboxMaterial;

    // Update is called once per frame
    void Update()
    {
        skyboxMaterial.SetVector("_MainLightForward", transform.forward);
        skyboxMaterial.SetVector("_MainLightUp", transform.up);
        skyboxMaterial.SetVector("_MainLightRight", transform.right);
    }
}
