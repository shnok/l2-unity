using System.Collections.Generic;
using Animancer;
using UnityEngine;

// Used by MONSTERS
public class NewMonsterAnimationController : NewBaseAnimationController
{
    [Header("Monster")]
    [SerializeField] protected L2MonsterAnimationContainer _animContainer; //TODO: Cache in Singleton?
    public MonsterAnimationEvent LastAnim { get { return (MonsterAnimationEvent)_lastAnim; } }
    private MonsterAudioHandler AudioHandler { get => (MonsterAudioHandler)_entityReferenceHolder.AudioHandler; }

    public override void Initialize()
    {
        base.Initialize();

        if (_animContainer == null)
        {
            Debug.LogWarning($"[{transform.name}] L2Animations was not assigned, please pre-assign it.");
        }

        Wait();
    }

    protected override AnimationClip GetAnimationClip(AnimationCategory _, int index)
    {
        if (_animContainer == null || index >= _animContainer.Animations.Length)
        {
            return null;
        }

        return _animContainer.Animations[index].AnimationClip;
    }

    public override void SetRunSpeed(float value)
    {
        _runSpdMultiplier = value;
    }

    public override void SetWalkSpeed(float value)
    {
        _walkSpdMultiplier = value;
    }
    public override void Jump() { }
    public override void Attack()
    {
        if (PlayAnimation((int)MonsterAnimationEvent.atk01))
        {
            _animancerState.EffectiveSpeed = _atkSpdMultiplier;

            if (!_animancerState.HasEvents)
            {
                _animancerState.Events.Add(AudioHandler.AtkRatio, () => AudioHandler.PlayAtkSound());

                if (_entityReferenceHolder.Gear.WeaponType == WeaponType.bow)
                {
                    _animancerState.Events.Add(_nockArrowRatio, () =>
                    {
                        _entityReferenceHolder.Combat.NockArrow();
                        AudioHandler.PlayBowBendSound();
                    });

                    _animancerState.Events.Add(_shootArrowRatio, () =>
                    {
                        _entityReferenceHolder.Combat.ShootArrow();
                        AudioHandler.PlayArrowShootSound();
                    });
                }

                _animancerState.Events.OnEnd += Wait;
            }
        }
    }

    public override void Run()
    {
        if (PlayAnimation((int)MonsterAnimationEvent.run))
        {
            _animancerState.EffectiveSpeed = _runSpdMultiplier;

            if (!_animancerState.HasEvents)
            {
                _animancerState.Events.Add(0.5f, () => AudioHandler.PlayBreatheSound());

                foreach (float ratio in AudioHandler.RunStepRatios)
                {
                    _animancerState.Events.Add(ratio, () => AudioHandler.PlaySound(EntitySoundEvent.Step));
                }
            }
        }
    }

    public override void Wait()
    {
        if (AtkWait())
        {
            return;
        }

        if (PlayAnimation((int)MonsterAnimationEvent.wait))
        {
            if (!_animancerState.HasEvents)
            {
                _animancerState.Events.Add(0.5f, () => AudioHandler.PlayWaitSound()); // breathe?
            }
        }
    }

    public override bool AtkWait()
    {
        if (!base.AtkWait())
        {
            return false;
        }

        // play any sound ?

        return PlayAnimation((int)MonsterAnimationEvent.atkwait);
    }

    public override void Walk()
    {
        if (PlayAnimation((int)MonsterAnimationEvent.walk))
        {
            _animancerState.EffectiveSpeed = _runSpdMultiplier;

            if (!_animancerState.HasEvents)
            {
                foreach (float ratio in AudioHandler.WalkStepRatios)
                {
                    _animancerState.Events.Add(ratio, () => AudioHandler.PlaySound(EntitySoundEvent.Step));
                }
            }
        }
    }

    public override void Sit() { }

    public override void Die()
    {
        if (PlayAnimation((int)MonsterAnimationEvent.death))
        {
            if (!_animancerState.HasEvents)
            {
                _animancerState.Events.Add(AudioHandler.DeathRatio, () => AudioHandler.PlaySound(EntitySoundEvent.Death));
                _animancerState.Events.Add(AudioHandler.FallRatio, () => AudioHandler.PlaySound(EntitySoundEvent.Fall));
                _animancerState.Events.OnEnd = DieWait;
            }
        }
    }

    public override void DieWait()
    {
        PlayAnimation((int)MonsterAnimationEvent.deathwait);
    }

    public override void SitWait() { }

    public override void Stand() { }

    public override void Resurrect()
    {
    }

    public override void FightStanceStarted()
    {
        if (_lastAnim == (int)MonsterAnimationEvent.wait)
        {
            AtkWait();
        }
    }

    public override void FightStanceStopped()
    {
        if (_lastAnim == (int)MonsterAnimationEvent.atkwait)
        {
            Wait();
        }
    }

    public override void Emote(int action)
    {
        Debug.Log($"[{transform.name}] Social Action: {action}");
        PlayAnimation((int)MonsterAnimationEvent.spwait);
        if (!_animancerState.HasEvents)
        {
            _animancerState.Events.OnEnd = Wait;
        }
    }
}
