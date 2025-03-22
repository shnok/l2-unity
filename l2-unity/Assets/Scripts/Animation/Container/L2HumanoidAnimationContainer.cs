using System;
using UnityEngine;

public abstract class L2HumanoidAnimationContainer<TEvent, TAnimation> : ScriptableObject
    where TEvent : Enum
    where TAnimation : IL2HumanoidAnimation<TEvent>, new()
{
    [SerializeField]
    private TAnimation[] _animations;

    public TAnimation[] Animations { get => _animations; }

    protected virtual void Awake()
    {
        _animations = new TAnimation[Enum.GetValues(typeof(TEvent)).Length];
        for (int i = 0; i < _animations.Length; i++)
        {
            _animations[i] = new TAnimation();
            _animations[i].SetEvent((TEvent)Enum.ToObject(typeof(TEvent), i));
        }
    }
}