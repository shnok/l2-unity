using UnityEngine;

[System.Serializable]
public class CameraCollisionDetection {
    public LayerMask collisionLayer;
    private Camera camera;
    private Transform target;
    private Vector3 offset;
    public float adjustedDistance;
    public Transform collisionObject;

    public CameraCollisionDetection(Camera camera, Transform target, Vector3 cameraOffset, LayerMask collisionmask) {
        this.camera = camera;
        this.target = target;
        offset = cameraOffset;
        collisionLayer = collisionmask;
    }

    /* Calculate our near clip points */
    public Vector3[] GetCameraClipPoints(float distance) {
        Vector3[] cameraClipPoints = new Vector3[5];

        Quaternion camRot = camera.transform.rotation;
        Vector3 camPos = camera.transform.position;
        Vector3 desiredPos = camRot * (Vector3.forward * -distance) + target.position + offset;

        float collisionSize = 3f;
        float z = camera.nearClipPlane;
        float x = Mathf.Tan(camera.fieldOfView / collisionSize) * z;
        float y = x / camera.aspect;

        //top left
        cameraClipPoints[0] = (camRot * new Vector3(-x, y, z)) + desiredPos;
        //top right
        cameraClipPoints[1] = (camRot * new Vector3(x, y, z)) + desiredPos;
        //bottom left
        cameraClipPoints[2] = (camRot * new Vector3(-x, -y, z)) + desiredPos;
        //bottom right
        cameraClipPoints[3] = (camRot * new Vector3(x, -y, z)) + desiredPos;
        //camera position
        cameraClipPoints[4] = desiredPos - (camera.transform.forward * 0.25f);

        return cameraClipPoints;
    }

    /* Cast a ray from the target to each clip points */
    public void DetectCollision(float desiredDistance) {
        if(camera == null) {
            return;
        }

        Vector3[] clipPoints = GetCameraClipPoints(desiredDistance);

        adjustedDistance = desiredDistance;
        float distance = -1f;

        Transform hitObject = null;
        for(int i = 0; i < clipPoints.Length; i++) {
            Ray ray = new Ray(target.position, clipPoints[i] - (target.position));
            float rayDistance = Vector3.Distance(clipPoints[i], target.position);

            RaycastHit hit;
            if(Physics.Raycast(ray, out hit, rayDistance, collisionLayer)) {
                if(distance == -1f || hit.distance < distance) {
                    distance = hit.distance;
                }
                hitObject = hit.transform;
            } 
        }

        if(distance != -1f) {
            adjustedDistance = distance;
        }

        collisionObject = hitObject;
    }

    public float GetCameraDistance() {
        return adjustedDistance;
    }
}
