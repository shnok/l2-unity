using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace AdvancedTerrainGrass
{
    public class SwitchCameras : MonoBehaviour
    {
        public GameObject firstCam;
        public GameObject secondCam;
        public GrassManager GrassManager;

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
            //  Switch cameras.
                firstCam.SetActive(!firstCam.activeInHierarchy);
                secondCam.SetActive(!secondCam.activeInHierarchy);
            //  Force the GrassManager to pick up the new camera.
                GrassManager.Cam = null;
            //  Reset HiZ.
                GrassManager.HiZBuffer = null;
            //  Call BurstInit to init all new visible cells at once.
                GrassManager.BurstInit();
            }
        }
    }
}
