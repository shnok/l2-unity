// Animancer // https://kybernetik.com.au/animancer // Copyright 2018-2025 Kybernetik //

#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using static Animancer.AnimancerEvent.Sequence.Serializable;
using Object = UnityEngine.Object;

namespace Animancer.Editor
{
    /// <summary>[Editor-Only]
    /// A system for migrating old asset data to the current version of Animancer.
    /// </summary>
    /// https://kybernetik.com.au/animancer/api/Animancer.Editor/AnimancerDataMigrator
    public class AnimancerDataMigrator
    {
        /************************************************************************************************************************/

        /// <summary>The directory where any newly created files will be saved.</summary>
        public const string
            MigratedDataDirectory = "Assets/Animancer Migrated Data";

        /************************************************************************************************************************/
        #region Entry Point
        /************************************************************************************************************************/

        [InitializeOnLoadMethod]
        private static void Initialize()
        {
            AnimancerReadMe.Editor.MigrateOldAssetData +=
                text => new AnimancerDataMigrator(text);
        }

        /************************************************************************************************************************/

        /// <summary>
        /// Shows dialogue windows explaining what the migration process will involve
        /// and confirming that the user wants to proceed before doing so.
        /// </summary>
        /// <remarks>
        /// <list type="number">
        /// <item>Save the scene if dirty.</item>
        /// <item>Make a backup.</item>
        /// </list>
        /// </remarks>
        public AnimancerDataMigrator(string functionName)
        {
            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                return;

            if (EditorApplication.isCompiling)
            {
                Debug.LogError(functionName + " failed: unable to migrate while scripts are compiling.");
                EditorApplication.Beep();
                return;
            }

            if (EditorUtility.scriptCompilationFailed)
            {
                Debug.LogError(functionName + " failed: all compile errors must be fixed first.");
                EditorApplication.Beep();
                return;
            }

            if (!AskForBackup(functionName))
                return;

            BeginMigration();
        }

        /************************************************************************************************************************/

        /// <summary>
        /// Explains what migration will involve
        /// and confirms that the user has backed up their project.
        /// </summary>
        private static bool AskForBackup(string functionName)
        {
            const string Message =
                "This process scans through your entire project" +
                " for assets containing data from an older version of Animancer" +
                " and attempts to convert that data to the format required by the current version." +
                "\n• This operation cannot be undone and may not be 100% reliable." +
                "\n• DO NOT proceed unless you have made a backup of your project" +
                " (Version Control such as Git is recommended)." +
                "\n• All files changed will be logged." +
                "\n• Please report any issues to " + Strings.DocsURLs.DeveloperEmail;

            var result = EditorUtility.DisplayDialogComplex(
                functionName,
                Message,
                "I have made a backup -> Migrate Data",
                "Cancel",
                "Open Upgrade Guide");

            switch (result)
            {
                case 0:
                    return true;

                default:
                case 1:
                    return false;

                case 2:
                    Application.OpenURL(Strings.DocsURLs.UpgradeGuideURL);
                    return false;
            }
        }

        /************************************************************************************************************************/

        private void BeginMigration()
            => MigrateData(GatherTargetFiles());

        /************************************************************************************************************************/

        private static List<string> GatherTargetFiles()
        {
            AssetDatabase.SaveAssets();

            var directory = Environment.CurrentDirectory;
            var targetFiles = new List<string>();

            GatherTargetFiles(Path.Combine(directory, "Assets"), targetFiles);
            GatherTargetFiles(Path.Combine(directory, "Packages"), targetFiles);

            return targetFiles;
        }

        private static void GatherTargetFiles(string directory, List<string> targetFiles)
        {
            var allFiles = Directory.GetFiles(directory, "*", SearchOption.AllDirectories);
            for (int i = 0; i < allFiles.Length; i++)
            {
                var file = allFiles[i];
                if (file.EndsWith(".asset", StringComparison.OrdinalIgnoreCase) ||
                    file.EndsWith(".prefab", StringComparison.OrdinalIgnoreCase) ||
                    file.EndsWith(".unity", StringComparison.OrdinalIgnoreCase))
                    targetFiles.Add(file);
            }
        }

