// Animancer // https://kybernetik.com.au/animancer // Copyright 2018-2025 Kybernetik //

#if UNITY_EDITOR

using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Animancer.Editor
{
    /// <summary>[Editor-Only] A custom Inspector for <see cref="SoloAnimation"/>.</summary>
    /// https://kybernetik.com.au/animancer/api/Animancer.Editor/SoloAnimationEditor
    [UnityEditor.CustomEditor(typeof(SoloAnimation), true), UnityEditor.CanEditMultipleObjects]
    public class SoloAnimationEditor : UnityEditor.Editor
    {
        /************************************************************************************************************************/

        /// <summary>The <see cref="UnityEditor.Editor.target"/>.</summary>
        [field: NonSerialized]
        public SoloAnimation Target { get; private set; }

        /// <summary>The <see cref="UnityEditor.Editor.targets"/>.</summary>
        [field: NonSerialized]
        public Object[] Targets { get; private set; }

        /// <summary>The animator referenced by each target.</summary>
        [NonSerialized]
        private Animator[] _Animators;

        /// <summary>A <see cref="UnityEditor.SerializedObject"/> encapsulating the <see cref="_Animators"/>.</summary>
        [NonSerialized]
        private UnityEditor.SerializedObject _SerializedAnimator;

        /// <summary>The <see cref="Animator.keepAnimatorStateOnDisable"/> property.</summary>
        [NonSerialized]
        private UnityEditor.SerializedProperty _KeepStateOnDisable;

        /// <summary>The backing field of the <see cref="Animator.keepAnimatorStateOnDisable"/> property.</summary>
        private const string KeeyStateOnDisableField = "m_KeepAnimatorStateOnDisable";

        /************************************************************************************************************************/

        /// <summary>Initializes the targets.</summary>
        protected virtual void OnEnable()
        {
            Target = (SoloAnimation)target;
            Targets = targets;
        }

        /************************************************************************************************************************/

        /// <inheritdoc/>
        public override void OnInspectorGUI()
        {
            DoSerializedFieldsGUI();
            RefreshSerializedAnimator();
            DoStopOnDisableGUI();
            DoRuntimeDetailsGUI();
        }

        /************************************************************************************************************************/

        /// <summary>Draws the target's serialized fields.</summary>
        private void DoSerializedFieldsGUI()
        {
            serializedObject.Update();

            var property = serializedObject.GetIterator();

            property.NextVisible(true);

            if (property.name != "m_Script")
                UnityEditor.EditorGUILayout.PropertyField(property, true);

            while (property.NextVisible(false))
            {
                UnityEditor.EditorGUILayout.PropertyField(property, true);
            }

            serializedObject.ApplyModifiedProperties();
        }

        /************************************************************************************************************************/

        /// <summary>Ensures that the cached references relating to the target's <see cref="Animator"/> are correct.</summary>
        private void RefreshSerializedAnimator()
        {
            AnimancerUtilities.SetLength(ref _Animators, Targets.Length);

            var dirty = false;
            var hasAll = true;

            for (int i = 0; i < _Animators.Length; i++)
            {
                var animator = (Targets[i] as SoloAnimation).Animator;
                if (_Animators[i] != animator)
                {
                    _Animators[i] = animator;
                    dirty = true;
                }

                if (animator == null)
                    hasAll = false;
            }

            if (!dirty)
                return;

            OnDisable();

            if (!hasAll)
                return;

            _SerializedAnimator = new(_Animators);
            _KeepStateOnDisable = _SerializedAnimator.FindProperty(KeeyStateOnDisableField);
        }

        /************************************************************************************************************************/

        /// <summary>
        /// Draws a toggle inverted from the <see cref="Animator.keepAnimatorStateOnDisable"/> field.
        /// </summary>
        private void DoStopOnDisableGUI()
        {
            var area = AnimancerGUI.LayoutSingleLineRect();

            using (var label = PooledGUIContent.Acquire("Stop On Disable",
                "If true, disabling this object will stop and rewind the animation." +
                " Otherwise it will simply be paused so it can resume from there when re-enabled."))
            {
                if (_KeepStateOnDisable != null)
                {
                    _KeepStateOnDisable.serializedObject.Update();

                    var content = UnityEditor.EditorGUI.BeginProperty(area, label, _KeepStateOnDisable);

                    _KeepStateOnDisable.boolValue = !UnityEditor.EditorGUI.Toggle(area, content, !_KeepStateOnDisable.boolValue);

                    UnityEditor.EditorGUI.EndProperty();

                    _KeepStateOnDisable.serializedObject.ApplyModifiedProperties();
                }
                else
                {
                    label.tooltip = $"Unable to locate field: {nameof(Animator)}.{KeeyStateOnDisableField}";
                    using (new UnityEditor.EditorGUI.DisabledScope(true))
                        UnityEditor.EditorGUI.Toggle(area, label, false);
                }
            }
        }

        /************************************************************************************************************************/

        /// <summary>Draws the target's runtime details.</summary>
        private void DoRuntimeDetailsGUI()
        {
            if (Targets.Length != 1)
                return;

            if (!UnityEditor.EditorApplication.isPlaying &&
                !Target.ApplyInEditMode)
                return;

            AnimancerGUI.BeginVerticalBox(GUI.skin.box);

            if (!Target.IsInitialized)
            {
                GUILayout.Label("Not Initialized");
            }
            else
            {
                UnityEditor.EditorGUILayout.LabelField("Playable Graph", "Not Serialized");

                UnityEditor.EditorGUI.BeginChangeCheck();
                var isPlaying = UnityEditor.EditorGUILayout.Toggle("Is Playing", Target.IsPlaying);
                if (UnityEditor.EditorGUI.EndChangeCheck())
                    Target.IsPlaying = isPlaying;

                UnityEditor.EditorGUI.BeginChangeCheck();
                var time = UnityEditor.EditorGUILayout.FloatField("Time", Target.Time);
                if (UnityEditor.EditorGUI.EndChangeCheck())
                {
                    Target.Time = time;
                    Target.Evaluate();
                }

                time = AnimancerUtilities.Wrap01(Target.NormalizedTime);
                if (time == 0 && Target.Time != 0)
                    time = 1;

                UnityEditor.EditorGUI.BeginChangeCheck();
                time = UnityEditor.EditorGUILayout.Slider("Normalized Time", time, 0, 1);
                if (UnityEditor.EditorGUI.EndChangeCheck())
                {
                    Target.NormalizedTime = time;
                    Target.Evaluate();
                }
            }

            AnimancerGUI.EndVerticalBox(GUI.skin.box);
            Repaint();
        }

        /************************************************************************************************************************/

        /// <summary>Cleans up cached references relating to the target's <see cref="Animator"/>.</summary>
        protected virtual void OnDisable()
        {
            if (_SerializedAnimator != null)
            {
                _SerializedAnimator.Dispose();
                _SerializedAnimator = null;
                _KeepStateOnDisable = null;
            }
        }

        /************************************************************************************************************************/
    }
}

#endif
