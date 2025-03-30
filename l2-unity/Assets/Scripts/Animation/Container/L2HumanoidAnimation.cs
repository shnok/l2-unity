using System;
using UnityEngine;

// [Serializable]
// public struct L2HumanoidAnimation
// {
//     [SerializeField] private HumanoidAnimationEvent _event;
//     [SerializeField] private AnimationClip _clip;

//     public AnimationClip AnimationClip { get => _clip; }
//     public HumanoidAnimationEvent Event { get => _event; set => _event = value; }
// }

[Serializable]
public struct L2HumanoidAnimation<TEvent> : IL2HumanoidAnimation<TEvent> where TEvent : Enum
{
    [SerializeField] private TEvent _event;
    [SerializeField] private AnimationClip _clip;

    public AnimationClip AnimationClip { get => _clip; }
    public TEvent Event { get => _event; }
    public void SetEvent(TEvent value) => _event = value;
}