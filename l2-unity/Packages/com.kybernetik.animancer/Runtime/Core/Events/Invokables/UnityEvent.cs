// Animancer // https://kybernetik.com.au/animancer // Copyright 2018-2025 Kybernetik //

using System;

namespace Animancer
{
    /// <summary>A <see cref="UnityEngine.Events.UnityEvent"/> which implements <see cref="IInvokable"/>.</summary>
    /// https://kybernetik.com.au/animancer/api/Animancer/UnityEvent
    [Serializable]
    public class UnityEvent : UnityEngine.Events.UnityEvent, IInvokable { }
}

