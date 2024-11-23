using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveWorld : MonoBehaviour {
    public Camera cam;
    public float threshold = 100.0f;


    private Transform camtrans;
    private Transform thistrans;

    private Vector3 thispos;
	// Use this for initialization
	void Start () {
        camtrans = cam.transform;
        thistrans = this.transform;
    }
	
	// Update is called once per frame
	void Update () {

    //  Spupport for changing cameras at runtime.
        if (!camtrans.gameObject.activeInHierarchy)
        {
            var t_camtrans = Camera.main.transform;
            if (t_camtrans != null)
            {
                camtrans = t_camtrans;
            }
            else
            {
                Debug.Log("No main camera found.");
                return;
            }
        }

        var campos = camtrans.position;
        thispos = thistrans.position;

        var cameraPosition = campos;
        cameraPosition.y = 0f;
        if (cameraPosition.magnitude > threshold)
        {
            thistrans.position = thispos - cameraPosition;
        //  Not needed if the camera is child of the world object.
        //  camtrans.position = campos - cameraPosition;
        }
        

       /*     
                    if (campos.x > 100) {
                        var diff = campos.x;
                        thispos.x -= diff;
                        thistrans.position = thispos;

                        campos.x -= diff;
                        camtrans.position = campos;
                    }
                    else if (campos.x < -100)
                    {
                        var diff = campos.x;
                        thispos.x -= diff;
                        thistrans.position = thispos;

                        campos.x -= diff;
                        camtrans.position = campos;
                    }
                    if (campos.z > 100)
                    {
                        var diff = campos.z;
                        thispos.z -= diff;
                        thistrans.position = thispos;

                        campos.z -= diff;
                        camtrans.position = campos;
                    }
                    else if (campos.z < -100)
                    {
                        var diff = campos.z;
                        thispos.z -= diff;
                        thistrans.position = thispos;

                        campos.z -= diff;
                        camtrans.position = campos;
                    }
            */

        }
}
