using UnityEngine;
using System.Collections;
using UnityEditor;

public class ATGWindFoliageDrawer : MaterialPropertyDrawer {

	public override void OnGUI (Rect position, MaterialProperty prop, string label, MaterialEditor editor) {
		
	//	Needed since Unity 2019
		EditorGUIUtility.labelWidth = 0;

		Vector4 vec4value = prop.vectorValue;

		GUILayout.Space(-18);
		EditorGUI.BeginChangeCheck();
		EditorGUILayout.BeginVertical();
			vec4value.x = EditorGUILayout.Slider("Primary Strength", vec4value.x, 0.0f, 10.0f);
			vec4value.y = EditorGUILayout.Slider("Scondary Strength", vec4value.y, 0.0f, 10.0f);
			vec4value.z = EditorGUILayout.Slider("Edge Flutter", vec4value.z, 0.0f, 4.0f);
			GUILayout.Space(5);
			vec4value.w = (float)EditorGUILayout.IntSlider("LOD Level", (int)vec4value.w, 0, 8);
		EditorGUILayout.EndVertical();
		if (EditorGUI.EndChangeCheck ()) {
			prop.vectorValue = vec4value;
		}
	}
}