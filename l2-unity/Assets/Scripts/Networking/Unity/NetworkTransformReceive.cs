using System;
using UnityEngine;
using UnityEngine.AI;

public class NetworkTransformReceive : MonoBehaviour
{
    [SerializeField] private Vector3 _serverPosition;
    private Vector3 _lastPos;
    private float _newRotation;
    private float _posLerpValue;
    [SerializeField] private Transform _model;
    [SerializeField] private bool _static = false;
    [SerializeField] private bool _positionSyncProtection = true;
    [SerializeField] private bool _positionSynced = false;
    [SerializeField] private bool _positionSyncPaused = false;
    private float _lerpDuration = 0.1f;
    [SerializeField] private float _positionDelta;
    private float _finalLerpDelta = 0.05f;
    [SerializeField] private long _lastDesyncTime = 0;
    [SerializeField] private long _lastDesyncDuration = 0;
    private long _maximumAllowedDesyncTimeMs = 0;
    private float _lastRotationUpdateTime = 0;

    void Awake()
    {
        if (_model == null)
        {
            _model = transform.GetChild(0);
        }
    }

    protected virtual void Start()
    {
        _lastPos = transform.position;
        _newRotation = transform.eulerAngles.y;
        _serverPosition = transform.position;
        _positionSyncPaused = false;
    }

    void FixedUpdate()
    {
        if (_positionSyncProtection && !_positionSyncPaused && !_static)
        {
            UpdatePosition();
        }
        UpdateRotation();
    }

    /* Set new theorical position */
    public void SetNewPosition(Vector3 pos)
    {
        SetNewPosition(pos, true);
    }

    public virtual void SetNewPosition(Vector3 pos, bool calculateY)
    {
        // Debug.LogWarning($"[{transform.name}] SetNewPosition");


        if (calculateY)
        {
            /* adjust y to ground height */
            pos.y = World.Instance.GetGroundHeight(pos);
        }
        else
        {
            pos.y = transform.position.y;
        }

        _serverPosition = pos;
        /* reset states */
        _positionSynced = false; // Force sync with each new position
        _lastPos = transform.position;
        _posLerpValue = 0;
    }

    protected virtual float GetPositionSyncThreshold()
    {
        if (GameClient.Instance != null)
        {
            return GameClient.Instance.ServerEntityPositionSyncThreshold;
        }

        return 0.1f;
    }

    /* Safety measure to keep the transform position synced */
    private void UpdatePosition()
    {
        /* Check if client transform position is synced with server's */
        _positionDelta = VectorUtils.Distance2D(transform.position, _serverPosition);
        if (_positionDelta > GetPositionSyncThreshold() && _positionSynced)
        {
            _lastDesyncTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            _positionSynced = false;
        }

        if (_positionSynced)
        {
            OnPositionSynced();
        }
        else
        {
            OnPositionNotSynced();
        }
    }

    protected virtual void OnPositionSynced()
    {

    }

    protected virtual void OnPositionNotSynced()
    {
        _lastDesyncDuration = DateTimeOffset.Now.ToUnixTimeMilliseconds() - _lastDesyncTime;
        if (_lastDesyncDuration >= _maximumAllowedDesyncTimeMs)
        {

            if (_positionDelta < _finalLerpDelta)
            {
                _positionSynced = true;
                return;
            }

            Vector3 newPosition = Vector3.Lerp(_lastPos, _serverPosition, _posLerpValue);
            _posLerpValue += 1 / _lerpDuration * Time.deltaTime;
            newPosition.y = transform.position.y; // Keep the current y position
            transform.position = newPosition;
        }

    }

    /* Ajust rotation */
    protected virtual void UpdateRotation()
    {
        if (Time.time - _lastRotationUpdateTime > 2f)
        {
            return;
        }

        _model.rotation = Quaternion.Lerp(_model.rotation, Quaternion.Euler(Vector3.up * _newRotation), Time.deltaTime * 7.5f);
    }

    public virtual void SetFinalRotation(float finalRotation)
    {
        // Debug.Log($"[{transform.name}]Setting new final rotation: {finalRotation}");
        _newRotation = finalRotation;
        _lastRotationUpdateTime = Time.time;
    }

    public virtual void LookAt(Transform target)
    {
        // Debug.Log($"[{transform.name}] LookAt {target}");
        if (target != null)
        {
            SetFinalRotation(VectorUtils.CalculateMoveDirectionAngle(transform.position, target.position));
        }
    }

    public virtual void LookAt(Vector3 position)
    {
        SetFinalRotation(VectorUtils.CalculateMoveDirectionAngle(transform.position, position));
    }

    public bool IsPositionSynced()
    {
        return _positionSynced;
    }

    public void PausePositionSync()
    {
        _positionSyncPaused = true;
        _positionSynced = true;
    }

    public void ResumePositionSync()
    {
        _positionSyncPaused = false;
    }
}