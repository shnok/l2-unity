using UnityEngine;

[System.Serializable]
public class CameraCollisionDetection {
    [SerializeField] private LayerMask _collisionLayer;
    [SerializeField] private float _adjustedDistance;
    [SerializeField] private Transform _collisionObject;
    [SerializeField] private Vector2 _clipPlaneOffset = new Vector2(2f, 1f);
    [SerializeField] private bool _debug = false;
    private Camera _camera;
    private Transform _target;
    private Vector3 _offset;

    public float AdjustedDistance { get { return _adjustedDistance; } }

    public CameraCollisionDetection(Camera camera, Transform target, Vector3 cameraOffset, LayerMask collisionmask) {
        this._camera = camera;
        this._target = target;
        _offset = cameraOffset;
        _collisionLayer = collisionmask;
    }

    /* Calculate our near clip points */
    public Vector3[] GetCameraClipPoints(float distance) {
        Vector3[] cameraClipPoints = new Vector3[5];
        Quaternion camRot = _camera.transform.rotation;
        Vector3 desiredPos = camRot * (Vector3.forward * -distance) + _target.position + _offset;

        float z = _camera.nearClipPlane;
        float x = Mathf.Tan(_camera.fieldOfView / _clipPlaneOffset.x) * z;
        float y = x / _camera.aspect / _clipPlaneOffset.y;

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

    public Vector3[] GetCameraViewPortPoints() {
        Vector3[] cameraClipPoints = new Vector3[5];
        Quaternion camRot = _camera.transform.rotation;
        Vector3 camPos = _camera.transform.position;

        float z = _camera.nearClipPlane;
        float x = Mathf.Tan(_camera.fieldOfView) * z;
        float y = x / _camera.aspect / _clipPlaneOffset.y;

        //top left
        cameraClipPoints[0] = (camRot * new Vector3(-x, y, z)) + camPos;
        //top right
        cameraClipPoints[1] = (camRot * new Vector3(x, y, z)) + camPos;
        //bottom left
        cameraClipPoints[2] = (camRot * new Vector3(-x, -y, z)) + camPos;
        //bottom right
        cameraClipPoints[3] = (camRot * new Vector3(x, -y, z)) + camPos;
        //camera position
        cameraClipPoints[4] = camPos - (_camera.transform.forward * 0.25f);

        return cameraClipPoints;
    }

    /* Cast a ray from the target to each clip points */
    public void DetectCollision(float desiredDistance) {
        if(_camera == null) {
            return;
        }

        Vector3[] clipPoints = GetCameraClipPoints(desiredDistance);
        Vector3[] viewPoints = GetCameraViewPortPoints();

        _adjustedDistance = desiredDistance;
        float distance = -1f;

        Transform hitObject = null;
        RaycastHit hit;
        for(int i = 0; i < clipPoints.Length; i++) {
            if(Physics.Linecast(_target.position + _offset, clipPoints[i], out hit, _collisionLayer)) {
                if(distance == -1f || hit.distance < distance) {
                    distance = hit.distance;
                }
                hitObject = hit.transform;
            }

            if(_debug) {
                Debug.DrawLine(clipPoints[i], _target.position, Color.green);
                Debug.DrawLine(viewPoints[i], _target.position, Color.yellow);

            }
        }

        if(distance != -1f) {
            _adjustedDistance = distance;
        }

        _collisionObject = hitObject;
    }
}
