using UnityEngine;

public class NetworkTransformShare : MonoBehaviour {
    private Vector3 _lastPos, _lastRot;
    private float _lastSharedPosTime;
    [SerializeField] public Vector3 _serverPosition;

    void Start() {
        if(World.Instance.OfflineMode) {
            enabled = false;
            return;
        }

        _lastPos = transform.position;
        _lastRot = transform.forward;
        _lastSharedPosTime = Time.time;
    }

    void Update() {
        if (ShouldSharePosition()) {
            SharePosition();
        }
        ShareRotation();
    }

    // Share position every 0.25f and based on delay
    public bool ShouldSharePosition() {
        if (Vector3.Distance(transform.position, _lastPos) > .25f || Time.time - _lastSharedPosTime >= 10f) {
            return true;
        }

        return false;
    }

    public void SharePosition() {
        ClientPacketHandler.Instance.UpdatePosition(transform.position);
        _lastSharedPosTime = Time.time;
        _lastPos = transform.position;

        ClientPacketHandler.Instance.UpdateRotation(transform.eulerAngles.y);
    }

    public void ShareRotation() {
        if(Vector3.Angle(_lastRot, transform.forward) >= 10.0f) {
            _lastRot = transform.forward;
            ClientPacketHandler.Instance.UpdateRotation(transform.eulerAngles.y);
        }
    }

    public void ShareAnimation(byte id, float value) {
        ClientPacketHandler.Instance.UpdateAnimation(id, value);
    } 
}