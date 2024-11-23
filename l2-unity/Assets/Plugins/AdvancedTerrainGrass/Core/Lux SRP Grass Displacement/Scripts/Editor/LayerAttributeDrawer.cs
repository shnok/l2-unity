using System;
using UnityEngine;
using System.Collections;
#if UNITY_EDITOR
	using UnityEditor;
#endif


namespace Lux_SRP_GrassDisplacement {

	#if UNITY_EDITOR
		[CustomPropertyDrawer(typeof(LayerAttribute))]
		public class LayerAttributeDrawer : PropertyDrawer {
		    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
		        property.intValue = EditorGUI.LayerField(position, label, property.intValue);
		    }
		}
	#endif
}