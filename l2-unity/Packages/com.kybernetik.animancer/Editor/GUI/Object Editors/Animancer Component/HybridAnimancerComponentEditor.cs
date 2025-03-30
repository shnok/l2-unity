// Animancer // https://kybernetik.com.au/animancer // Copyright 2018-2025 Kybernetik //

#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

namespace Animancer.Editor
{
    /// <summary>[Editor-Only] A custom Inspector for <see cref="HybridAnimancerComponentEditor"/>s.</summary>
    /// https://kybernetik.com.au/animancer/api/Animancer.Editor/HybridAnimancerComponentEditor
    /// 
    [CustomEditor(typeof(HybridAnimancerComponent), true), CanEditMultipleObjects]
    public class HybridAnimancerComponentEditor : NamedAnimancerComponentEditor
    {
        /************************************************************************************************************************/

        /// <inheritdoc/>
        protected override bool DoOverridePropertyGUI(string path, SerializedProperty property, GUIContent label)
        {
            switch (path)
            {
                case "_Controller":

                    EditorGUILayout.PropertyField(property, label, true);

                    property = property.FindPropertyRelative("_Controller");

                    var hasAnimatorController = property?.objectReferenceValue != null;
                    var warning = GetAnimatorControllerWarning(hasAnimatorController, out var messageType);
                    if (warning is not null)
                    {
                        EditorGUILayout.HelpBox(warning, messageType);
                        if (AnimancerGUI.TryUseClickEventInLastRect())
                            Application.OpenURL(Strings.DocsURLs.AnimatorControllers);
                    }

                    return true;
            }

            return base.DoOverridePropertyGUI(path, property, label);
        }

        /************************************************************************************************************************/

        private string GetAnimatorControllerWarning(bool hasAnimatorController, out MessageType messageType)
        {
            messageType = MessageType.Warning;

            if (!hasAnimatorController)
            {
                return
                    $"No Animator Controller is assigned to this component so" +
                    $" you should likely use a base {nameof(AnimancerComponent)} instead." +
                    $" Click here for more information.";
            }

            if (Targets.Length > 0)
            {
                var animator = Targets[0].Animator;
                if (animator != null && animator.runtimeAnimatorController != null)
                {
                    return
                        $"A Native Animator Controller is assigned to the Animator component" +
                        $" and a Hybrid Animator Controller is also assigned to this component." +
                        $" That's not necessarily a problem, but using both systems at the same time is very unusual" +
                        $" and likely a waste of performance if you don't need to play both Animator Controllers at once." +
                        $" Click here for more information.";
                }
            }

            return null;
        }

        /************************************************************************************************************************/
    }
}

#endif

