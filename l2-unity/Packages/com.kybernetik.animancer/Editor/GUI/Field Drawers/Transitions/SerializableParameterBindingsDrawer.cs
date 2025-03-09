// Animancer // https://kybernetik.com.au/animancer // Copyright 2018-2025 Kybernetik //

#if UNITY_EDITOR

using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using static Animancer.Editor.AnimancerGUI;

namespace Animancer.Editor
{
    /// <summary><see cref="PropertyDrawer"/> for <see cref="ControllerState.SerializableParameterBindings"/>.</summary>
    /// https://kybernetik.com.au/animancer/api/Animancer.Editor/SerializableParameterBindingsDrawer
    [CustomPropertyDrawer(typeof(ControllerState.SerializableParameterBindings), true)]
    public class SerializableParameterBindingsDrawer : PropertyDrawer
    {
        /************************************************************************************************************************/

        /// <inheritdoc/>
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (!property.isExpanded)
                return LineHeight;

            GetFields(property, out var mode, out var bindings);

            var count = bindings.arraySize;
            if (count > 0 && mode.boolValue)
                count = 1 + Mathf.CeilToInt(count * 0.5f);

            return CalculateHeight(count + 3);
        }

        /************************************************************************************************************************/

        /// <inheritdoc/>
        public override void OnGUI(Rect area, SerializedProperty property, GUIContent label)
        {
            area.height = LineHeight;

            var isExpanded = EditorGUI.PropertyField(area, property, label, false);
            if (!isExpanded)
                return;

            EditorGUI.indentLevel++;

            NextVerticalArea(ref area);

            GetFields(property, out var mode, out var bindings);

            var parameterList = GetContextParameterList(property);

            var bindingCount = bindings.arraySize;

            DoModeGUI(ref area, mode, bindingCount, parameterList);

            var modeValue = mode.boolValue;

            DoBindingCountGUI(ref area, bindings, modeValue, ref bindingCount, parameterList);

            DoBindingsGUI(area, bindings, modeValue, bindingCount, parameterList);

            EditorGUI.indentLevel--;
        }

        /************************************************************************************************************************/

        private void DoModeGUI(
            ref Rect area,
            SerializedProperty mode,
            int bindingCount,
            string parameterList)
        {
            using var label = PooledGUIContent.Acquire();

            if (bindingCount == 0)
            {
                label.text = "Bind All Parameters";
                label.tooltip =
                    "If enabled, all parameters in the Animator Controller will be bound" +
                    " to Animancer parameters with the same name and the Bindings array can be left empty." +
                    parameterList;
            }
            else
            {
                label.text = "Rebind Names";
                label.tooltip =
                    "If enabled, the Bindings array will be taken in pairs so that each" +
                    " Animator Controller parameter can be bound to an Animancer Parameter with different name." +
                    parameterList;
            }

            EditorGUI.PropertyField(area, mode, label, false);

            NextVerticalArea(ref area);
        }

        /************************************************************************************************************************/

        private void DoBindingCountGUI(
            ref Rect area,
            SerializedProperty bindings,
            bool mode,
            ref int bindingCount,
            string parameterList)
        {
            using var label = PooledGUIContent.Acquire(
                "Bindings",
                "The names of parameters in the Animator Controller to bind to Animancer parameters." +
                "\n� Leave this array empty and enable the toggle if you want to bind all parameters." +
                parameterList);

            var newCount = bindingCount;

            if (mode && bindingCount > 0)
                newCount /= 2;

            newCount = EditorGUI.DelayedIntField(area, label, newCount);

            if (newCount < 0)
                newCount = 0;
            else if (mode && newCount > 0)
                newCount *= 2;

            if (bindingCount != newCount)
            {
                bindingCount = newCount;
                bindings.arraySize = newCount;
            }

            NextVerticalArea(ref area);
        }

        /************************************************************************************************************************/

        private void DoBindingsGUI(
            Rect area,
            SerializedProperty bindings,
            bool mode,
            int bindingCount,
            string parameterList)
        {
            if (bindingCount <= 0)
                return;

            using var label = PooledGUIContent.Acquire();

            if (mode)
            {
                var controllerArea = EditorGUI.IndentedRect(area);
                controllerArea.xMin -= 1;// Not sure why.
                var animancerArea = StealFromRight(ref controllerArea, controllerArea.width * 0.5f, StandardSpacing);

                label.text = "Controller";
                label.tooltip = "The name of the Animator Controller parameter" + parameterList;
                GUI.Label(controllerArea, label);

                label.text = "Animancer";
                label.tooltip = "The name of the Animancer parameter";
                GUI.Label(animancerArea, label);

                NextVerticalArea(ref controllerArea);
                NextVerticalArea(ref animancerArea);

                for (int i = 0; i < bindingCount; i++)
                {
                    DoBindingGUI(ref controllerArea, bindings, i, GUIContent.none);
                    i++;
                    DoBindingGUI(ref animancerArea, bindings, i, GUIContent.none);
                }
            }
            else
            {
                EditorGUI.indentLevel++;

                label.tooltip = "";

                for (int i = 0; i < bindingCount; i++)
                {
                    label.text = "Binding " + i;
                    DoBindingGUI(ref area, bindings, i, label);
                }

                EditorGUI.indentLevel--;
            }
        }

        /************************************************************************************************************************/

        private static void DoBindingGUI(ref Rect area, SerializedProperty bindings, int index, GUIContent label)
        {
            var indentLevel = EditorGUI.indentLevel;
            if (string.IsNullOrEmpty(label.text))
                EditorGUI.indentLevel = 0;

            var binding = bindings.GetArrayElementAtIndex(index);

            EditorGUI.PropertyField(area, binding, label);

            NextVerticalArea(ref area);

            EditorGUI.indentLevel = indentLevel;
        }

        /************************************************************************************************************************/

        private void GetFields(
            SerializedProperty root,
            out SerializedProperty mode,
            out SerializedProperty bindings)
        {
            mode = root.FindPropertyRelative(ControllerState.SerializableParameterBindings.ModeFieldName);
            bindings = root.FindPropertyRelative(ControllerState.SerializableParameterBindings.BindingsFieldName);
        }

        /************************************************************************************************************************/

        private string GetContextParameterList(SerializedProperty property)
        {
            var path = property.propertyPath;
            var lastDot = path.LastIndexOf('.');
            if (lastDot < 0)
                return null;

            path = path[..(lastDot + 1)] + ControllerTransition.ControllerFieldName;
            property = property.serializedObject.FindProperty(path);
            if (property == null ||
                property.objectReferenceValue is not AnimatorController animatorController)
                return null;

            return GetParameterList(animatorController);
        }

        private readonly Dictionary<AnimatorController, string>
            ControllerToParameterList = new();

        private string GetParameterList(AnimatorController animatorController)
        {
            if (animatorController == null)
                return null;

            if (ControllerToParameterList.TryGetValue(animatorController, out var parameterList))
                return parameterList;

            var text = StringBuilderPool.Instance.Acquire();

            var parameters = animatorController.parameters;
            if (parameters.Length > 0)
            {
                text.Append("\n\nParameters in ")
                    .Append(animatorController.name)
                    .Append(':');

                for (int i = 0; i < parameters.Length; i++)
                {
                    var parameter = parameters[i];

                    text.Append("\n� ")
                        .Append(parameter.type)
                        .Append(' ')
                        .Append(parameter.name);
                }
            }

            parameterList = text.ReleaseToString();
            ControllerToParameterList.Add(animatorController, parameterList);
            return parameterList;
        }

        /************************************************************************************************************************/
    }
}

#endif