        /************************************************************************************************************************/

        private void MigrateData(List<string> files)
        {
            try
            {
                Debug.Log("Data Migration Started");

                var modifiedFileCount = 0;
                var timer = SimpleTimer.Start();

                for (int i = 0; i < files.Count; i++)
                {
                    var file = files[i];

                    if (ShowProgressBar(i, files.Count, file))
                        return;

                    if (MigrateData(file))
                        modifiedFileCount++;
                }

                Debug.Log(
                    $"Data Migration Complete." +
                    $" Modified {modifiedFileCount} files in {timer}." +
                    $" Please check the modified files and report any issues to {Strings.DocsURLs.DeveloperEmail}");

                WarnAboutUnSharedChanges();

                AssetDatabase.Refresh();
            }
            finally
            {
                EditorUtility.ClearProgressBar();
            }
        }

        /************************************************************************************************************************/

        private static bool ShowProgressBar(int index, int count, string currentFile)
            => EditorUtility.DisplayCancelableProgressBar(
                "Migrating Asset Data",
                $"{index} / {count}: {currentFile}",
                index / (float)count);

        /************************************************************************************************************************/

        private bool MigrateData(string filePath)
        {
            try
            {
                var originalText = File.ReadAllText(filePath);
                var modifiedText = originalText;

                MigrateSerializedReferences(ref modifiedText);
                MigrateEvents(ref modifiedText);
                MigrateUnSharedTransitionAssets(filePath, ref modifiedText);
                MigrateTransitionAssets(ref modifiedText);

                if (!ReferenceEquals(originalText, modifiedText))
                {
                    File.WriteAllText(filePath, modifiedText);

                    filePath = GetRelativeFilePath(filePath);

                    AssetDatabase.ImportAsset(filePath, ImportAssetOptions.ForceSynchronousImport);
                    var asset = AssetDatabase.LoadAssetAtPath<Object>(filePath);

                    Debug.Log($"Migrated: {filePath}", asset);

                    return true;
                }
            }
            catch (Exception exception)
            {
                Debug.LogException(exception);
            }

            return false;
        }

        /************************************************************************************************************************/
        #endregion
        /************************************************************************************************************************/
        #region Serialized References
        /************************************************************************************************************************/

        /// <summary>[Animancer v8.0]
        /// Animancer assemblies renamed when moving to a package.
        /// </summary>
        private static void MigrateSerializedReferences(ref string fileText)
        {
            fileText = fileText.Replace(
                ", ns: Animancer, asm: Animancer}",
                ", ns: Animancer, asm: Kybernetik.Animancer}");
        }

        /************************************************************************************************************************/
        #endregion
        /************************************************************************************************************************/
        #region Events
        /************************************************************************************************************************/

        /// <summary>[Animancer v8.0]
        /// Event callbacks changed from <see cref="UnityEvent"/> to <see cref="IInvokable"/>.
        /// Event names changed from <see cref="string"/> to <see cref="StringAsset"/>.
        /// </summary>
        private static void MigrateEvents(ref string text)
        {
            const string Prefix = "_" + nameof(IHasEvents.Events) + ":";

            var index = 0;
            while ((index = text.IndexOf(Prefix, index)) >= 0)
            {
                var callbacks = IndexOfEventField(text, index, CallbacksField);
                if (callbacks >= 0)
                    MigrateEventCallbacks(ref text, callbacks);

                var names = IndexOfEventField(text, index, NamesField);
                if (names >= 0)
                    MigrateEventNames(ref text, names);

                index += Prefix.Length;
            }
        }

        /************************************************************************************************************************/

        /// <summary>Finds the indices of the serialized event fields and returns true if successful.</summary>
        private static int IndexOfEventField(
            string text,
            int start,
            string fieldName)
        {
            var baseIndentation = CountConsecutiveCharacters(text, start - 1, -1, ' ');

            while (true)
            {
                start = text.IndexOf('\n', start);
                if (start < 0)
                    break;

                start++;

                var lineIndentation = CountConsecutiveCharacters(text, start, 1, ' ');
                if (lineIndentation <= baseIndentation)
                    break;

                start += lineIndentation;
                if (text[start] != '_')
                    continue;

                if (SubstringEquals(text, start, fieldName + ":"))
                    return start;
            }

            return -1;
        }

