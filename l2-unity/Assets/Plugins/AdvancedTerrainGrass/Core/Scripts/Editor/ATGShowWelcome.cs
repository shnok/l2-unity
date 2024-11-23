using UnityEditor;
using UnityEngine;
using System;
using System.Collections;



[InitializeOnLoad]
public class ATGShowWelcome : MonoBehaviour
{
    
	static ATGShowWelcome()
	{
	//	To show it at start up
		EditorApplication.update += Update;
	}


    static void Update()
	{
		EditorApplication.update -= Update;

		if( !EditorApplication.isPlayingOrWillChangePlaymode )
		{
			var hide = EditorPrefs.GetBool("ATGDoNotShowWelcome");
			if(!hide)
			{
				ATGWelcome.Init();
			}
		}
	}
}