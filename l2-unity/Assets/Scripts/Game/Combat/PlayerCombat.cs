using System;
using UnityEngine;

[System.Serializable]
public class PlayerCombat : Combat
{
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

    protected override void OnDeath()
    {
        base.OnDeath();
        PlayerStateMachine.Instance.NotifyEvent(Event.DEAD);
    }

    protected override void OnHit(bool criticalHit)
    {
        base.OnHit(criticalHit);
        AudioHandler.PlaySound(EntitySoundEvent.Dmg);
    }
}