        /************************************************************************************************************************/
        #region Event Names
        /************************************************************************************************************************/

        /// <summary>[Animancer v8.0]
        /// Event names changed from <see cref="string"/> to <see cref="StringAsset"/>.
        /// </summary>
        private static void MigrateEventNames(ref string text, int start)
        {
            start = text.IndexOf('\n', start);
            if (start < 0)
                return;

            start++;

            while (start < text.Length)
            {
                var end = text.IndexOf('\n', start + 1);
                if (start < 0)
                    end = text.Length;

                start += CountConsecutiveCharacters(text, start, 1, ' ');

                var character = text[start];
                if (character != '-')
                    return;

                start++;

                // If not already migrated.
                if (!SubstringEquals(text, start, " {fileID: "))
                {
                    var name = text[start..end];
                    name = name.Trim();

                    start = text.IndexOf(name, start);
                    MigrateEventName(ref text, start, name, out end);
                }

                start = end + 1;
            }
        }

        /************************************************************************************************************************/

        private static void MigrateEventName(ref string text, int start, string name, out int end)
        {
            StringAsset.FindOrCreate(name, MigratedDataDirectory, out var path);

            var fileID = GetFileID(path);
            var guid = AssetDatabase.AssetPathToGUID(path);

            end = start + name.Length;

            var before = text[..start];
            var after = text[end..];
            var newReference = $"{{fileID: {fileID}, guid: {guid}, type: 2}}";

            end = start + newReference.Length;

            text = $"{before}{newReference}{after}";
        }

        /************************************************************************************************************************/

        private static string GetFileID(string assetPath)
        {
            assetPath += ".meta";
            if (!File.Exists(assetPath))
                return null;

            var meta = File.ReadAllText(assetPath);

            const string Prefix = "mainObjectFileID: ";
            var start = meta.IndexOf(Prefix, StringComparison.Ordinal);
            if (start < 0)
                return null;

            start += Prefix.Length;

            var end = meta.IndexOf('\n', start);
            if (end < 0)
                end = meta.Length;

            var id = meta[start..end];
            id = id.Trim();
            return id;
        }

        /************************************************************************************************************************/
        #endregion
        /************************************************************************************************************************/
        #region Event Callbacks
        /************************************************************************************************************************/

        /// <summary>[Animancer v8.0]
        /// Event callbacks changed from <see cref="UnityEvent"/> to <see cref="IInvokable"/>.
        /// </summary>
        private static void MigrateEventCallbacks(ref string text, int start)
        {
            start = text.IndexOf('\n', start);
            if (start < 0)
                return;

            start++;

            while (start < text.Length)
            {
                var end = text.IndexOf('\n', start + 1);
                if (start < 0)
                    end = text.Length;

                var indentation = CountConsecutiveCharacters(text, start, 1, ' ');

                start += indentation;

                var character = text[start];
                if (character != '-')
                    return;

                start++;

                // If already migrated, skip over it.
                if (SubstringEquals(text, start, " rid:"))
                {
                    start = end;
                }
                else// Otherwise, turn it into a serialized reference.
                {
                    var callback = ReIndentFieldData(text, start, indentation, 6, out end);
                    var id = RandomLong();

                    var newText = StringBuilderPool.Instance.Acquire();
                    newText.Append(text, 0, start - indentation - 1);
                    newText.Append(' ', indentation);
                    newText.Append("- rid: ");
                    newText.Append(id);
                    newText.Append('\n');

                    start = newText.Length;

                    newText.Append(text, end, text.Length - end);

                    text = newText.ReleaseToString();

                    AddSerializedReference(ref text, start, id, typeof(Animancer.UnityEvent), callback);
                }
            }
        }

        /************************************************************************************************************************/

