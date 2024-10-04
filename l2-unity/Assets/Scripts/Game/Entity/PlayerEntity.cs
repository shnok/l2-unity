using UnityEngine;

// Used by LOCAL PLAYER
public class PlayerEntity : Entity
{
    private static PlayerEntity _instance;
    public static PlayerEntity Instance { get => _instance; }

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    public override void Initialize()
    {
        base.Initialize();

        EntityLoaded = true;
    }

    void OnDestroy()
    {
        _instance = null;
    }

    protected override void LookAtTarget() { }

    public override float UpdateRunSpeed(int speed)
    {
        float converted = base.UpdateRunSpeed(speed);
        PlayerController.Instance.DefaultRunSpeed = converted;

        return converted;
    }

    public override float UpdateWalkSpeed(int speed)
    {
        float converted = base.UpdateWalkSpeed(speed);
        PlayerController.Instance.DefaultWalkSpeed = converted;

        return converted;
    }

    public void OnActionFailed(PlayerAction action)
    {
        switch (action)
        {
            case PlayerAction.SetTarget:
                TargetManager.Instance.ClearTarget();
                break;
            case PlayerAction.AutoAttack:
                PlayerStateMachine.Instance.OnActionDenied();
                break;
            case PlayerAction.Move:
                PlayerStateMachine.Instance.OnActionDenied();
                break;
        }
    }

    public void OnActionAllowed(PlayerAction action)
    {
        Debug.LogWarning("Action allowed: " + action);
        switch (action)
        {
            case PlayerAction.SetTarget:
                break;
            case PlayerAction.AutoAttack:
                break;
            case PlayerAction.Move:
                PlayerStateMachine.Instance.OnActionAllowed();
                break;
        }
    }

    public override void UpdateWaitType(ChangeWaitTypePacket.WaitType moveType)
    {
        base.UpdateWaitType(moveType);

        PlayerStateMachine.Instance.OnActionAllowed();
    }

    public override void UpdateMoveType(bool running)
    {
        base.UpdateMoveType(running);

        if (PlayerController.Instance != null)
        {
            PlayerController.Instance.Running = running;
        }

        if (PlayerStateMachine.Instance != null)
        {
            PlayerStateMachine.Instance.NotifyEvent(Event.MOVE_TYPE_UPDATED);
        }

        if (CharacterInfoWindow.Instance != null)
        {
            CharacterInfoWindow.Instance.UpdateValues();
        }
    }
}