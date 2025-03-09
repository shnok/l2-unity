// Animancer // https://kybernetik.com.au/animancer // Copyright 2018-2025 Kybernetik //

#if UNITY_EDITOR && UNITY_IMGUI

using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

namespace Animancer.Editor
{
    /// <inheritdoc/>
    /// https://kybernetik.com.au/animancer/api/Animancer.Editor/ControllerTransitionDrawer
    [CustomPropertyDrawer(typeof(ControllerTransition<>), true)]
    [CustomPropertyDrawer(typeof(ControllerTransition), true)]
    public class ControllerTransitionDrawer : TransitionDrawer
    {
        /************************************************************************************************************************/

        private readonly string[] Parameters;
        private readonly string[] ParameterPropertySuffixes;

        /************************************************************************************************************************/

        /// <summary>Creates a new <see cref="ControllerTransitionDrawer"/> without any parameters.</summary>
        public ControllerTransitionDrawer()
            : base(ControllerTransition.ControllerFieldName)
        { }

        /// <summary>Creates a new <see cref="ControllerTransitionDrawer"/> and sets the <see cref="Parameters"/>.</summary>
        public ControllerTransitionDrawer(params string[] parameters)
            : base(ControllerTransition.ControllerFieldName)
        {
            Parameters = parameters;
            if (parameters == null)
                return;

            ParameterPropertySuffixes = new string[parameters.Length];

            for (int i = 0; i < ParameterPropertySuffixes.Length; i++)
            {
                ParameterPropertySuffixes[i] = "." + parameters[i];
            }
        }

        /************************************************************************************************************************/

        /// <inheritdoc/>
        protected override void DoChildPropertyGUI(
            ref Rect area,
            SerializedProperty rootProperty,
            SerializedProperty property,
            GUIContent label)
        {
            var path = property.propertyPath;

            if (ParameterPropertySuffixes != null)
            {
                var controllerProperty = rootProperty.FindPropertyRelative(MainPropertyName);
                if (controllerProperty.objectReferenceValue is AnimatorController controller)
                {
                    for (int i = 0; i < ParameterPropertySuffixes.Length; i++)
                    {
                        if (path.EndsWith(ParameterPropertySuffixes[i]))
                        {
                            area.height = AnimancerGUI.LineHeight;
                            DoParameterGUI(area, controller, property);
                            return;
                        }
                    }
                }
            }

            EditorGUI.BeginChangeCheck();

            base.DoChildPropertyGUI(ref area, rootProperty, property, label);

            // When the controller changes, validate all parameters.
            if (EditorGUI.EndChangeCheck() &&
                Parameters != null &&
                path.EndsWith(MainPropertyPathSuffix))
            {
                if (property.objectReferenceValue is AnimatorController controller)
                {
                    for (int i = 0; i < Parameters.Length; i++)
                    {
                        property = rootProperty.FindPropertyRelative(Parameters[i]);
                        var parameterName = property.stringValue;

                        // If a parameter is missing, assign it to the first float parameter.
                        if (!HasFloatParameter(controller, parameterName))
                        {
                            parameterName = GetFirstFloatParameterName(controller);
                            if (!string.IsNullOrEmpty(parameterName))
                                property.stringValue = parameterName;
                        }
                    }
                }
            }
        }

        /************************************************************************************************************************/

        /// <summary>Draws a dropdown menu to select the name of a parameter in the `controller`.</summary>
        protected void DoParameterGUI(Rect area, AnimatorController controller, SerializedProperty property)
        {
            var parameterName = property.stringValue;
            var parameters = controller.parameters;

            using (var label = PooledGUIContent.Acquire(property))
            {
                var propertyLabel = EditorGUI.BeginProperty(area, label, property);

                var xMax = area.xMax;
                area.width = EditorGUIUtility.labelWidth;
                EditorGUI.PrefixLabel(area, propertyLabel);

                area.x += area.width;
                area.xMax = xMax;
            }

            var color = GUI.color;
            if (!HasFloatParameter(controller, parameterName))
                GUI.color = AnimancerGUI.ErrorFieldColor;

            using (var label = PooledGUIContent.Acquire(parameterName))
            {
                if (EditorGUI.DropdownButton(area, label, FocusType.Passive))
                {
                    property = property.Copy();

                    var menu = new GenericMenu();

                    for (int i = 0; i < parameters.Length; i++)
                    {
                        var parameter = parameters[i];
                        Serialization.AddPropertyModifierFunction(menu, property, parameter.name,
                            parameter.type == AnimatorControllerParameterType.Float,
                            (targetProperty) =>
                            {
                                targetProperty.stringValue = parameter.name;
                            });
                    }

                    if (menu.GetItemCount() == 0)
                        menu.AddDisabledItem(new("No Parameters"));

                    menu.ShowAsContext();
                }
            }

            GUI.color = color;

            EditorGUI.EndProperty();
        }

        /************************************************************************************************************************/

        private static bool HasFloatParameter(AnimatorController controller, string name)
        {
            if (string.IsNullOrEmpty(name))
                return false;

            var parameters = controller.parameters;

            for (int i = 0; i < parameters.Length; i++)
            {
                var parameter = parameters[i];
                if (parameter.type == AnimatorControllerParameterType.Float &&
                    parameter.name == name)
                {
                    return true;
                }
            }

            return false;
        }

        /************************************************************************************************************************/

        private static string GetFirstFloatParameterName(AnimatorController controller)
        {
            var parameters = controller.parameters;

            for (int i = 0; i < parameters.Length; i++)
            {
                var parameter = parameters[i];
                if (parameter.type == AnimatorControllerParameterType.Float)
                {
                    return parameter.name;
                }
            }

            return "";
        }

        /************************************************************************************************************************/
    }
}

#endif

