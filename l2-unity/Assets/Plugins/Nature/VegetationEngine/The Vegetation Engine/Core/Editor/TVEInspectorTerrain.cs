//Cristian Pop - https://boxophobic.com/

using UnityEngine;
using UnityEditor;

namespace TheVegetationEngine
{
    [DisallowMultipleComponent]
    [CustomEditor(typeof(TVETerrain))]
    public class TVEInspectorTerrain : Editor
    {
        string excludeProp = "m_Script";
        //TVETerrain targetScript;

        //void OnEnable()
        //{
        //    targetScript = (TVETerrain)target;
        //}

        public override void OnInspectorGUI()
        {
            DrawInspector();
        }

        void DrawInspector()
        {
            serializedObject.Update();

            DrawPropertiesExcluding(serializedObject, excludeProp);

            serializedObject.ApplyModifiedProperties();
        }
    }
}


