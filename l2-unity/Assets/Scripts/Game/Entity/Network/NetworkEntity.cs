using UnityEngine;

public class NetworkEntity : Entity
{
    public override void Initialize()
    {
        base.Initialize();
        if (_networkTransformReceive == null)
        {
            Debug.LogWarning($"[{transform.name}] NetworkTransformReceive was not assigned, please pre-assign animator to avoid unecessary load.");
            TryGetComponent(out _networkTransformReceive);
        }
        if (_networkCharacterControllerReceive == null)
        {
            Debug.LogWarning($"[{transform.name}] NetworkCharacterControllerReceive was not assigned, please pre-assign animator to avoid unecessary load.");
            TryGetComponent(out _networkCharacterControllerReceive);
        }
    }
    protected override void OnDeath()
    {
        if (_animationController != null)
        {
            _animationController.enabled = false;
        }
        if (_networkTransformReceive != null)
        {
            _networkTransformReceive.enabled = false;
        }
        if (_networkCharacterControllerReceive != null)
        {
            _networkCharacterControllerReceive.enabled = false;
        }
    }

    public override float UpdateMAtkSpeed(int mAtkSpd)
    {
        float converted = base.UpdateMAtkSpeed(mAtkSpd);
        _animationController.SetMAtkSpd(converted);

        return converted;
    }

    public override float UpdatePAtkSpeed(int pAtkSpd)
    {
        float converted = base.UpdatePAtkSpeed(pAtkSpd);
        _animationController.SetPAtkSpd(converted);

        return converted;
    }

    public override float UpdateRunSpeed(int speed)
    {
        float converted = base.UpdateRunSpeed(speed);
        _animationController.SetRunSpeed(converted);
        return converted;
    }
    public override float UpdateWalkSpeed(int speed)
    {
        float converted = base.UpdateWalkSpeed(speed);
        _animationController.SetWalkSpeed(converted);
        return converted;
    }
}