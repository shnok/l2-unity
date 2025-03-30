// Animancer // https://kybernetik.com.au/animancer // Copyright 2018-2025 Kybernetik //

using UnityEngine.Playables;

namespace Animancer
{
    /// <summary>A utility for re-assigning Animancer's <see cref="PlayableOutput"/>.</summary>
    /// 
    /// <remarks>
    /// This should be totally useless, but for some reason it seems to fix an issue with Unity's
    /// Animation Rigging package. Normally, all of the Rig's parameters get reset to their
    /// starting values any time a playable is connected or disconnected (which Animancer does frequently),
    /// but using this utility effectively re-captures the starting values
    /// so any subsequent resets retain the values you set.
    /// <para></para>
    /// <strong>Example:</strong>
    /// <para></para><code>
    /// public class PlayableOutputRefresherExample : MonoBehaviour
    /// {
    ///     [SerializeField] private AnimancerComponent _Animancer;
    ///     [SerializeField] private Rig _Rig;
    /// 
    ///     // A field to store it in.
    ///     private PlayableOutputRefresher _OutputRefresher;
    /// 
    ///     protected virtual void OnEnable()
    ///     {
    ///         // Initialize on startup.
    ///         _OutputRefresher = new(_Animancer);
    ///     }
    /// 
    ///     public void SetWeight(float weight)
    ///     {
    ///         // Change something that would be reset.
    ///         _Rig.weight = weight;
    ///         
    ///         // Then call this afterwards.
    ///         _OutputRefresher.Refresh();
    ///     }
    /// }
    /// </code></remarks>
    /// 
    /// https://kybernetik.com.au/animancer/api/Animancer/PlayableOutputRefresher
    /// 
    public struct PlayableOutputRefresher
    {
        /************************************************************************************************************************/

        /// <summary>The <see cref="PlayableOutput"/> of Animancer's <see cref="PlayableGraph"/>.</summary>
        public PlayableOutput Output { get; set; }

        /// <summary>The root <see cref="Playable"/> of Animancer's <see cref="PlayableGraph"/>.</summary>
        public Playable Root { get; set; }

        /************************************************************************************************************************/

        /// <summary>Creates a new <see cref="PlayableOutputRefresher"/>.</summary>
        public PlayableOutputRefresher(PlayableOutput output)
        {
            Output = output;
            Root = Output.GetSourcePlayable();
        }

        /************************************************************************************************************************/

        /// <summary>Creates a new <see cref="PlayableOutputRefresher"/>.</summary>
        public PlayableOutputRefresher(AnimancerGraph animancer)
            : this(animancer.Output)
        { }

        /************************************************************************************************************************/

        /// <summary>Re-assigns the <see cref="Root"/> as the source playable of the <see cref="Output"/>.</summary>
        public readonly void Refresh()
            => Output.SetSourcePlayable(Root);

        /************************************************************************************************************************/

        /// <summary>Re-acquires the <see cref="Root"/> from the <see cref="Output"/>.</summary>
        /// <remarks>Call this after <see cref="AnimancerGraph.InsertOutputPlayable"/>.</remarks>
        public void OnSourcePlayableChanged()
            => Root = Output.GetSourcePlayable();

        /************************************************************************************************************************/
    }
}

