using System;
using System.Collections;
using UnityEngine;

public class NetworkTransformReceive : MonoBehaviour {
    [SerializeField] private Vector3 serverPosition;
    private Vector3 lastPos;
    private Quaternion lastRot, newRot;
    private float rotLerpValue, posLerpValue;
    [SerializeField] private bool positionSyncProtection = true;
    [SerializeField] private float positionSyncNodesThreshold = 2f;
    [SerializeField] private bool positionSynced = false;
    [SerializeField] private bool positionSyncPaused = false;
    [SerializeField] private float lerpDuration = 0.3f;
    [SerializeField] private float positionDelta;
    [SerializeField] private long lastDesyncTime = 0;
    [SerializeField] private long lastDesyncDuration = 0;
    [SerializeField] private long maximumAllowedDesyncTimeMs = 300;

    void Start() {
        if(World.GetInstance().offlineMode) {
            this.enabled = false;
            return;
        }

        lastPos = transform.position;
        newRot = transform.rotation;
        lastRot = transform.rotation;
        serverPosition = transform.position;
        positionSyncPaused = false;
    }

    void FixedUpdate() {
        if(positionSyncProtection && !positionSyncPaused) {
            AdjustPosition();
        }
        LerpToRotation();
    }

    /* Set new theorical position */
    public void SetNewPosition(Vector3 pos) {
        /* adjust y to ground height */
        pos.y = World.GetInstance().GetGroundHeight(pos);

        serverPosition = pos;

        /* reset states */
        lastPos = transform.position;
        posLerpValue = 0;
    }

    /* Safety measure to keep the transform position synced */

    public void AdjustPosition() {
        /* Check if client transform position is synced with server's */
        positionDelta = Vector3.Distance(transform.position, serverPosition);
        if(positionDelta > Geodata.GetInstance().nodeSize * positionSyncNodesThreshold) {
            lastDesyncTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            positionSynced = false;
        }

        if(!positionSynced) {

            lastDesyncDuration = DateTimeOffset.Now.ToUnixTimeMilliseconds() - lastDesyncTime;
            if(lastDesyncDuration > maximumAllowedDesyncTimeMs) {
                if(positionDelta <= Geodata.GetInstance().nodeSize / 10f) {
                    positionSynced = true;
                    return;
                }

                transform.position = Vector3.Lerp(lastPos, serverPosition, posLerpValue);
                posLerpValue += (1 / lerpDuration) * Time.deltaTime;
            }

        }   
    }

    /* Ajust rotation */
    public void LookAt(Vector3 dest) {
        var heading = dest - transform.position;
        float angle = Vector3.Angle(heading, Vector3.forward);
        Vector3 cross = Vector3.Cross(heading, Vector3.forward);
        if(cross.y >= 0) angle = -angle;
        RotateTo(angle);
    }

    public void LerpToRotation() {
        if(Mathf.Abs(transform.rotation.eulerAngles.y - newRot.eulerAngles.y) > 2f) {
            transform.rotation = Quaternion.Lerp(lastRot, newRot, rotLerpValue);
            rotLerpValue += Time.deltaTime * 7.5f;
        }
    }

    public void RotateTo(float angle) {
        Quaternion rot = transform.rotation;
        Vector3 eulerAngles = transform.rotation.eulerAngles;
        eulerAngles.y = angle;
        rot.eulerAngles = eulerAngles;

        newRot = rot;
        lastRot = transform.rotation;
        rotLerpValue = 0;
    }

    public bool IsPositionSynced() {
        return positionSynced;
    }

    public void PausePositionSync() {
        positionSyncPaused = true;
        positionSynced = true;
    }

    public void ResumePositionSync() {
        positionSyncPaused = false;
    }
}