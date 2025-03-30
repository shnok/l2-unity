// Animancer // https://kybernetik.com.au/animancer // Copyright 2018-2025 Kybernetik //
// FlexiMotion // https://kybernetik.com.au/flexi-motion // Copyright 2023-2024 Kybernetik //

#pragma warning disable CS0649 // Field is never assigned to, and will always have its default value.

#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.PackageManager.UI;
using UnityEngine;

// Shared File Last Modified: 2024-07-13
namespace Animancer.Editor
// namespace FlexiMotion.Editor
{
    /// <summary>[Editor-Only] A welcome screen for an asset.</summary>
    /// https://kybernetik.com.au/animancer/api/Animancer.Editor/ReadMe
    /// https://kybernetik.com.au/flexi-motion/api/FlexiMotion.Editor/ReadMe
    /// 
    public abstract class ReadMe : ScriptableObject
    {
        /************************************************************************************************************************/
        #region Fields and Properties
        /************************************************************************************************************************/

        /// <summary>The release ID of the current version.</summary>
        public abstract int ReleaseNumber { get; }

        /// <summary>The display name of this product version.</summary>
        public abstract string VersionName { get; }

        /// <summary>The key used to save the release number.</summary>
        public abstract string PrefKey { get; }

        /// <summary>An introductory explanation of this asset.</summary>
        public virtual string Introduction => null;

        /// <summary>The base name of this product (without any "Lite", "Pro", "Demo", etc.).</summary>
        public abstract string BaseProductName { get; }

        /// <summary>The name of this product.</summary>
        public virtual string ProductName => BaseProductName;

        /// <summary>The display name for the samples section.</summary>
        public virtual string SamplesLabel => "Samples";

        /// <summary>The URL for the documentation.</summary>
        public abstract string DocumentationURL { get; }

        /// <summary>The URL for the change log of this version.</summary>
        public abstract string ChangeLogURL { get; }

        /// <summary>The URL for the sample documentation.</summary>
        public abstract string SamplesURL { get; }

        /// <summary>The URL to check for the latest version.</summary>
        public virtual string UpdateURL => null;

        /************************************************************************************************************************/

        /// <summary>
        /// The <see cref="ReadMe"/> file name ends with the <see cref="VersionName"/> to detect if the user imported
        /// this version without deleting a previous version.
        /// </summary>
        /// <remarks>
        /// When Unity's package importer sees an existing file with the same GUID as one in the package, it will
        /// overwrite that file but not move or rename it if the name has changed. So it will leave the file there with
        /// the old version name.
        /// </remarks>
        private bool HasCorrectName => name.EndsWith(VersionName);

        /************************************************************************************************************************/

        /// <summary>Sections to be displayed below the samples.</summary>
        public LinkSection[] LinkSections { get; set; }

        /// <summary>Extra sections to be displayed with the samples.</summary>
        public LinkSection[] ExtraSamples { get; set; }

        /************************************************************************************************************************/

        /// <summary>Creates a new <see cref="ReadMe"/> and sets the <see cref="LinkSections"/>.</summary>
        public ReadMe(params LinkSection[] linkSections)
        {
            LinkSections = linkSections;
            _CheckForUpdatesKey = $"{PrefKey}.{nameof(CheckForUpdates)}";
        }

        /************************************************************************************************************************/

        /// <summary>A heading with a link to be displayed in the Inspector.</summary>
        public class LinkSection
        {
            /************************************************************************************************************************/

            /// <summary>The main label.</summary>
            public readonly string Heading;

            /// <summary>A short description to be displayed near the <see cref="Heading"/>.</summary>
            public readonly string Description;

            /// <summary>A link that can be opened by clicking the <see cref="Heading"/>.</summary>
            public readonly string URL;

            /// <summary>An optional user-friendly version of the <see cref="URL"/>.</summary>
            public readonly string DisplayURL;

            /************************************************************************************************************************/

            /// <summary>Creates a new <see cref="LinkSection"/>.</summary>
            public LinkSection(string heading, string description, string url, string displayURL = null)
            {
                Heading = heading;
                Description = description;
                URL = url;
                DisplayURL = displayURL;
            }

            /************************************************************************************************************************/
        }

        /************************************************************************************************************************/

        /// <summary>Returns a <c>mailto</c> link.</summary>
        public static string GetEmailURL(string address, string subject)
            => $"mailto:{address}?subject={subject.Replace(" ", "%20")}";

        /************************************************************************************************************************/
        #endregion
        /************************************************************************************************************************/
        #region Show On Startup and Check for Updates
        /************************************************************************************************************************/

        private const string PrefPrefix = nameof(ReadMe) + ".";

        [SerializeField] private bool _DontShowOnStartup;

        [NonSerialized] private string _CheckForUpdatesKey;
        [NonSerialized] private bool _NewVersionAvailable;
        [NonSerialized] private string _UpdateCheckFailureMessage;
        [NonSerialized] private string _LatestVersionName;
        [NonSerialized] private string _LatestVersionChangeLogURL;
        [NonSerialized] private int _LatestVersionNumber;

#if UNITY_WEB_REQUEST
        [NonSerialized] private bool _CheckedForUpdates;
#endif

        private bool CheckForUpdates
        {
            get => EditorPrefs.GetBool(_CheckForUpdatesKey, true);
            set => EditorPrefs.SetBool(_CheckForUpdatesKey, value);
        }

        /************************************************************************************************************************/

        private static readonly Dictionary<Type, IDisposable>
            TypeToUpdateCheck = new();

        static ReadMe()
        {
            AssemblyReloadEvents.beforeAssemblyReload += () =>
            {
                foreach (var webRequest in TypeToUpdateCheck.Values)
                    webRequest.Dispose();

                TypeToUpdateCheck.Clear();
            };
        }

        /************************************************************************************************************************/

        /// <summary>Automatically checks for updates and selects a <see cref="ReadMe"/> on startup.</summary>
        [InitializeOnLoadMethod]
        private static void ShowReadMe()
        {
            EditorApplication.delayCall += () =>
            {
                var instances = FindInstances(out var autoSelect);

                for (int i = 0; i < instances.Count; i++)
                    instances[i].StartCheckForUpdates();

                // Delay the call again to ensure that the Project window actually shows the selection.
                if (autoSelect != null)
                    EditorApplication.delayCall += () =>
                        Selection.activeObject = autoSelect;
            };
        }

        /************************************************************************************************************************/

        /// <summary>
        /// Finds the most recently modified <see cref="ReadMe"/> asset with <see cref="_DontShowOnStartup"/> disabled.
        /// </summary>
        private static List<ReadMe> FindInstances(out ReadMe autoSelect)
        {
            var instances = new List<ReadMe>();

            DateTime latestWriteTime = default;
            autoSelect = null;
            string autoSelectGUID = null;

            var guids = AssetDatabase.FindAssets($"t:{nameof(ReadMe)}");
            for (int i = 0; i < guids.Length; i++)
            {
                var guid = guids[i];

                var assetPath = AssetDatabase.GUIDToAssetPath(guid);
                var asset = AssetDatabase.LoadAssetAtPath<ReadMe>(assetPath);
                if (asset == null)
                    continue;

                instances.Add(asset);

                if (asset._DontShowOnStartup && asset.HasCorrectName)
                    continue;

                // Check if already shown since opening the Unity Editor.
                if (SessionState.GetBool(PrefPrefix + guid, false))
                    continue;

                var writeTime = File.GetLastWriteTimeUtc(assetPath);
                if (latestWriteTime < writeTime)
                {
                    latestWriteTime = writeTime;
                    autoSelect = asset;
                    autoSelectGUID = guid;
                }
            }

            if (autoSelectGUID != null)
                SessionState.SetBool(PrefPrefix + autoSelectGUID, true);

            return instances;
        }

        /************************************************************************************************************************/

        /// <summary>Called after this object is loaded.</summary>
        protected virtual void OnEnable()
        {
            var name = GetType().FullName;
            var updateText = SessionState.GetString(PrefPrefix + name, "");
            OnUpdateCheckComplete(updateText, false);
        }

        /************************************************************************************************************************/

        private void StartCheckForUpdates()
        {
#if UNITY_WEB_REQUEST
            if (!CheckForUpdates ||
                _CheckedForUpdates)
                return;

            var type = GetType();
            if (TypeToUpdateCheck.ContainsKey(type))
                return;

            var url = UpdateURL;
            if (string.IsNullOrEmpty(url))
                return;

            _CheckedForUpdates = true;

            var webRequest = UnityEngine.Networking.UnityWebRequest.Get(url);
            TypeToUpdateCheck.Add(type, webRequest);
            webRequest.SendWebRequest().completed += _ =>
            {
                var name = type.FullName;

                if (webRequest.result == UnityEngine.Networking.UnityWebRequest.Result.Success)
                {
                    var text = webRequest.downloadHandler.text;
                    OnUpdateCheckComplete(text, true);
                    SessionState.SetString(PrefPrefix + name, text);
                }
                else
                {
                    _UpdateCheckFailureMessage = $"Update check failed: {webRequest.error}.";
                    SessionState.SetString(PrefPrefix + name, "");
                }

                TypeToUpdateCheck.Remove(type);
                webRequest.Dispose();
            };
#endif
        }

        /************************************************************************************************************************/

        private void OnUpdateCheckComplete(string text, bool log)
        {
#if UNITY_WEB_REQUEST
            if (string.IsNullOrEmpty(text))
                return;

            _CheckedForUpdates = true;

            var lines = text.Split('\n');
            if (lines.Length < 3)
            {
                _UpdateCheckFailureMessage = "Update check failed: text is malformed:\n" + text;
                return;
            }

            int.TryParse(lines[0], out _LatestVersionNumber);
            _LatestVersionName = lines[1].Trim();
            _LatestVersionChangeLogURL = $"{DocumentationURL}/{lines[2].Trim()}";

            if (ReleaseNumber >= _LatestVersionNumber)
                return;

            _NewVersionAvailable = true;

            if (log)
                Debug.Log($"{_LatestVersionName} is now available." +
                    $"\n• Change Log: {_LatestVersionChangeLogURL}" +
                    $"\n• This check can be disabled in the Read Me asset's Inspector.",
                    this);

            Selection.activeObject = this;
#endif
        }

        #endregion
        /************************************************************************************************************************/
        #region Custom Editor
        /************************************************************************************************************************/

        /// <summary>[Editor-Only] A custom Inspector for <see cref="ReadMe"/>.</summary>
        [CustomEditor(typeof(ReadMe), editorForChildClasses: true)]
        public class Editor : UnityEditor.Editor
        {
            /************************************************************************************************************************/

            private static readonly GUIContent
                GUIContent = new();

            private static GUIContent TempContent(string text, string tooltip = null)
            {
                GUIContent.text = text;
                GUIContent.tooltip = tooltip;
                return GUIContent;
            }

            /************************************************************************************************************************/

            [NonSerialized] private ReadMe _Target;
            [NonSerialized] private Texture2D _Icon;
            [NonSerialized] private string _ReleaseNumberPrefKey;
            [NonSerialized] private int _PreviousVersion;
            [NonSerialized] private IEnumerable<Sample> _Samples;
            [NonSerialized] private string _Title;
            [NonSerialized] private SerializedProperty _DontShowOnStartupProperty;

            /// <summary>The <see cref="ReadMe"/> being edited.</summary>
            public ReadMe Target => _Target;

            /************************************************************************************************************************/

            /// <summary>Don't use any margins.</summary>
            public override bool UseDefaultMargins() => false;

            /************************************************************************************************************************/

            protected virtual void OnEnable()
            {
                _Target = (ReadMe)target;
                _Icon = AssetPreview.GetMiniThumbnail(target);

                _ReleaseNumberPrefKey = _Target.PrefKey + "." + nameof(_Target.ReleaseNumber);
                _PreviousVersion = PlayerPrefs.GetInt(_ReleaseNumberPrefKey, -1);

                _Title = $"{_Target.ProductName}\n{_Target.VersionName}";
                _DontShowOnStartupProperty = serializedObject.FindProperty(nameof(_DontShowOnStartup));

                if (!string.IsNullOrEmpty(_Target.SamplesLabel))
                {
                    var assetPath = AssetDatabase.GetAssetPath(_Target);
                    var package = UnityEditor.PackageManager.PackageInfo.FindForAssetPath(assetPath);
                    if (package != null)
                    {
                        try
                        {
                            _Samples = Sample.FindByPackage(package.name, "");
                        }
                        catch { }// Unity sometimes throws an exception here. Not sure why.
                    }
                }
            }

            /************************************************************************************************************************/

            protected override void OnHeaderGUI()
            {
                GUILayout.BeginHorizontal(Styles.TitleArea);
                {
                    var title = TempContent(_Title);

                    var iconWidth = Styles.Title.CalcHeight(title, EditorGUIUtility.currentViewWidth);
                    GUILayout.Label(_Icon, GUILayout.Width(iconWidth), GUILayout.Height(iconWidth));
                    GUILayout.Label(title, Styles.Title);
                }
                GUILayout.EndHorizontal();
            }

            /************************************************************************************************************************/

            /// <inheritdoc/>
            public override void OnInspectorGUI()
            {
                serializedObject.Update();

                DoIntroduction();

                DoSpace();

                DoWarnings();

                DoNewVersionDetails();

                DoCheckForUpdates();
                DoShowOnStartup();

                DoSpace();

                DoIntroductionBlock();

                DoSpace();

                DoSampleBlock();

                DoSpace();

                DoSupportBlock();

                DoSpace();

                DoCheckForUpdates();
                DoShowOnStartup();

                serializedObject.ApplyModifiedProperties();
            }

            /************************************************************************************************************************/

            /// <summary>Draws a GUI space 20% of the height of a standard line.</summary>
            protected static void DoSpace()
                => GUILayout.Space(EditorGUIUtility.singleLineHeight * 0.2f);

            /************************************************************************************************************************/

            /// <summary>Draws the <see cref="ReadMe.Introduction"/> if it isn't <c>null</c>.</summary>
            protected virtual void DoIntroduction()
            {
                var introduction = _Target.Introduction;
                if (introduction == null)
                    return;

                DoSpace();
                GUILayout.Label(introduction, EditorStyles.wordWrappedLabel);
            }

            /************************************************************************************************************************/

            /// <summary>Draws a message indicating whether a new version is available.</summary>
            protected virtual void DoNewVersionDetails()
            {
                if (_Target._UpdateCheckFailureMessage != null)
                {
                    EditorGUILayout.HelpBox(_Target._UpdateCheckFailureMessage, MessageType.Info);
                    return;
                }

                if (_Target._LatestVersionName == null ||
                    _Target._LatestVersionChangeLogURL == null)
                    return;

                var message = _Target._NewVersionAvailable
                    ? $"{_Target._LatestVersionName} is now available.\nClick here to view the Change Log."
                    : $"{_Target.BaseProductName} is up to date.";

                EditorGUILayout.HelpBox(message, MessageType.Info);

                if (TryUseClickEventInLastRect())
                    Application.OpenURL(_Target._LatestVersionChangeLogURL);
            }

            /************************************************************************************************************************/

            /// <summary>Draws a toggle to disable automatic update checks.</summary>
            protected virtual void DoCheckForUpdates()
            {
#if UNITY_WEB_REQUEST
                if (string.IsNullOrEmpty(_Target.UpdateURL))
                    return;

                var area = GUILayoutUtility.GetRect(0, EditorGUIUtility.singleLineHeight);
                area.xMin += EditorGUIUtility.singleLineHeight * 0.2f;

                EditorGUI.BeginChangeCheck();
                var value = GUI.Toggle(area, _Target.CheckForUpdates, "Check For Updates");
                if (EditorGUI.EndChangeCheck())
                {
                    _Target.CheckForUpdates = value;
                    if (value)
                        _Target.StartCheckForUpdates();
                }
#endif
            }

            /************************************************************************************************************************/

            /// <summary>Draws a toggle to disable automatically selecting the <see cref="ReadMe"/> on startup.</summary>
            protected virtual void DoShowOnStartup()
            {
                var area = GUILayoutUtility.GetRect(0, EditorGUIUtility.singleLineHeight);
                area.xMin += EditorGUIUtility.singleLineHeight * 0.2f;

                var content = TempContent(_DontShowOnStartupProperty.displayName, _DontShowOnStartupProperty.tooltip);

                var label = EditorGUI.BeginProperty(area, content, _DontShowOnStartupProperty);
                EditorGUI.BeginChangeCheck();
                var value = _DontShowOnStartupProperty.boolValue;
                value = GUI.Toggle(area, value, label);
                if (EditorGUI.EndChangeCheck())
                {
                    _DontShowOnStartupProperty.boolValue = value;
                    if (value)
                        PlayerPrefs.SetInt(_ReleaseNumberPrefKey, _Target.ReleaseNumber);
                }
                EditorGUI.EndProperty();
            }

            /************************************************************************************************************************/

            /// <summary>Draws warnings about deleting older versions of the product.</summary>
            protected virtual void DoWarnings()
            {
                MessageType messageType;

                if (!_Target.HasCorrectName)
                {
                    messageType = MessageType.Error;
                }
                else if (_PreviousVersion >= 0 && _PreviousVersion < _Target.ReleaseNumber)
                {
                    messageType = MessageType.Warning;
                }
                else return;

                // Upgraded from any older version.

                DoSpace();

                var directory = AssetDatabase.GetAssetPath(_Target);
                if (string.IsNullOrEmpty(directory))
                    return;

                directory = Path.GetDirectoryName(directory);

                var productName = _Target.ProductName;

                string versionWarning;
                if (messageType == MessageType.Error)
                {
                    versionWarning =
                        $"You must fully delete any old version of {productName} before importing a new version." +
                        $"\n1. Check the Upgrade Guide in the Change Log." +
                        $"\n2. Click here to delete '{directory}'." +
                        $"\n3. Import {productName} again.";
                }
                else
                {
                    versionWarning =
                        $"You must fully delete any old version of {productName} before importing a new version." +
                        $"\n1. Ignore this message if you have already deleted the old version." +
                        $"\n2. Check the Upgrade Guide in the Change Log." +
                        $"\n3. Click here to delete '{directory}'." +
                        $"\n4. Import {productName} again.";
                }

                EditorGUILayout.HelpBox(versionWarning, messageType);
                CheckDeleteDirectory(directory);

                DoSpace();
            }

            /************************************************************************************************************************/

            /// <summary>Asks if the user wants to delete the `directory` and does so if they confirm.</summary>
            private void CheckDeleteDirectory(string directory)
            {
                if (!TryUseClickEventInLastRect())
                    return;

                var name = _Target.ProductName;

                if (!AssetDatabase.IsValidFolder(directory))
                {
                    Debug.Log($"{directory} doesn't exist." +
                        $" You must have moved {name} somewhere else so you will need to delete it manually.", this);
                    return;
                }

                if (!EditorUtility.DisplayDialog($"Delete {name}? ",
                    $"Would you like to delete {directory}?\n\nYou will then need to reimport {name} manually.",
                    "Delete", "Cancel"))
                    return;

                AssetDatabase.DeleteAsset(directory);
            }

            /************************************************************************************************************************/

            /// <summary>
            /// Returns true and uses the current event if it is <see cref="EventType.MouseUp"/> inside the specified
            /// `area`.
            /// </summary>
            public static bool TryUseClickEvent(Rect area, int button = -1)
            {
                var currentEvent = Event.current;
                if (currentEvent.type != EventType.MouseUp ||
                    (button >= 0 && currentEvent.button != button) ||
                    !area.Contains(currentEvent.mousePosition))
                    return false;

                GUI.changed = true;
                GUIUtility.hotControl = 0;
                currentEvent.Use();

                if (currentEvent.button == 2)
                    GUIUtility.keyboardControl = 0;

                return true;
            }

            /// <summary>
            /// Returns true and uses the current event if it is <see cref="EventType.MouseUp"/> inside the last GUI Layout
            /// <see cref="Rect"/> that was drawn.
            /// </summary>
            public static bool TryUseClickEventInLastRect(int button = -1)
                => TryUseClickEvent(GUILayoutUtility.GetLastRect(), button);

            /************************************************************************************************************************/

            protected virtual void DoIntroductionBlock()
            {
                GUILayout.BeginVertical(Styles.Block);

                DoHeadingLink("Documentation", null, _Target.DocumentationURL);

                DoSpace();

                DoHeadingLink("Change Log", null, _Target.ChangeLogURL, fontSize: GUI.skin.label.fontSize);

                GUILayout.EndVertical();
            }

            /************************************************************************************************************************/

            protected virtual void DoSampleBlock()
            {
                var label = _Target.SamplesLabel;
                if (string.IsNullOrEmpty(label))
                    return;

                GUILayout.BeginVertical(Styles.Block);

                DoHeadingLink(label, null, _Target.SamplesURL);

                if (_Samples != null)
                {
                    foreach (var sample in _Samples)
                    {
                        if (sample.isImported)
                        {
                            try
                            {
                                var path = Path.GetRelativePath(Environment.CurrentDirectory, sample.importPath);
                                var folder = AssetDatabase.LoadAssetAtPath<DefaultAsset>(path);
                                using (new EditorGUI.DisabledScope(true))
                                    EditorGUILayout.ObjectField(GUIContent.none, folder, typeof(DefaultAsset), false);
                            }
                            catch (Exception exception)
                            {
                                if (GUILayout.Button($"{sample.description}: {exception.GetType().Name}"))
                                    Debug.LogException(exception);
                            }
                        }
                        else
                        {
                            EditorGUILayout.LabelField(sample.displayName, "Not Imported");
                        }
                    }

                    var buttonContent = TempContent(
                        "Open Package Manager",
                        "Samples can be imported via the Samples tab in the Package Manager" +
                        "\n\nIt's generally recommended to delete any samples after you're done with them");
                    if (GUILayout.Button(buttonContent))
                        Window.Open("Animancer");
                }

                DoExtraSamples();

                GUILayout.EndVertical();
            }

            /************************************************************************************************************************/

            protected virtual void DoExtraSamples()
            {
                if (_Target.ExtraSamples == null)
                    return;

                for (int i = 0; i < _Target.ExtraSamples.Length; i++)
                {
                    if (i > 0)
                        DoSpace();

                    var section = _Target.ExtraSamples[i];
                    DoHeadingLink(
                        section.Heading,
                        section.Description,
                        section.URL,
                        section.DisplayURL,
                        GUI.skin.label.fontSize);
                }
            }

            /************************************************************************************************************************/

            protected virtual void DoSupportBlock()
            {
                GUILayout.BeginVertical(Styles.Block);

                for (int i = 0; i < _Target.LinkSections.Length; i++)
                {
                    if (i > 0)
                        DoSpace();

                    var section = _Target.LinkSections[i];
                    DoHeadingLink(
                        section.Heading,
                        section.Description,
                        section.URL,
                        section.DisplayURL);
                }

                GUILayout.EndVertical();
            }

            /************************************************************************************************************************/

            /// <summary>Draws a headding which acts as a button to open a URL.</summary>
            public static void DoHeadingLink(
                string heading,
                string description,
                string url,
                string displayURL = null,
                int fontSize = 22)
            {
                // Heading.
                var style = url == null
                    ? Styles.HeaderLabel
                    : Styles.HeaderLink;
                var area = DoLinkButton(heading, url, style, fontSize);

                // Description.

                area.y += EditorGUIUtility.standardVerticalSpacing;

                var urlHeight = Styles.URL.fontSize + Styles.URL.margin.vertical;
                area.height -= urlHeight;

                if (description != null)
                    GUI.Label(area, description, Styles.Description);

                // URL.

                area.y += area.height;
                area.height = urlHeight;

                displayURL ??= url;

                if (displayURL != null)
                {
                    var content = TempContent(displayURL, "Click to copy this link to the clipboard");

                    if (GUI.Button(area, content, Styles.URL))
                    {
                        GUIUtility.systemCopyBuffer = displayURL;
                        Debug.Log($"Copied '{displayURL}' to the clipboard.");
                    }

                    EditorGUIUtility.AddCursorRect(area, MouseCursor.Text);
                }
            }

            /************************************************************************************************************************/

            /// <summary>Draws a button to open a URL.</summary>
            public static Rect DoLinkButton(string text, string url, GUIStyle style, int fontSize = 22)
            {
                var content = TempContent(text, url);

                style.fontSize = fontSize;

                var size = style.CalcSize(content);
                var area = GUILayoutUtility.GetRect(0, size.y);

                var linkArea = new Rect(area.x, area.y, size.x, area.height);
                area.xMin += size.x;

                if (url == null)
                {
                    GUI.Label(linkArea, content, style);
                }
                else
                {
                    if (GUI.Button(linkArea, content, style))
                        Application.OpenURL(url);

                    EditorGUIUtility.AddCursorRect(linkArea, MouseCursor.Link);

                    DrawLine(
                        new(linkArea.xMin, linkArea.yMax),
                        new(linkArea.xMax, linkArea.yMax),
                        style.normal.textColor);
                }

                return area;
            }

            /************************************************************************************************************************/

            /// <summary>Draws a line between the `start` and `end` using the `color`.</summary>
            public static void DrawLine(Vector2 start, Vector2 end, Color color)
            {
                var previousColor = Handles.color;
                Handles.BeginGUI();
                Handles.color = color;
                Handles.DrawLine(start, end);
                Handles.color = previousColor;
                Handles.EndGUI();
            }

            /************************************************************************************************************************/

            /// <summary>Various <see cref="GUIStyle"/>s used by the <see cref="Editor"/>.</summary>
            protected static class Styles
            {
                /************************************************************************************************************************/

                public static readonly GUIStyle TitleArea = "In BigTitle";

                public static readonly GUIStyle Title = new(GUI.skin.label)
                {
                    fontSize = 26,
                };

                public static readonly GUIStyle Block = GUI.skin.box;

                public static readonly GUIStyle HeaderLabel = new(GUI.skin.label)
                {
                    stretchWidth = false,
                };

                public static readonly GUIStyle HeaderLink = new(HeaderLabel);

                public static readonly GUIStyle Description = new(GUI.skin.label)
                {
                    alignment = TextAnchor.LowerLeft,
                };

                public static readonly GUIStyle URL = new(GUI.skin.label)
                {
                    fontSize = 9,
                    alignment = TextAnchor.LowerLeft,
                };

                /************************************************************************************************************************/

                static Styles()
                {
                    HeaderLink.normal.textColor = HeaderLink.hover.textColor =
                        new Color32(0x00, 0x78, 0xDA, 0xFF);

                    URL.normal.textColor = Color.Lerp(URL.normal.textColor, Color.grey, 0.8f);
                }

                /************************************************************************************************************************/
            }

            /************************************************************************************************************************/
        }

        /************************************************************************************************************************/
        #endregion
        /************************************************************************************************************************/
    }
}

#endif

