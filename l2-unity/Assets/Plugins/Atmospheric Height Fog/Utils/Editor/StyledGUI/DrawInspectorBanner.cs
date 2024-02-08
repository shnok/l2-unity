// Cristian Pop - https://boxophobic.com/

using UnityEngine;
using UnityEditor;
using Boxophobic.Constants;

namespace Boxophobic.StyledGUI
{
    public partial class StyledGUI
    {
        public static void DrawInspectorBanner(Color color, string title)
        {
            GUILayout.Space(10);

            var fullRect = GUILayoutUtility.GetRect(0, 0, 36, 0);
            var fillRect = new Rect(0, fullRect.position.y, fullRect.xMax + 3, 36);
            var lineRect = new Rect(0, fullRect.position.y, fullRect.xMax + 3, 1);

            if (EditorGUIUtility.isProSkin)
            {
                color = new Color(color.r, color.g, color.b, 1f);
            }
            else
            {
                color = CONSTANT.ColorLightGray;
            }

            EditorGUI.DrawRect(fillRect, color);
            EditorGUI.DrawRect(lineRect, CONSTANT.LineColor);

            Color guiColor = CONSTANT.ColorDarkGray;

            GUI.Label(fullRect, "<size=16><color=#" + ColorUtility.ToHtmlStringRGB(guiColor) + ">" + title + "</color></size>", CONSTANT.TitleStyle);

            GUILayout.Space(10);
        }

        public static void DrawInspectorBanner(string title)
        {
            GUILayout.Space(10);

            var fullRect = GUILayoutUtility.GetRect(0, 0, 36, 0);
            var fillRect = new Rect(0, fullRect.position.y, fullRect.xMax + 3, 36);
            var lineRect = new Rect(0, fullRect.position.y, fullRect.xMax + 3, 1);

            Color color;
            Color guiColor;

            if (EditorGUIUtility.isProSkin)
            {
                color = CONSTANT.ColorDarkGray;
                guiColor = CONSTANT.ColorLightGray;
            }
            else
            {
                color = CONSTANT.ColorLightGray;
                guiColor = CONSTANT.ColorDarkGray;
            }

            EditorGUI.DrawRect(fillRect, color);
            EditorGUI.DrawRect(lineRect, CONSTANT.LineColor);

            GUI.Label(fullRect, "<size=16><color=#" + ColorUtility.ToHtmlStringRGB(guiColor) + ">" + title + "</color></size>", CONSTANT.TitleStyle);

            GUILayout.Space(10);
        }

        public static void DrawInspectorBanner(Color color, string title, string subtitle)
        {
            GUIStyle titleStyle = new GUIStyle("label")
            {
                richText = true,
                alignment = TextAnchor.MiddleCenter
            };

            GUIStyle subTitleStyle = new GUIStyle("label")
            {
                richText = true,
                alignment = TextAnchor.MiddleRight
            };

            GUILayout.Space(10);

            var fullRect = GUILayoutUtility.GetRect(0, 0, 36, 0);
            var fillRect = new Rect(0, fullRect.position.y, fullRect.xMax + 3, 36);
            var subRect = new Rect(0, fullRect.position.y - 2, fullRect.xMax, 36);
            var lineRect = new Rect(0, fullRect.position.y, fullRect.xMax + 3, 1);

            if (EditorGUIUtility.isProSkin)
            {
                color = new Color(color.r, color.g, color.b, 1f);
            }
            else
            {
                color = CONSTANT.ColorLightGray;
            }

            EditorGUI.DrawRect(fillRect, color);
            EditorGUI.DrawRect(lineRect, CONSTANT.LineColor);

            Color guiColor = CONSTANT.ColorDarkGray;

            GUI.Label(fullRect, "<size=16><color=#" + ColorUtility.ToHtmlStringRGB(guiColor) + ">" + title + "</color></size>", titleStyle);
            GUI.Label(subRect, "<size=10><color=#808080>" + subtitle + "</color></size>", subTitleStyle);

            GUILayout.Space(10);
        }

        public static void DrawInspectorBanner(string title, string subtitle)
        {
            GUIStyle titleStyle = new GUIStyle("label")
            {
                richText = true,
                alignment = TextAnchor.MiddleCenter
            };

            GUIStyle subTitleStyle = new GUIStyle("label")
            {
                richText = true,
                alignment = TextAnchor.MiddleRight
            };

            GUILayout.Space(10);

            var fullRect = GUILayoutUtility.GetRect(0, 0, 36, 0);
            var fillRect = new Rect(0, fullRect.position.y, fullRect.xMax + 3, 36);
            var subRect = new Rect(0, fullRect.position.y - 2, fullRect.xMax, 36);
            var lineRect = new Rect(0, fullRect.position.y, fullRect.xMax + 3, 1);

            Color color;
            Color guiColor;

            if (EditorGUIUtility.isProSkin)
            {
                color = CONSTANT.ColorDarkGray;
                guiColor = CONSTANT.ColorLightGray;
            }
            else
            {
                color = CONSTANT.ColorLightGray;
                guiColor = CONSTANT.ColorDarkGray;
            }

            EditorGUI.DrawRect(fillRect, color);
            EditorGUI.DrawRect(lineRect, CONSTANT.LineColor);

            GUI.Label(fullRect, "<size=16><color=#" + ColorUtility.ToHtmlStringRGB(guiColor) + ">" + title + "</color></size>", titleStyle);
            GUI.Label(subRect, "<size=10><color=#808080>" + subtitle + "</color></size>", subTitleStyle);

            GUILayout.Space(10);
        }
    }
}

