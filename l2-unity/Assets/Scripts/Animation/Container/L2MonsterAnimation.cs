using System;
using UnityEngine;

[Serializable]
public class L2MonsterAnimation
{
    [SerializeField] private MonsterAnimationEvent _event;
    [SerializeField] private AnimationClip _clip;

    public AnimationClip AnimationClip { get => _clip; }
}