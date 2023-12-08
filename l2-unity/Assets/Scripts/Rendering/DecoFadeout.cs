using UnityEngine;

[ExecuteInEditMode]
public class Fadeout : MonoBehaviour {
    public float disableDistance = 100f;

    void FixedUpdate() {
        FadoutChilds();
    }

    void FadoutChilds() {
        Camera camera = Camera.main;
        if(Camera.current != null) {
            camera = Camera.current;
        }

        foreach(Transform child in transform) {
            if(isInRange(camera, child)) {
                child.gameObject.SetActive(true);
            } else {
                child.gameObject.SetActive(false);
            }
        }
    }

    bool isInRange(Camera camera, Transform target) {
        Vector3 flatTargetPos = new Vector3(target.position.x, 0, target.position.z);
        Vector3 flatCameraPos = new Vector3(camera.transform.position.x, 0, camera.transform.position.z);
        float distance = Vector3.Distance(flatTargetPos, flatCameraPos);

        return distance < disableDistance;
    }
}
