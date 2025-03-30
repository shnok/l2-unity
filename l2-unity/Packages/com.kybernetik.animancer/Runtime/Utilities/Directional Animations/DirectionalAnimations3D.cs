// Animancer // https://kybernetik.com.au/animancer // Copyright 2018-2025 Kybernetik //

using UnityEngine;

namespace Animancer
{
    /// <summary>A <see cref="DirectionalAnimations3D{T}"/> using <see cref="int"/> as the group type.</summary>
    /// 
    /// <remarks>
    /// <strong>Sample:</strong>
    /// <see href="https://kybernetik.com.au/animancer/docs/samples/sprites/character-3d">
    /// Directional Character 3D</see>
    /// </remarks>
    /// 
    /// https://kybernetik.com.au/animancer/api/Animancer/DirectionalAnimations3D
    /// 
    [AddComponentMenu(Strings.MenuPrefix + "Directional Animations 3D")]
    [AnimancerHelpUrl(typeof(DirectionalAnimations3D))]
    public class DirectionalAnimations3D : DirectionalAnimations3D<int> { }

    /************************************************************************************************************************/

    /// <summary>
    /// A component which manages a screen-facing billboard and plays animations from a
    /// <see cref="DirectionalSet4{T}"/> to make it look like a <see cref="Sprite"/>
    /// based character is facing a particular direction in 3D space.
    /// </summary>
    /// 
    /// <remarks>
    /// <strong>Sample:</strong>
    /// <see href="https://kybernetik.com.au/animancer/docs/samples/sprites/character-3d">
    /// Directional Character 3D</see>
    /// </remarks>
    /// 
    /// https://kybernetik.com.au/animancer/api/Animancer/DirectionalAnimations3D_1
    /// 
    [AnimancerHelpUrl(typeof(DirectionalAnimations3D<>))]
    public class DirectionalAnimations3D<TGroup> : MonoBehaviour
    {
        /************************************************************************************************************************/
        #region Fields and Properties
        /************************************************************************************************************************/

        [SerializeField]
        [Tooltip("The object to rotate according to the " + nameof(Mode))]
        private Transform _Transform;

        /// <summary>[<see cref="SerializeField"/>]
        /// The object to rotate according to the <see cref="Mode"/>.
        /// </summary>
        /// <remarks>Uses this <see cref="Component.transform"/> by default.</remarks>
        public ref Transform Transform
            => ref _Transform;

        /************************************************************************************************************************/

        [SerializeField]
        [Tooltip("The " + nameof(UnityEngine.Camera) + " to make the " + nameof(Transform) + " face towards" +
            "\n\nLeave this null to automatically use the Main Camera")]
        private Transform _Camera;

        /// <summary>[<see cref="SerializeField"/>]
        /// The <see cref="UnityEngine.Camera"/> to make the <see cref="Transform"/> face towards.
        /// </summary>
        /// <remarks>
        /// Leave this <c>null</c> to automatically use the <see cref="Camera.main"/>.
        /// </remarks>
        public Transform Camera
        {
            get
            {
                if (_Camera == null)
                {
                    var camera = UnityEngine.Camera.main;
                    if (camera != null)
                        _Camera = camera.transform;
                }

                return _Camera;
            }
            set => _Camera = value;
        }

        /************************************************************************************************************************/

        [SerializeField]
        [Tooltip("The " + nameof(AnimancerComponent) + " to play animations on")]
        private AnimancerComponent _Animancer;

        /// <summary>[<see cref="SerializeField"/>]
        /// The <see cref="AnimancerComponent"/> to play animations on.
        /// </summary>
        public ref AnimancerComponent Animancer
            => ref _Animancer;

        /************************************************************************************************************************/

        [SerializeField]
        [Tooltip("The " + nameof(DirectionalAnimationSet4) + " or " + nameof(DirectionalAnimationSet8) +
            " to play animations from (Forwards in 3D space corresponds to the Up animation)")]
        private DirectionalSet<AnimationClip> _Animations;

        /// <summary>[<see cref="SerializeField"/>]
        /// The animations to choose between based on the <see cref="Forward"/> direction.
        /// </summary>
        /// <remarks>Forwards in 3D space corresponds to the Up animation.</remarks>
        public ref DirectionalSet<AnimationClip> Animations
            => ref _Animations;

        /************************************************************************************************************************/

        [SerializeField]
        [Tooltip("The World-Space direction this character is facing used to select which animation to play")]
        private Vector3 _Forward = Vector3.forward;

