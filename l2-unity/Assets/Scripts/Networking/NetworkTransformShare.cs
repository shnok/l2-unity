using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class NetworkTransformShare : MonoBehaviour {
    private CharacterController _characterController;
    private Vector3 _lastPos, _lastRot;
    private float _lastSharedPosTime;

    [SerializeField] public Vector3 _serverPosition;
    [SerializeField] public bool _shouldShareRotation;

    public bool ShouldShareRotation { get { return _shouldShareRotation; } set { _shouldShareRotation = value; } }

    void Start() {
        if(World.Instance.OfflineMode) {
            enabled = false;
            return;
        }

        _characterController = GetComponent<CharacterController>();
        _lastPos = transform.position;
        _lastRot = transform.forward;
        _lastSharedPosTime = Time.time;
    }

    void Update() {
        if (ShouldSharePosition()) {
            SharePosition();
        }

        if(ShouldShareRotation) {
            ShareRotation();
        }
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
        if (Vector3.Angle(_lastRot, transform.forward) >= 10.0f) {
            _lastRot = transform.forward;
            ClientPacketHandler.Instance.UpdateRotation(transform.eulerAngles.y);
        }
    }

    public void ShareAnimation(byte id, float value) {
        ClientPacketHandler.Instance.UpdateAnimation(id, value);
    } 
}