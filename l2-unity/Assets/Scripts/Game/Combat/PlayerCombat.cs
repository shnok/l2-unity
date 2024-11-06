using System;
using UnityEngine;

// Used by LOCAL PLAYER
[System.Serializable]
public class PlayerCombat : Combat
{
    public bool IsForcedAction { get; set; }

    private static PlayerCombat _instance;
    public static PlayerCombat Instance { get => _instance; }

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

    public override void OnDeath()
    {
        base.OnDeath();
        PlayerStateMachine.Instance.NotifyEvent(Event.DEAD);
    }

    public override void OnRevive()
    {
        base.OnRevive();
        PlayerStateMachine.Instance.NotifyEvent(Event.REVIVED);
    }

    protected override void OnHit(Hit hit)
    {
        base.OnHit(hit);
    }

    public override bool AttackOnce()
    {
        if (base.AttackOnce())
        {
            Debug.LogWarning("Attack Once");
            PlayerStateMachine.Instance.OnAttackAllowed();
            return true;
        }
        else
        {
            return false;
        }
    }
}
