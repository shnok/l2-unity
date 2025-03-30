// Animancer // https://kybernetik.com.au/animancer // Copyright 2018-2025 Kybernetik //

#if ULT_EVENTS

using System;

namespace Animancer
{
    /// <summary>An <see cref="UltEvents.UltEvent"/> which implements <see cref="IInvokable"/>.</summary>
    /// https://kybernetik.com.au/animancer/api/Animancer/UltEvent
    [Serializable]
    public class UltEvent : UltEvents.UltEvent, IInvokable { }
}

#endif