        private static string ReIndentFieldData(
            string text,
            int start,
            int oldIndentation,
            int newIndentation,
            out int end)
        {
            var callback = StringBuilderPool.Instance.Acquire();

            end = start;

            while (start < text.Length)
            {
                var indentation = CountConsecutiveCharacters(text, start, 1, ' ');

                // Remove the dash from the start of the first line.
                if (callback.Length == 0)
                {
                    start += indentation;
                    indentation = newIndentation + 2;
                }
                else// Adjust the indentation of other lines.
                {
                    if (indentation <= oldIndentation)
                    {
                        end = start;
                        break;
                    }

                    start += indentation;

                    indentation = indentation - oldIndentation + newIndentation;
                }

                end = text.IndexOf('\n', start);
                if (end < 0)
                    end = text.Length;

                callback.Append(' ', indentation);
                callback.Append(text, start, end - start);
                callback.Append('\n');

                start = end + 1;
            }

            return callback.ReleaseToString();
        }

        /************************************************************************************************************************/
        #endregion
        /************************************************************************************************************************/
        #endregion
        /************************************************************************************************************************/
        #region UnShared Transition Assets
        /************************************************************************************************************************/

        private List<string> _UnSharedFilePaths;

        /************************************************************************************************************************/

        /// <summary>[Animancer v8.0]
        /// UnShared classes removed,
        /// data replaced with direct <see cref="TransitionAssetBase"/> references.
        /// </summary>
        private void MigrateUnSharedTransitionAssets(string filePath, ref string fileText)
        {
            var isChanged = false;

            var start = 0;
            while (start < fileText.Length)
            {
                const string
                    AssetReferencePrefix = "    _Asset: ",
                    FileReferencePrefix = "{fileID: ",
                    GuidPrefix = ", guid: ",
                    TypePrefix = ", type: ";

                start = fileText.IndexOf(AssetReferencePrefix + FileReferencePrefix, start, StringComparison.Ordinal);
                if (start < 0)
                    break;

                var end = fileText.IndexOf(
                    '\n',
                    start + AssetReferencePrefix.Length + FileReferencePrefix.Length);
                if (end < 0)
                    end = fileText.Length;

                var guid = fileText.IndexOf(
                    GuidPrefix,
                    start + AssetReferencePrefix.Length,
                    end - (start + AssetReferencePrefix.Length),
                    StringComparison.Ordinal);
                if (guid < 0)
                    break;

                var type = fileText.IndexOf(
                    TypePrefix,
                    guid + GuidPrefix.Length,
                    end - (guid + GuidPrefix.Length),
                    StringComparison.Ordinal);
                if (type < 0)
                    break;

                var guidReference = fileText[(guid + GuidPrefix.Length)..type];

                var assetPath = AssetDatabase.GUIDToAssetPath(guidReference);

                if (string.IsNullOrEmpty(assetPath) ||
                    AssetDatabase.LoadAssetAtPath<TransitionAssetBase>(assetPath) == null)
                {
                    start++;
                    continue;
                }

                var indent = CountConsecutiveCharacters(fileText, start, -1, ' ');

                var reference = fileText[(start + AssetReferencePrefix.Length)..end];

                fileText = $"{fileText[..(start - indent)]} {reference}{fileText[end..]}";

                if (!isChanged)
                {
                    isChanged = true;
                    _UnSharedFilePaths ??= new();
                    _UnSharedFilePaths.Add(GetRelativeFilePath(filePath));
                }

                start += reference.Length;
            }
        }

        /************************************************************************************************************************/

        private void WarnAboutUnSharedChanges()
        {
            if (_UnSharedFilePaths == null)
                return;

            var text = StringBuilderPool.Instance.Acquire();

            AppendUnSharedFilePaths(text, int.MaxValue);

            var message = text.ToString();

            Debug.LogWarning(message + '\n');

            if (_UnSharedFilePaths.Count > 10)
            {
                text.Length = 0;
                AppendUnSharedFilePaths(text, 10);
                message = text.ToString();
            }

            EditorUtility.DisplayDialog(
                "UnShared Transition Assets",
                message + "\n\nThis warning will be logged.",
                "I will review the listed files");

            StringBuilderPool.Instance.Release(text);
        }