        /// <summary>[<see cref="SerializeField"/>]
        /// The World-Space direction this character is facing used to select which animation to play.
        /// </summary>
        public Vector3 Forward
        {
            get => _Forward;
            set
            {
                _Forward = value;
                if (!enabled)
                    PlayCurrentAnimation(TimeSynchronizer.CurrentGroup);
            }
        }

        /************************************************************************************************************************/

        /// <summary>Functions used to face the <see cref="Transform"/> towards the <see cref="Camera"/>.</summary>
        public enum BillboardMode
        {
            /// <summary>Don't control the <see cref="Transform"/>.</summary>
            None,

            /// <summary>Copy the <see cref="Camera"/> 's rotation.</summary>
            MatchRotation,

            /// <summary>Face the <see cref="Camera"/>'s position.</summary>
            FacePosition,

            /// <summary>As <see cref="MatchRotation"/>, but only rotate around the Y axis.</summary>
            UprightMatchRotation,

            /// <summary>As <see cref="FacePosition"/>, but only rotate around the Y axis.</summary>
            UprightFacePosition,

            /// <summary>
            /// As <see cref="UprightMatchRotation"/>,
            /// and also scale on the Y axis to maintain the same screen size
            /// regardless of the <see cref="Camera"/>'s Euler X Angle.</summary>
            /// <remarks>Only use this mode with an Orthographic Camera</remarks>
            UprightMatchRotationStretched,

            /// <summary>
            /// As <see cref="UprightFacePosition"/>,
            /// and also scale on the Y axis to maintain the same screen size
            /// regardless of the <see cref="Camera"/>'s Euler X Angle.</summary>
            /// <remarks>Only use this mode with an Orthographic Camera</remarks>
            UprightFacePositionStretched,
        }

        [SerializeField]
        [Tooltip("The function used to face the " + nameof(Transform) + " towards the " + nameof(Camera) + ":" +
            "\n• None - Don't control the " + nameof(Transform) +
            "\n• Match Rotation - Copy the " + nameof(Camera) + "'s rotation" +
            "\n• Face Position - Face the " + nameof(Camera) + "'s position" +
            "\n• Upright - As above, but only rotate around the Y axis" +
            "\n• Stretched - As above, and also scale on the Y axis to maintain the same screen size" +
            " regardless of the " + nameof(Camera) + "'s Euler X Angle (only use with an Orthographic Camera)")]
        private BillboardMode _Mode = BillboardMode.UprightMatchRotation;

        /// <summary>[<see cref="SerializeField"/>]
        /// The function used to face the <see cref="Transform"/> towards the <see cref="Camera"/>.
        /// </summary>
        public BillboardMode Mode
        {
            get => _Mode;
            set
            {
                _Mode = value;
                ResetScaleIfNotStretched();
            }
        }

        /************************************************************************************************************************/

        /// <summary>
        /// Maintains the <see cref="AnimancerState.NormalizedTime"/> when swapping between animations.
        /// </summary>
        public readonly TimeSynchronizer<TGroup>
            TimeSynchronizer = new(default, true);

        /************************************************************************************************************************/
        #endregion
        /************************************************************************************************************************/
        #region Methods
        /************************************************************************************************************************/

        /// <summary>
        /// Finds missing references,
        /// samples the current animation,
        /// and resets the scale to 1 if not using a stretched mode.
        /// </summary>
        protected virtual void OnValidate()
        {
            gameObject.GetComponentInParentOrChildren(ref _Transform);
            gameObject.GetComponentInParentOrChildren(ref _Animancer);

            if (TryGetCurrentAnimation(out var animation))
                AnimancerUtilities.EditModeSampleAnimation(animation, _Animancer);

            ResetScaleIfNotStretched();
        }

        /************************************************************************************************************************/

        /// <summary>
        /// Finds missing references,
        /// samples the current animation,
        /// and resets the scale to 1 if not using a stretched mode.
        /// </summary>
        protected virtual void OnDrawGizmosSelected()
        {
            if (TryGetCurrentAnimation(out var animation))
                AnimancerUtilities.EditModeSampleAnimation(animation, _Animancer);

            if (_Transform == null)
                return;

            var position = _Transform.position;
            var length = 1f;

            var renderer = GetComponentInChildren<Renderer>();
            if (renderer != null)
            {
                var bounds = renderer.bounds;
                position.y += bounds.extents.y;
                length = bounds.extents.magnitude;
            }

            Gizmos.color = new(0.75f, 0.75f, 1, 1);
            Gizmos.DrawRay(position, Forward.normalized * length);
        }

