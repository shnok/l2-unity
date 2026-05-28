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
        if (_animations != null && _animations.Length > 0 && _animations.Length == Enum.GetValues(typeof(TEvent)).Length)
        {
            return;
        }

        int startIndex = 0;
        if (_animations == null || _animations.Length == 0)
        {
            _animations = new TAnimation[Enum.GetValues(typeof(TEvent)).Length];
        }
        else
        {
            startIndex = _animations.Length - 1;

            Array.Resize(ref _animations, Enum.GetValues(typeof(TEvent)).Length);
        }

        for (int i = startIndex; i < _animations.Length; i++)
        {
            _animations[i] = new TAnimation();
            _animations[i].SetEvent((TEvent)Enum.ToObject(typeof(TEvent), i));
        }
    }
}