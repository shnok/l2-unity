using UnityEngine;

[System.Serializable]
public class CameraCollisionDetection {
	public LayerMask collisionLayer = ~LayerMask.GetMask(new string[] {"Entity", "PlayerEntity"});
	private Camera _camera;
    private Transform _target;
    private Vector3 _offset;
    public float adjustedDistance;

    public CameraCollisionDetection(Camera camera, Transform target, Vector3 cameraOffset) {
        _camera = camera;
        _target = target;
        _offset = cameraOffset;
    }

	/* Calculate our near clip points */
	private Vector3[] GetCameraClipPoints(float distance) {
		Vector3[] cameraClipPoints = new Vector3[5];

        Quaternion camRot = _camera.transform.rotation;
        Vector3 camPos = _camera.transform.position;
        Vector3 desiredPos = camRot * (Vector3.forward * -distance) + _target.position + _offset;

		float collisionSize = 3f;
		float z = _camera.nearClipPlane;
		float x = Mathf.Tan (_camera.fieldOfView / collisionSize) * z;
		float y = x / _camera.aspect;

		//top left
		cameraClipPoints[0] = (camRot * new Vector3(-x, y, z)) + desiredPos;
		//top right
		cameraClipPoints[1] = (camRot * new Vector3(x, y, z)) + desiredPos;
		//bottom left
		cameraClipPoints[2] = (camRot * new Vector3(-x, -y, z)) + desiredPos;
		//bottom right
		cameraClipPoints[3] = (camRot * new Vector3(x, -y, z)) + desiredPos;
		//camera position
		cameraClipPoints[4] = desiredPos - (_camera.transform.forward * 0.25f);

        return cameraClipPoints;
	}

	/* Cast a ray from the target to each clip points */
	public void DetectCollision(float desiredDistance) {
        Vector3[] clipPoints = GetCameraClipPoints(desiredDistance);

        adjustedDistance = desiredDistance;
        float distance = -1f;

		for(int i = 0; i < clipPoints.Length; i++) {
			Ray ray = new Ray(_target.position, clipPoints[i] - (_target.position));
			float rayDistance = Vector3.Distance(clipPoints[i], _target.position);

            RaycastHit hit;
			if(Physics.Raycast(ray, out hit, rayDistance, collisionLayer)) {
                if(distance == -1f || hit.distance < distance) {
                    distance = hit.distance;
                }                
			}
		}

        if(distance != -1f) {
            adjustedDistance = distance; 
        }
	}

    public float GetCameraDistance() {
        return adjustedDistance;
    }
}
