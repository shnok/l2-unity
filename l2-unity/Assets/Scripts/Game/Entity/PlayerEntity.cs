using UnityEngine;

public class PlayerEntity : Entity
{
    private CharacterAnimationAudioHandler _characterAnimationAudioHandler;

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

        _characterAnimationAudioHandler = GetComponentInChildren<CharacterAnimationAudioHandler>();

        if (_instance == null)
        {
            _instance = this;
        }

        EquipAllArmors();

        EntityLoaded = true;
    }

    void OnDestroy()
    {
        _instance = null;
    }

    public void EquipAllArmors()
    {
        PlayerAppearance appearance = (PlayerAppearance)_appearance;
        if (appearance.Chest != 0)
        {
            ((PlayerGear)_gear).EquipArmor(appearance.Chest, ItemSlot.chest);
        }
        else
        {
            ((PlayerGear)_gear).EquipArmor(ItemTable.NAKED_CHEST, ItemSlot.chest);
        }

        if (appearance.Legs != 0)
        {
            ((PlayerGear)_gear).EquipArmor(appearance.Legs, ItemSlot.legs);
        }
        else
        {
            ((PlayerGear)_gear).EquipArmor(ItemTable.NAKED_LEGS, ItemSlot.legs);
        }

        if (appearance.Gloves != 0)
        {
            ((PlayerGear)_gear).EquipArmor(appearance.Gloves, ItemSlot.gloves);
        }
        else
        {
            ((PlayerGear)_gear).EquipArmor(ItemTable.NAKED_GLOVES, ItemSlot.gloves);
        }

        if (appearance.Feet != 0)
        {
            ((PlayerGear)_gear).EquipArmor(appearance.Feet, ItemSlot.feet);
        }
        else
        {
            ((PlayerGear)_gear).EquipArmor(ItemTable.NAKED_BOOTS, ItemSlot.feet);
        }
    }

    protected override void LookAtTarget() { }

    protected override void OnDeath()
    {
        base.OnDeath();
        Debug.Log("Player on death _networkAnimationReceive:" + _networkAnimationReceive);
        PlayerStateMachine.Instance.NotifyEvent(Event.DEAD);
    }

    protected override void OnHit(bool criticalHit)
    {
        base.OnHit(criticalHit);
        _characterAnimationAudioHandler.PlaySound(CharacterSoundEvent.Dmg);
    }

    public override bool StartAutoAttacking()
    {
        base.StartAutoAttacking();

        return true;
    }

    public override bool StopAutoAttacking()
    {
        base.StopAutoAttacking();

        return true;
    }

    public override float UpdateMAtkSpeed(int mAtkSpd)
    {
        float converted = base.UpdateMAtkSpeed(mAtkSpd);
        PlayerAnimationController.Instance.SetMAtkSpd(converted);

        return converted;
    }

    public override float UpdatePAtkSpeed(int pAtkSpd)
    {
        float converted = base.UpdatePAtkSpeed(pAtkSpd);
        PlayerAnimationController.Instance.SetPAtkSpd(converted);

        return converted;
    }

    public override float UpdateRunSpeed(int speed)
    {
        float converted = base.UpdateRunSpeed(speed);
        PlayerAnimationController.Instance.SetRunSpeed(converted);
        PlayerController.Instance.DefaultRunSpeed = converted;

        return converted;
    }

    public override float UpdateWalkSpeed(int speed)
    {
        float converted = base.UpdateWalkSpeed(speed);
        PlayerAnimationController.Instance.SetWalkSpeed(converted);
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