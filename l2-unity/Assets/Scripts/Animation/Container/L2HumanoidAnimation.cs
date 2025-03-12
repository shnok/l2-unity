using System;
using UnityEngine;

[Serializable]
public class L2HumanoidAnimation
{
    [SerializeField] private HumanoidAnimationEvent _event;
    [SerializeField] private AnimationClip _clip;

    public AnimationClip AnimationClip { get => _clip; }

}