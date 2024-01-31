using UnityEngine;

public class NetworkTransformShare : MonoBehaviour {
    private Vector3 _lastPos, _lastRot;
    private float _lastSharedPosTime;
    [SerializeField] public Vector3 _serverPosition;

    void Start() {
        if(World.GetInstance().offlineMode) {
            enabled = false;
            return;
        }

        _lastPos = transform.position;
        _lastRot = transform.forward;
        _lastSharedPosTime = Time.time;
    }

    void Update() {
        SharePosition();
        ShareRotation();
    }

    public void SharePosition() {
        if(Vector3.Distance(transform.position, _lastPos) > .25f || Time.time - _lastSharedPosTime >= 10f) {
            _lastSharedPosTime = Time.time;
            ClientPacketHandler.Instance.UpdatePosition(transform.position);
            _lastPos = transform.position;

            ClientPacketHandler.Instance.UpdateRotation(transform.eulerAngles.y);
        }
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