using System;
using UnityEngine;

public class PlayerTransformReceive : NetworkTransformReceive
{
    private static PlayerTransformReceive _instance;
    public static PlayerTransformReceive Instance { get { return _instance; } }

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else if (_instance != this)
        {
            Destroy(this);
        }
    }

    protected override void Start()
    {
        base.Start();
        PausePositionSync();
    }

    protected override float GetPositionSyncThreshold()
    {
        return GameClient.Instance.PlayerPositionSyncThreshold;
    }

    /* Set new theorical position */
    public override void SetNewPosition(Vector3 pos, bool calculateY)
    {
        // Debug.LogWarning("Adjust player position error");
        base.SetNewPosition(pos, calculateY);
        ResumePositionSync();
    }

    protected override void OnPositionSynced()
    {
        PausePositionSync();
    }

    /* Ajust rotation */
    protected override void UpdateRotation()
    {
    }

    public override void SetFinalRotation(float finalRotation)
    {
    }

    public override void LookAt(Transform target)
    {
    }

    public override void LookAt(Vector3 position)
    {
    }
}