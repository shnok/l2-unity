using UnityEngine;

public class NetworkTransformShare : MonoBehaviour {
    private Vector3 _lastPos, _lastRot;
    private float _lastSharedPosTime;
    public Vector3 _serverPosition;

    void Start() {
        _lastPos = transform.position;
        _lastRot = transform.forward;
        _lastSharedPosTime = Time.time;
    }

    void Update() {
        SharePosition();
        ShareRotation();
    }

    /* SHARING */
    public void SharePosition() {
        if(Vector3.Distance(transform.position, _lastPos) > .25f || Time.time - _lastSharedPosTime >= 5f) {
            _lastSharedPosTime = Time.time;
            //ClientPacketHandler.GetInstance().UpdatePosition(transform.position);
            _lastPos = transform.position;

           // ClientPacketHandler.GetInstance().UpdateRotation(transform.eulerAngles.y);
        }
    }

    public void ShareRotation() {
        if(Vector3.Angle(_lastRot, transform.forward) >= 10.0f) {
            _lastRot = transform.forward;
         //   ClientPacketHandler.GetInstance().UpdateRotation(transform.eulerAngles.y);
        }
    }

    public void ShareAnimation(byte id, float value) {
      //  ClientPacketHandler.GetInstance().UpdateAnimation(id, value);
    } 
}