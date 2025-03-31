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

    public override void Attack()
    {
        if (PlayAnimation((int)MonsterAnimationEvent.atk01))
        {
            _animancerState.EffectiveSpeed = _atkSpdMultiplier;

            if (_animancerState.Events(null, out AnimancerEvent.Sequence events))
            {
                events.Add(AudioHandler.AtkRatio, () => AudioHandler.PlaySound(EntitySoundEvent.Atk));

                //TODO: Detect if monster has a bow ?
            }
        }
    }

    public override void Run()
    {
        if (PlayAnimation((int)MonsterAnimationEvent.run))
        {
            _animancerState.EffectiveSpeed = _runSpdMultiplier;

            if (_animancerState.Events(null, out AnimancerEvent.Sequence events))
            {
                foreach (float ratio in AudioHandler.RunStepRatios)
                {
                    events.Add(0.5f, () => AudioHandler.PlayBreatheSound());
                    events.Add(ratio, () => AudioHandler.PlaySound(EntitySoundEvent.Step));
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
            if (_animancerState.Events(null, out AnimancerEvent.Sequence events))
            {
                events.Add(0.5f, () => AudioHandler.PlayWaitSound()); // breathe?
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

            if (_animancerState.Events(null, out AnimancerEvent.Sequence events))
            {
                foreach (float ratio in AudioHandler.WalkStepRatios)
                {
                    events.Add(ratio, () => AudioHandler.PlaySound(EntitySoundEvent.Step));
                }
            }
        }
    }

    public override void Sit() { }

    public override void Die()
    {
        if (PlayAnimation((int)MonsterAnimationEvent.death))
        {
            if (_animancerState.Events(null, out AnimancerEvent.Sequence events))
            {
                events.Add(AudioHandler.DeathRatio, () => AudioHandler.PlaySound(EntitySoundEvent.Death));
                events.Add(AudioHandler.FallRatio, () => AudioHandler.PlaySound(EntitySoundEvent.Fall));
                events.OnEnd = DieWait;
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
        if (_animancerState.Events(null, out AnimancerEvent.Sequence events))
        {
            events.OnEnd = Wait;
        }
    }
}