        private void AppendUnSharedFilePaths(StringBuilder text, int maxCount)
        {
            text.Append("Data resembling UnShared Transition Assets has been detected in the following files:");

            var count = Math.Min(maxCount, _UnSharedFilePaths.Count);
            for (int i = 0; i < count; i++)
            {
                text.Append("\n• ")
                    .Append(_UnSharedFilePaths[i]);
            }

            if (count < _UnSharedFilePaths.Count)
                text.Append("\n• And ")
                    .Append(_UnSharedFilePaths.Count - count)
                    .Append(" more (see the log).");

            text.Append(
                "\n\nThe UnShared system has been removed so this data has been modified" +
                " to the format of a direct reference to a Transition Asset." +
                $"\n\nFor example, if you had a ClipTransitionAsset.UnShared field" +
                $" and you changed it to a direct ClipTransitionAsset" +
                $" then this modification should allow the field to keep its referenced asset." +
                $"\n\nThis may have falsely identified unrelated data which looks similar" +
                $" so please review the listed files to check your references.");
        }

        /************************************************************************************************************************/
        #endregion
        /************************************************************************************************************************/
        #region Transition Assets
        /************************************************************************************************************************/

        private const string AnimancerTransitionAssetGUID = "c5a8877f26e7a6a43aaf06fade1a064a";

        private static readonly string[] RemovedTransitionAssetGUIDs =
        {
            "65baa284d24adb24b90b39482364509c",// ClipTransitionAsset.
            "6cb514e38f7bd084383a7355a6273a33",// ControllerTransitionAsset.
            "15ed7dccdb910ec4896f03f7cc2ede35",// Float1ControllerTransitionAsset.
            "f283fbe305b90dc489b5dec987ecad09",// Float2ControllerTransitionAsset.
            "435c1d26374dbbb498fa45fe394dfe10",// Float3ControllerTransitionAsset.
            "a472a4806da7f9147a4a5cd7eee170df",// LinearMixerTransitionAsset.
            "9785324053a1e39449b4e579bfb71f76",// ManualMixerTransitionAsset.
            "8a2d01d4f425b3848938f199392c9afb",// MixerTransition2DAsset.
            "5415cf2115901c345af7680b044d4604",// PlayableAssetTransitionAsset.
        };

        private void MigrateTransitionAssets(ref string fileText)
        {
            const string
                ScriptReferencePrefix = "m_Script: {fileID: 11500000, guid: ",
                ScriptReferenceSuffix = ", type: 3}";

            var index = 0;

            while (index < fileText.Length)
            {
                index = fileText.IndexOf(ScriptReferencePrefix, index);
                if (index < 0)
                    return;

                index += ScriptReferencePrefix.Length;

                var guidLength = AnimancerTransitionAssetGUID.Length;
                var guidEnd = index + guidLength;
                if (guidEnd >= fileText.Length)
                    return;

                for (int i = 0; i < RemovedTransitionAssetGUIDs.Length; i++)
                {
                    if (string.CompareOrdinal(fileText, index, RemovedTransitionAssetGUIDs[i], 0, guidLength) == 0)
                    {
                        fileText = $"{fileText[..index]}{AnimancerTransitionAssetGUID}{fileText[guidEnd..]}";
                        break;
                    }
                }

                index = guidEnd + ScriptReferenceSuffix.Length;
            }
        }

        /************************************************************************************************************************/

        #endregion
        /************************************************************************************************************************/
        #region Utilities
        /************************************************************************************************************************/

        private static int CountConsecutiveCharacters(
            string text,
            int start,
            int direction,
            char character)
        {
            var count = 0;

            while (start >= 0 && start < text.Length)
            {
                if (text[start] == character)
                {
                    count++;
                    start += direction;
                }
                else
                {
                    break;
                }
            }

            return count;
        }

        /************************************************************************************************************************/

        private static bool SubstringEquals(
            string text,
            int start,
            string substring)
            => string.CompareOrdinal(
                text,
                start,
                substring,
                0,
                substring.Length) == 0;

        /************************************************************************************************************************/

        private static readonly System.Random
            Random = new();

        private static long RandomLong()
            => (uint)Random.Next() | ((long)Random.Next() << 32);

        /************************************************************************************************************************/

