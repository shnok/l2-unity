using UnityEngine;

public class CameraController : MonoBehaviour {
    public Transform target;
    public Transform rootBone;
    public float rootBoneHeight = 0;
    public bool followRootBoneOffset = true;
    private float x, y = 0;
    public float minDistance = .5f;
    public float maxDistance = 10f;
    public Vector3 camOffset = new Vector3();
    public float camDistance = 5f;
    public float currentDistance = 0;
    public float camSpeed = 25f;
    public float zoomSpeed = 5f;

    public Vector3 targetPos;
    public CameraCollisionDetection detector;

    public static CameraController _instance;

    public static CameraController GetInstance() {
        return _instance;
    }

    void Awake() {
        if(_instance == null) {
            _instance = this;
        }
    }

    void Update() {
        if(target != null && detector != null) {
            detector.DetectCollision(camDistance);
            UpdatePosition();
            UpdateInputs();
            UpdateZoom();
        }
    }

    public void SetTarget(GameObject go) {
        target = go.transform;
        rootBone = target.transform.FindRecursive(child => child.tag == "Root");
        rootBoneHeight = rootBone.position.y - target.position.y;
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

    private void UpdateInputs() {
        if(InputManager.GetInstance().IsInputPressed(InputType.TurnCamera)) {
            Vector2 mouseAxis = InputManager.GetInstance().mouseAxis;
            x += mouseAxis.x * camSpeed * 0.1f;
            y -= mouseAxis.y * camSpeed * 0.1f;
            y = ClampAngle(y, -90, 90);
        }
    }

    private void UpdateZoom() {
        if(InputManager.GetInstance().IsInputPressed(InputType.Zoom)) {
            float scrollAxis = InputManager.GetInstance().scrollAxis;
            camDistance = Mathf.Clamp(camDistance - scrollAxis * zoomSpeed, minDistance, maxDistance);
        }
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

        float boneOffset = 0;
        if(followRootBoneOffset) {
            boneOffset = rootBone.position.y - target.position.y - rootBoneHeight;
        }
        targetPos = new Vector3(camOffset.x, boneOffset + camOffset.y, camOffset.z) + target.position;
        Vector3 adjustedPosition = rotation * (Vector3.forward * -currentDistance) + targetPos;

        transform.position = adjustedPosition;
    }

    private float ClampAngle(float angle, float min, float max) {
        if(angle < -360F)
            angle += 360F;
        if(angle > 360F)
            angle -= 360F;
        return Mathf.Clamp(angle, min, max);
    }
}
