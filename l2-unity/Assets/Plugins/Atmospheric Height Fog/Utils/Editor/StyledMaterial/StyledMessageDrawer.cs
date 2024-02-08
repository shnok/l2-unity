// Cristian Pop - https://boxophobic.com/

using UnityEngine;
using UnityEditor;
using System;

namespace Boxophobic.StyledGUI
{
    public class StyledMessageDrawer : MaterialPropertyDrawer
    {
        public string type;
        public string message;
        public string keyword;
        public float value;
        public float top;
        public float down;

        MessageType mType;

        public StyledMessageDrawer(string t, string m)
        {
            type = t;
            message = m;
            keyword = null;

            this.top = 0;
            this.down = 0;
        }

        public StyledMessageDrawer(string t, string m, float top, float down)
        {
            type = t;
            message = m;
            keyword = null;

            this.top = top;
            this.down = down;
        }

        public StyledMessageDrawer(string t, string m, string k, float v, float top, float down)
        {
            type = t;
            message = m;
            keyword = k;
            value = v;

            this.top = top;
            this.down = down;
        }

        public override void OnGUI(Rect position, MaterialProperty prop, String label, MaterialEditor materialEditor)
        {
            Material material = materialEditor.target as Material;

            if (type == "None")
            {
                mType = MessageType.None;
            }
            else if (type == "Info")
            {
                mType = MessageType.Info;
            }
            else if (type == "Warning")
            {
                mType = MessageType.Warning;
            }
            else if (type == "Error")
            {
                mType = MessageType.Error;
            }

            message = message.Replace("__", ",");

            if (keyword != null)
            {
                if (material.HasProperty(keyword))
                {
                    if (material.GetFloat(keyword) == value)
                    {
                        GUILayout.Space(top);

                        EditorGUILayout.HelpBox(message, mType);

                        GUILayout.Space(down);

                    }
                }
            }
            else
            {
                GUILayout.Space(top);
                EditorGUILayout.HelpBox(message, mType);
                GUILayout.Space(down);
            }
        }

        public override float GetPropertyHeight(MaterialProperty prop, string label, MaterialEditor editor)
        {
            return -2;
        }
    }
}
