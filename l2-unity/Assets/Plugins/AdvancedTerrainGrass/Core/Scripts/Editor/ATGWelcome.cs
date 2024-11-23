using UnityEngine;
using UnityEditor;
using System;
using System.Collections;


public class ATGWelcome : EditorWindow
{
	
	static ATGWelcome window;

	[MenuItem( "Window/ATG/Welcome", false, 1000 )]
	public static void Init()
	{
		window = GetWindow<ATGWelcome>(false, "Advanced Terrain Grass", true);
		//window.minSize = window.maxSize = new Vector2(480, 270);
		window.minSize = new Vector2(480, 270);
	}

	public void OnGUI()
	{
		
		var _style_bodytxt = new GUIStyle(EditorStyles.label);
		_style_bodytxt.wordWrap = true;
		_style_bodytxt.fontSize = 12;

		GUILayout.Space(16);

		EditorGUILayout.BeginVertical();

		GUILayout.BeginHorizontal();
			GUILayout.Space(16);
			EditorGUILayout.LabelField("Welcome to the Advanced Terrain Grass!", EditorStyles.boldLabel);
			GUILayout.Space(16);
		GUILayout.EndHorizontal();

		GUILayout.Space(8);

		GUILayout.BeginHorizontal();
			GUILayout.Space(16);
			EditorGUILayout.LabelField(
				"Currently installed: Version 1.4 for Unity 2021.3.21 and above.", _style_bodytxt);
			GUILayout.Space(16);
		GUILayout.EndHorizontal();

		GUILayout.Space(16);

		GUILayout.BeginHorizontal();
			GUILayout.Space(16);
			EditorGUILayout.LabelField("Compatibility Notes", EditorStyles.boldLabel);
			GUILayout.Space(16);
		GUILayout.EndHorizontal();

		GUILayout.Space(8);

		GUILayout.BeginHorizontal();
			GUILayout.Space(16);
			EditorGUILayout.LabelField(
				"The package you have downloaded from the asset store installed shaders compatible with the built in Render Pipeline (BIRP). " + 
				"In case you want to use ATG with HDRP or URP you will have to install additional packages which are located in the folder 'PipelineSupport'.", _style_bodytxt);
			GUILayout.Space(16);
		GUILayout.EndHorizontal();


		GUILayout.Space(16);

		GUILayout.BeginHorizontal();
			GUILayout.Space(16);
			EditorGUILayout.LabelField("Useful Resources", EditorStyles.boldLabel);
			GUILayout.Space(16);
		GUILayout.EndHorizontal();

		GUILayout.Space(8);

		GUILayout.BeginHorizontal();
			GUILayout.Space(16);
			if (GUILayout.Button("Documentation"))
			{
				Application.OpenURL("https://docs.google.com/document/d/1JrSQVQaPkYLkbF6XJGpzcXnLcLc1G9FGzUhdW1xWoyA");
			}
			if (GUILayout.Button("URP 12 and above"))
			{
				Application.OpenURL("https://docs.google.com/document/d/12Lc7q75OHLs0ACgsZRUty7EdnLzAEvJ1C_Gl3cYlnDM");
			}
			if (GUILayout.Button("HDR 12 and above"))
			{
				Application.OpenURL("https://docs.google.com/document/d/1XU_Lgzrwr5oedB9_7kuGRsb7B4KWMvGBPgR9AgxAQ10");
			}
			if (GUILayout.Button("Forum Thread"))
			{
				Application.OpenURL("https://forum.unity.com/threads/released-advanced-terrain-grass.494865/");
			}
			GUILayout.Space(16);
		GUILayout.EndHorizontal();

		GUILayout.EndVertical();

		GUILayout.FlexibleSpace();

		EditorGUILayout.BeginVertical(GUILayout.Height(24));

			GUILayout.BeginHorizontal();
				GUILayout.Space(16);
				EditorGUI.BeginChangeCheck();
				var show = EditorPrefs.GetBool("ATGDoNotShowWelcome");
				show = EditorGUILayout.Toggle("Do not show again", show);
				if( EditorGUI.EndChangeCheck() )
				{
					EditorPrefs.SetBool("ATGDoNotShowWelcome", show);
				}
				GUILayout.Space(16);
			GUILayout.EndHorizontal();

		GUILayout.EndVertical();
	}
}



































