using UnityEngine;
using System.Collections.Generic;

public class CameraController : MonoBehaviour
{
	public Transform target;
	private float x, y = 0;
	private float _minDistance = 1;
	private float _maxDistance = 10f;
	public Vector3 camOffset = new Vector3();
	public float camDistance = 5f;
	public float currentDistance = 0;
	public float camSpeed = 25f;

	public Vector3 targetPos;
	public CameraCollisionDetection detector;
	public static CameraController _instance;

    public static CameraController GetInstance() {
        return _instance;
    }

    void Awake() {
        if (_instance == null) {
            _instance = this;
        }
    }

	void Update() {
		if(target != null && detector != null) {
			detector.DetectCollision(camDistance);
			UpdatePosition();
		}
	}

	public void SetTarget(GameObject go) {
		target = go.transform;
		detector = new CameraCollisionDetection(GetComponent<Camera>(), target, camOffset);
	}

	public bool IsObjectVisible(GameObject target) {
		if(Vector3.Angle(target.transform.position - transform.position, transform.forward) <= Camera.main.fieldOfView) {
            RaycastHit hit;
			if(Physics.Linecast(transform.position, target.transform.position + Vector3.up, out hit, ~LayerMask.GetMask(new string[] { "Entity", "PlayerEntity" }))) {
				return false;
			}
		}

		Vector3 viewPort = Camera.main.WorldToViewportPoint(target.transform.position);

		bool insideView = viewPort.x <= 1 && viewPort.x >= 0 && viewPort.y <= 1 && viewPort.y >= 0 && viewPort.z >= -0.2f;
		return insideView;
	}

	public void UpdateInputs(float xi, float yi) {
		x += xi * camSpeed * 0.1f;
		y -= yi * camSpeed * 0.1f;
		y = ClampAngle(y, -90, 90);
	}

	public void UpdateZoom(float axis) {
		camDistance = Mathf.Clamp(camDistance - axis *5, _minDistance, _maxDistance);	
	}

    void FixedUpdate() {
			
    }

	private void UpdatePosition() {
		Quaternion rotation = Quaternion.Euler(y, x, 0);
		transform.rotation = rotation;

		float adjustedDistance = detector.GetCameraDistance();

		if(adjustedDistance > Vector3.Distance(targetPos + camOffset, transform.position)) {
			currentDistance += ((adjustedDistance - currentDistance) / 0.2f) * Time.deltaTime;
		} else {
			currentDistance -= ((currentDistance - adjustedDistance) / 0.075f) * Time.deltaTime;
		}

		targetPos = target.position + camOffset;
		Vector3 adjustedPosition = rotation * (Vector3.forward * -currentDistance) + targetPos;

		transform.position = adjustedPosition;
	}

	private float ClampAngle(float angle, float min, float max) {
		if (angle < -360F)
			angle += 360F;
		if (angle > 360F)
			angle -= 360F;
		return Mathf.Clamp(angle, min, max);
	}
}
