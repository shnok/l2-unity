using System.Collections.Generic;
using UnityEngine;

// Used by MONSTERS
public class MonsterAnimationController : BaseAnimationController
{
    [SerializeField] protected MonsterAnimationEvent _lastAnim;
    public MonsterAnimationEvent LastAnim { get { return _lastAnim; } }
    private bool _lastValue = false;

    public override void Initialize()
    {
        base.Initialize();
    }

    public void SetBool(MonsterAnimationEvent animationEvent, bool value)
    {
        if (animationEvent == _lastAnim && value == _lastValue)
        {
            return;
        }

        _lastValue = value;
        _lastAnim = animationEvent;

        // Debug.LogWarning($"SetBool: {animationEvent}={value}");

        base.SetBool(GetParameterId(animationEvent), value);
    }

    public bool GetBool(MonsterAnimationEvent animationEvent)
    {
        return base.GetBool(GetParameterId(animationEvent));
    }

    protected int GetParameterId(MonsterAnimationEvent animationEvent)
    {
        // Debug.LogWarning("GetParameterId: " + animationEvent);
        return AnimatorParameterHashTable.GetMonsterParameterHash((int)animationEvent);
    }

    public override void SetMAtkSpd(float value)
    {
        //TODO: update for cast animation
        float newMAtkSpd = _spAtk01ClipLength / value;
        Animator.SetFloat(GetParameterId(MonsterAnimationEvent.matkspd), newMAtkSpd);
    }

    public override void SetRunSpeed(float value)
    {
        Animator.SetFloat(GetParameterId(MonsterAnimationEvent.speed), value);
    }

    public override void SetWalkSpeed(float value)
    {
        Animator.SetFloat(GetParameterId(MonsterAnimationEvent.speed), value);
    }

    public override void UpdateAnimatorAtkSpdMultiplier(float clipLength)
    {
        float newAtkSpd = clipLength * 1000f / _pAtkSpd;
        Animator.SetFloat(GetParameterId(MonsterAnimationEvent.patkspd), newAtkSpd);
    }
}
