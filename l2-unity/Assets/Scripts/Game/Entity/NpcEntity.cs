using UnityEngine;

[RequireComponent(typeof(NetworkAnimationController)),
    RequireComponent(typeof(NetworkTransformReceive)),
    RequireComponent(typeof(NetworkCharacterControllerReceive)),
    RequireComponent(typeof(CharacterAnimationAudioHandler))]
public class NpcEntity : NetworkEntity
{
    private CharacterAnimationAudioHandler _npcAnimationAudioHandler;

    [SerializeField] private NpcData _npcData;

    public NpcData NpcData { get { return _npcData; } set { _npcData = value; } }

    public override void Initialize()
    {
        base.Initialize();
        _npcAnimationAudioHandler = GetComponent<CharacterAnimationAudioHandler>();

        EntityLoaded = true;
    }

    protected override void OnDeath()
    {
        base.OnDeath();
        _networkAnimationReceive.SetAnimationProperty((int)NpcAnimationEvent.Death, 1f, true);
    }

    protected override void OnHit(bool criticalHit)
    {
        base.OnHit(criticalHit);
        _npcAnimationAudioHandler.PlaySound(CharacterSoundEvent.Dmg);
    }

    public override void OnStopMoving()
    {
        if (_networkAnimationReceive.GetAnimationProperty((int)NpcAnimationEvent.Atk01) == 0f)
        {
            _networkAnimationReceive.SetAnimationProperty((int)NpcAnimationEvent.Wait, 1f);
        }
    }

    public override void OnStartMoving(bool walking)
    {
        base.OnStartMoving(walking);
        _networkAnimationReceive.SetAnimationProperty(walking ? (int)NpcAnimationEvent.Walk : (int)NpcAnimationEvent.Run, 1f);
    }

    public override bool StartAutoAttacking()
    {
        if (base.StartAutoAttacking())
        {
            _networkAnimationReceive.SetAnimationProperty((int)NpcAnimationEvent.Atk01, 1f);
        }

        if (networkCharacterController != null)
        {
            // Should stop moving if autoattacking
            networkCharacterController.SetDestination(transform.position);
        }

        return true;
    }

    public override bool StopAutoAttacking()
    {
        if (base.StopAutoAttacking())
        {
            if (!IsDead())
            {
                _networkAnimationReceive.SetAnimationProperty((int)NpcAnimationEvent.AtkWait, 1f);
            }
        }

        return true;
    }
}
