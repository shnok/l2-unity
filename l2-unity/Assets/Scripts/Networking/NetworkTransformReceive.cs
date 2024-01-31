using System;
using System.Collections;
using UnityEngine;

public class NetworkTransformReceive : MonoBehaviour {
    [SerializeField] private Vector3 _serverPosition;
    private Vector3 _lastPos;
    private Quaternion _lastRot, _newRot;
    private float _rotLerpValue, _posLerpValue;
    [SerializeField] private bool _positionSyncProtection = true;
    [SerializeField] private float _positionSyncNodesThreshold = 2f;
    [SerializeField] private bool _positionSynced = false;
    [SerializeField] private bool _positionSyncPaused = false;
    [SerializeField] private float _lerpDuration = 0.3f;
    [SerializeField] private float _positionDelta;
    [SerializeField] private long _lastDesyncTime = 0;
    [SerializeField] private long _lastDesyncDuration = 0;
    [SerializeField] private long _maximumAllowedDesyncTimeMs = 300;

    void Start() {
        if(World.GetInstance().offlineMode) {
            this.enabled = false;
            return;
        }

        _lastPos = transform.position;
        _newRot = transform.rotation;
        _lastRot = transform.rotation;
        _serverPosition = transform.position;
        _positionSyncPaused = false;
    }

    void FixedUpdate() {
        if(_positionSyncProtection && !_positionSyncPaused) {
            AdjustPosition();
        }
        LerpToRotation();
    }

    /* Set new theorical position */
    public void SetNewPosition(Vector3 pos) {
        /* adjust y to ground height */
        pos.y = World.GetInstance().GetGroundHeight(pos);

        _serverPosition = pos;

        /* reset states */
        _lastPos = transform.position;
        _posLerpValue = 0;
    }

    /* Safety measure to keep the transform position synced */

    public void AdjustPosition() {
        /* Check if client transform position is synced with server's */
        _positionDelta = Vector3.Distance(transform.position, _serverPosition);
        if(_positionDelta > Geodata.Instance.NodeSize * _positionSyncNodesThreshold) {
            _lastDesyncTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            _positionSynced = false;
        }

        if(!_positionSynced) {

            _lastDesyncDuration = DateTimeOffset.Now.ToUnixTimeMilliseconds() - _lastDesyncTime;
            if(_lastDesyncDuration > _maximumAllowedDesyncTimeMs) {
                if(_positionDelta <= Geodata.Instance.NodeSize / 10f) {
                    _positionSynced = true;
                    return;
                }

                transform.position = Vector3.Lerp(_lastPos, _serverPosition, _posLerpValue);
                _posLerpValue += (1 / _lerpDuration) * Time.deltaTime;
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
        if(Mathf.Abs(transform.rotation.eulerAngles.y - _newRot.eulerAngles.y) > 2f) {
            transform.rotation = Quaternion.Lerp(_lastRot, _newRot, _rotLerpValue);
            _rotLerpValue += Time.deltaTime * 7.5f;
        }
    }

    public void RotateTo(float angle) {
        Quaternion rot = transform.rotation;
        Vector3 eulerAngles = transform.rotation.eulerAngles;
        eulerAngles.y = angle;
        rot.eulerAngles = eulerAngles;

        _newRot = rot;
        _lastRot = transform.rotation;
        _rotLerpValue = 0;
    }

    public bool IsPositionSynced() {
        return _positionSynced;
    }

    public void PausePositionSync() {
        _positionSyncPaused = true;
        _positionSynced = true;
    }

    public void ResumePositionSync() {
        _positionSyncPaused = false;
    }
}