using UnityEngine;

public class CameraController : MonoBehaviour {
    private Vector3 _lerpTargetPos;
    private float _x, _y = 0;
    private Vector3 _targetPos;
    private LayerMask _collisionMask;

    [SerializeField] private Transform _target;

    [Header("Camera controls")]
    [SerializeField] private bool _smoothCamera = true;
    [SerializeField] private Vector3 _camOffset = new Vector3(0, 0.8f, 0);
    [SerializeField] private float _smoothness = 8f;
    [SerializeField] private float _camSpeed = 25f;

    [Header("Zoom controls")]
    [SerializeField] private float _minDistance = .5f;
    [SerializeField] private float _maxDistance = 10f;
    [SerializeField] private float _zoomSpeed = 5f;
    [SerializeField] private float _camDistance = 2f;
    [SerializeField] private float _currentDistance = 0;

    [Header("Bone stickiness")]
    [SerializeField] private bool _stickToBone = true;
    [SerializeField] private Transform _rootBone;
    [SerializeField] private float _rootBoneHeight = 0;

    [SerializeField] private CameraCollisionDetection _collisionDetector;

    public Transform Target { get { return _target; } set { _target = value; } }

    public bool StickToBone { get { return _stickToBone; } set { _stickToBone = value; } }


    private static CameraController _instance;
    public static CameraController Instance { get { return _instance; } }

    private void Awake() {
        if(_instance == null) {
            _instance = this;
        }
    }

    private void Start() {
        _lerpTargetPos = Vector3.zero;
    }

    public void SetMask(LayerMask collisionMask) {
        this._collisionMask = collisionMask;
    }

    private void Update() {
        if(_target != null && _collisionDetector != null) {
            UpdateInputs();
        }
    }

    void FixedUpdate() {
        if(_target != null && _collisionDetector != null) {
            _collisionDetector.DetectCollision(_camDistance);
            UpdatePosition();
            
            UpdateZoom();
        }
    }

    public void SetTarget(GameObject go) {
        _target = go.transform;
        transform.position = _targetPos;

        _rootBone = _target.transform.FindRecursive(child => child.tag == "Root");
        _rootBoneHeight = _rootBone.position.y - _target.position.y;
        _collisionDetector = new CameraCollisionDetection(GetComponent<Camera>(), _target, _camOffset, _collisionMask);
    }

    public bool IsObjectVisible(Transform target) {
        RaycastHit hit;
        Vector3[] cameraClips = _collisionDetector.GetCameraViewPortPoints();
        bool visible = false;
        for(int i = 0; i < cameraClips.Length; i++) {
            if(!Physics.Linecast(cameraClips[i], target.position + 0.5f * Vector3.up, out hit, _collisionMask)) {
                visible = true;
                break;
            }
        }

        if(visible == false) {
            return false;
        }

        Vector3 viewPort = Camera.main.WorldToViewportPoint(target.position);
        bool insideView = viewPort.x <= 1 && viewPort.x >= 0 && viewPort.y <= 1 && viewPort.y >= 0 && viewPort.z >= -0.2f;
        return insideView;
    }

    private void UpdateInputs() {
        if(InputManager.Instance.IsInputPressed(InputType.TurnCamera)) {
            Vector2 mouseAxis = InputManager.Instance.mouseAxis;
            _x += mouseAxis.x * _camSpeed * 0.1f;
            _y -= mouseAxis.y * _camSpeed * 0.1f;
            _y = ClampAngle(_y, -90, 90);
        }
    }

    private void UpdateZoom() {
        if(InputManager.Instance.IsInputPressed(InputType.Zoom)) {
            float scrollAxis = InputManager.Instance.scrollAxis;
            _camDistance = Mathf.Clamp(_camDistance - scrollAxis * _zoomSpeed, _minDistance, _maxDistance);
        }
    }

    private void UpdatePosition() {
        Quaternion rotation = Quaternion.Euler(_y, _x, 0);
        transform.rotation = rotation;

        if(_collisionDetector.AdjustedDistance > Vector3.Distance(_targetPos + _camOffset, transform.position)) {
            _currentDistance += ((_collisionDetector.AdjustedDistance - _currentDistance) / 0.2f) * Time.deltaTime;
        } else {
            _currentDistance -= ((_currentDistance - _collisionDetector.AdjustedDistance) / 0.075f) * Time.deltaTime;
        }

        float boneOffset = 0;
        if(_stickToBone) {
            boneOffset = _rootBone.position.y - _target.position.y - _rootBoneHeight;
        }

        _targetPos = new Vector3(_camOffset.x, boneOffset + _camOffset.y, _camOffset.z) + _target.position;

        if(_smoothCamera) {
            if(_lerpTargetPos == Vector3.zero) {
                _lerpTargetPos = _targetPos;
            }
            _lerpTargetPos = Vector3.Lerp(_lerpTargetPos, _targetPos, _smoothness * Time.deltaTime);
        } else {
            _lerpTargetPos = _targetPos;
        }

        Vector3 adjustedPosition = rotation * (Vector3.forward * -_currentDistance) + _lerpTargetPos;

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
