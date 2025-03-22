// Interface for all animation types
using System;
using UnityEngine;

public interface IL2HumanoidAnimation<TEvent> where TEvent : Enum
{
    AnimationClip AnimationClip { get; }
    TEvent Event { get; }
    void SetEvent(TEvent value);
}