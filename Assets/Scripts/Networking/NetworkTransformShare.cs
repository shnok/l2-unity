using UnityEngine;

public class NetworkTransformShare : MonoBehaviour {
    private Vector3 lastPos, lastRot;
    private float lastSharedPosTime;
    public Vector3 serverPosition;

    private void Awake() {
        if(World.GetInstance().offlineMode) {
            enabled = false;
        }
    }

    void Start() {
        lastPos = transform.position;
        lastRot = transform.forward;
        lastSharedPosTime = Time.time;
    }

    void Update() {
        SharePosition();
        ShareRotation();
    }

    /* SHARING */
    public void SharePosition() {
        if(Vector3.Distance(transform.position, lastPos) > .25f || Time.time - lastSharedPosTime >= 5f) {
            lastSharedPosTime = Time.time;
            ClientPacketHandler.GetInstance().UpdatePosition(transform.position);
            lastPos = transform.position;

            ClientPacketHandler.GetInstance().UpdateRotation(transform.eulerAngles.y);
        }
    }

    public void ShareRotation() {
        if(Vector3.Angle(lastRot, transform.forward) >= 10.0f) {
            lastRot = transform.forward;
            ClientPacketHandler.GetInstance().UpdateRotation(transform.eulerAngles.y);
        }
    }

    public void ShareAnimation(byte id, float value) {
        ClientPacketHandler.GetInstance().UpdateAnimation(id, value);
    } 
}