        /************************************************************************************************************************/

        /// <summary>
        /// Applies the <see cref="Mode"/> then plays the appropriate animation
        /// based on the current rotation and <see cref="Forward"/> direction.
        /// </summary>
        protected virtual void Update()
        {
            UpdateTransform();
            PlayCurrentAnimation(TimeSynchronizer.CurrentGroup);
        }

        /************************************************************************************************************************/

        /// <summary>Applies the <see cref="Mode"/>.</summary>
        public void UpdateTransform()
        {
            switch (_Mode)
            {
                default:
                case BillboardMode.None:
                    break;

                case BillboardMode.MatchRotation:
                    _Transform.rotation = Camera.rotation;
                    break;

                case BillboardMode.FacePosition:
                    _Transform.rotation = Quaternion.LookRotation(_Transform.position - Camera.position);
                    break;

                case BillboardMode.UprightMatchRotation:
                    _Transform.eulerAngles = new(0, Camera.eulerAngles.y, 0);
                    break;

                case BillboardMode.UprightFacePosition:
                    var direction = _Transform.position - Camera.position;
                    _Transform.eulerAngles = new(
                        0,
                        Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg,
                        0);
                    break;

                case BillboardMode.UprightMatchRotationStretched:
                    var eulerAngles = Camera.eulerAngles;
                    _Transform.eulerAngles = new(0, eulerAngles.y, 0);
                    StretchHeight(eulerAngles.x);
                    break;

                case BillboardMode.UprightFacePositionStretched:
                    StretchHeight(Camera.eulerAngles.x);
                    goto case BillboardMode.UprightFacePosition;
            }
        }

        /************************************************************************************************************************/

        /// <summary>
        /// Scales the <see cref="Transform"/> on the Y axis to maintain the same screen size
        /// regardless of the <see cref="Camera"/>'s Euler X Angle.
        /// </summary>
        /// <remarks>This calculation only makes sense with an orthographic camera.</remarks>
        private void StretchHeight(float eulerX)
        {
            if (eulerX > 180)
                eulerX -= 360;
            else if (eulerX < -180)
                eulerX += 360;

            _Transform.localScale = new(
                1,
                1 / Mathf.Cos(eulerX * Mathf.Deg2Rad),
                1);
        }

        /// <summary>
        /// Resets the <see cref="Transform.localScale"/> to 1 if not using a stretched <see cref="Mode"/>.
        /// </summary>
        private void ResetScaleIfNotStretched()
        {
            if (_Transform == null)
                return;

            switch (_Mode)
            {
                case BillboardMode.UprightMatchRotationStretched:
                case BillboardMode.UprightFacePositionStretched:
                    break;

                default:
                    _Transform.localScale = Vector3.one;
                    break;
            }
        }

        /************************************************************************************************************************/

        /// <summary>
        /// Sets the <see cref="Animations"/> and plays the appropriate animation
        /// based on the current rotation and <see cref="Forward"/> direction.
        /// </summary>
        public void SetAnimations(DirectionalSet<AnimationClip> animations, TGroup group = default)
        {
            _Animations = animations;
            PlayCurrentAnimation(group);
        }

        /************************************************************************************************************************/

        /// <summary>
        /// Plays the appropriate animation based on the current rotation and <see cref="Forward"/> direction.
        /// </summary>
        /// <remarks>
        /// If the `group` is the same as the previous, the new animation will be given the same
        /// <see cref="AnimancerState.NormalizedTime"/> as the previous.
        /// </remarks>
        public void PlayCurrentAnimation(TGroup group)
        {
            if (TryGetCurrentAnimation(out var animation))
            {
                TimeSynchronizer.StoreTime(_Animancer);

                _Animancer.Play(animation);

                TimeSynchronizer.SyncTime(_Animancer, group);
            }
        }

        /************************************************************************************************************************/

        /// <summary>
        /// Tries to get an appropriate animation based on the current rotation and <see cref="Forward"/> direction.
        /// </summary>
        private bool TryGetCurrentAnimation(out AnimationClip animation)
        {
            if (_Animations == null ||
                _Forward == default)
            {
                animation = null;
                return false;
            }

            var localForward = _Transform.InverseTransformDirection(_Forward);
            var horizontalForward = new Vector2(localForward.x, localForward.z);

            animation = _Animations.Get(horizontalForward);
            return true;
        }

        /************************************************************************************************************************/
        #endregion
        /************************************************************************************************************************/
    }
}