        private static void AddSerializedReference(
            ref string text,
            int index,
            long id,
            Type type,
            string reference)
        {
            const string ObjectHeader = "\n--- !u!";

            var start = text.LastIndexOf(ObjectHeader, index) + 1;

            var end = text.IndexOf(ObjectHeader, start) + 1;
            if (end <= 0)
                end = text.Length;

            var newText = StringBuilderPool.Instance.Acquire();

            newText.Append(text, 0, end);

            if (text[^1] != '\n')
                newText.Append('\n');

            const string ReferencesHeader = "  references:\n";
            const string VersionHeader = "    version: 2\n";

            // If the references block is missing, add it.
            var referencesIndex = text.LastIndexOf("\n" + ReferencesHeader, end, end - start);
            if (referencesIndex < 0)
            {
                newText.Append(ReferencesHeader + VersionHeader + "    RefIds:\n");
            }
            else
            {
                // Or if it exists but has the wrong version, complain.
                if (!SubstringEquals(text, referencesIndex + ReferencesHeader.Length + 1, VersionHeader))
                    Debug.LogWarning("Unknown serialized reference format. Expected version 2.");
            }

            newText.Append("    - rid: ");
            newText.Append(id);

            newText.Append("\n      type: {class: ");
            newText.Append(type.Name);
            newText.Append(", ns: ");
            newText.Append(type.Namespace);
            newText.Append(", asm: ");
            newText.Append(type.Assembly.GetName().Name);
            newText.Append("}\n");

            newText.Append("      data:\n");
            newText.Append(reference);

            newText.Append(text, end, text.Length - end);

            text = newText.ReleaseToString();
        }

        /************************************************************************************************************************/

        private static string GetRelativeFilePath(string filePath)
        {
            var currentDirectory = Environment.CurrentDirectory;
            if (filePath.StartsWith(currentDirectory))
                filePath = filePath[(currentDirectory.Length + 1)..];

            return filePath;
        }

        /************************************************************************************************************************/
        #endregion
        /************************************************************************************************************************/
        #region Example Data
        /************************************************************************************************************************/
        // Old Transition Asset Example
        /************************************************************************************************************************/

        /*
%YAML 1.1
%TAG !u! tag:unity3d.com,2011:
--- !u!114 &11400000
MonoBehaviour:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {fileID: 0}
  m_PrefabInstance: {fileID: 0}
  m_PrefabAsset: {fileID: 0}
  m_GameObject: {fileID: 0}
  m_Enabled: 1
  m_EditorHideFlags: 0
  m_Script: {fileID: 11500000, guid: 65baa284d24adb24b90b39482364509c, type: 3}
  m_Name: New Clip Transition Asset
  m_EditorClassIdentifier: 
  _Transition:
    rid: 2481247569047191554
  references:
    version: 2
    RefIds:
    - rid: 2481247569047191554
      type: {class: ClipTransition, ns: Animancer, asm: Animancer}
      data:
        _FadeDuration: 0.25
        _Events:
          _NormalizedTimes:
          - 0.653912
          - NaN
          _Callbacks:
          - m_PersistentCalls:
              m_Calls:
              - m_Target: {fileID: 11400000}
                m_TargetAssemblyTypeName: UnityEngine.Object, UnityEngine
                m_MethodName: set_name
                m_Mode: 5
                m_Arguments:
                  m_ObjectArgument: {fileID: 0}
                  m_ObjectArgumentAssemblyTypeName: UnityEngine.Object, UnityEngine
                  m_IntArgument: 0
                  m_FloatArgument: 0
                  m_StringArgument: 
                  m_BoolArgument: 0
                m_CallState: 2
              - m_Target: {fileID: 11400000}
                m_TargetAssemblyTypeName: UnityEngine.Object, UnityEngine
                m_MethodName: set_name
                m_Mode: 5
                m_Arguments:
                  m_ObjectArgument: {fileID: 0}
                  m_ObjectArgumentAssemblyTypeName: UnityEngine.Object, UnityEngine
                  m_IntArgument: 0
                  m_FloatArgument: 0
                  m_StringArgument: SecondCall
                  m_BoolArgument: 0
                m_CallState: 2
          - m_PersistentCalls:
              m_Calls:
              - m_Target: {fileID: 11400000}
                m_TargetAssemblyTypeName: UnityEngine.Object, UnityEngine
                m_MethodName: set_name
                m_Mode: 5
                m_Arguments:
                  m_ObjectArgument: {fileID: 0}
                  m_ObjectArgumentAssemblyTypeName: UnityEngine.Object, UnityEngine
                  m_IntArgument: 0
                  m_FloatArgument: 0
                  m_StringArgument: Ended
                  m_BoolArgument: 0
                m_CallState: 2
          _Names:
          - Fire
        _Clip: {fileID: 7400000, guid: c9ffd254c8ebfae4fbe97ac489238d8d, type: 2}
        _Speed: 1
        _NormalizedStartTime: 0
         */

