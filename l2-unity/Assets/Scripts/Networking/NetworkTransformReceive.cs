using System;
using System.Collections;
using UnityEngine;

public class NetworkTransformReceive : MonoBehaviour {
    [SerializeField] private Vector3 _serverPosition;
    private Vector3 _lastPos;
    private float _newRotation;
    private float _posLerpValue;
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
        if(World.Instance.OfflineMode) {
            this.enabled = false;
            return;
        }

        _lastPos = transform.position;
        _newRotation = transform.eulerAngles.y;
        _serverPosition = transform.position;
        _positionSyncPaused = false;
    }

    void FixedUpdate() {
        if(_positionSyncProtection && !_positionSyncPaused) {
            UpdatePosition();
        }
        UpdateRotation();
    }

    /* Set new theorical position */
    public void SetNewPosition(Vector3 pos) {
        /* adjust y to ground height */
        pos.y = World.Instance.GetGroundHeight(pos);

        _serverPosition = pos;

        /* reset states */
        _lastPos = transform.position;
        _posLerpValue = 0;
    }

    /* Safety measure to keep the transform position synced */

    public void UpdatePosition() {
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
    private void UpdateRotation() {
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(Vector3.up * _newRotation), Time.deltaTime * 7.5f);
    }

    public void SetFinalRotation(float finalRotation) {
        _newRotation = finalRotation;
    }

    public void LookAt(Transform target) {
        if (target != null) {
            _newRotation = CalculateAngleToLookAt(target.position);
        }
    }

    public void LookAt(Vector3 position) {
        _newRotation = CalculateAngleToLookAt(position);
    }

    private float CalculateAngleToLookAt(Vector3 position) { 
        float angle = Mathf.Atan2(position.x - transform.position.x, position.z - transform.position.z) * Mathf.Rad2Deg;
        angle = Mathf.Round(angle / 45f);
        angle *= 45f;
        return angle;
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