        /************************************************************************************************************************/
        // New Transition Asset Example
        /************************************************************************************************************************/

        /*
%YAML 1.1
%TAG !u! tag:unity3d.com,2011:
--- !u!114 &11400000
MonoBehaviour:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {fileID: 0}
  m_PrefabInstance: {fileID: 0}
  m_PrefabAsset: {fileID: 0}
  m_GameObject: {fileID: 0}
  m_Enabled: 1
  m_EditorHideFlags: 0
  m_Script: {fileID: 11500000, guid: 65baa284d24adb24b90b39482364509c, type: 3}
  m_Name: New Clip Transition Asset
  m_EditorClassIdentifier: 
  _Transition:
    rid: 2481247569047191554
  references:
    version: 2
    RefIds:
    - rid: 2481247569047191554
      type: {class: ClipTransition, ns: Animancer, asm: Kybernetik.Animancer}
      data:
        _FadeDuration: 0.25
        _Speed: 1
        _Events:
          _NormalizedTimes:
          - 0.653912
          - NaN
          _Callbacks:
          - rid: 2481247573131657217
          - rid: 2481247573131657219
          _Names:
          - {fileID: 11400000, guid: f0b72c8b47f88b2449a2300915f08c72, type: 2}
        _Clip: {fileID: 7400000, guid: c9ffd254c8ebfae4fbe97ac489238d8d, type: 2}
        _NormalizedStartTime: 0
    - rid: 2481247573131657217
      type: {class: UnityEvent, ns: Animancer, asm: Kybernetik.Animancer}
      data:
        m_PersistentCalls:
          m_Calls:
          - m_Target: {fileID: 11400000}
            m_TargetAssemblyTypeName: UnityEngine.Object, UnityEngine
            m_MethodName: set_name
            m_Mode: 5
            m_Arguments:
              m_ObjectArgument: {fileID: 0}
              m_ObjectArgumentAssemblyTypeName: UnityEngine.Object, UnityEngine
              m_IntArgument: 0
              m_FloatArgument: 0
              m_StringArgument: 
              m_BoolArgument: 0
            m_CallState: 2
          - m_Target: {fileID: 11400000}
            m_TargetAssemblyTypeName: UnityEngine.Object, UnityEngine
            m_MethodName: set_name
            m_Mode: 5
            m_Arguments:
              m_ObjectArgument: {fileID: 0}
              m_ObjectArgumentAssemblyTypeName: UnityEngine.Object, UnityEngine
              m_IntArgument: 0
              m_FloatArgument: 0
              m_StringArgument: SecondCall
              m_BoolArgument: 0
            m_CallState: 2
    - rid: 2481247573131657219
      type: {class: UnityEvent, ns: Animancer, asm: Kybernetik.Animancer}
      data:
        m_PersistentCalls:
          m_Calls:
          - m_Target: {fileID: 11400000}
            m_TargetAssemblyTypeName: UnityEngine.Object, UnityEngine
            m_MethodName: set_name
            m_Mode: 5
            m_Arguments:
              m_ObjectArgument: {fileID: 0}
              m_ObjectArgumentAssemblyTypeName: UnityEngine.Object, UnityEngine
              m_IntArgument: 0
              m_FloatArgument: 0
              m_StringArgument: Ended
              m_BoolArgument: 0
            m_CallState: 2
        */

        /************************************************************************************************************************/
        #endregion
        /************************************************************************************************************************/
    }
}

#endif

