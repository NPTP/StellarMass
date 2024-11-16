using System;
using System.Collections.Generic;
using System.IO;
using Summoner.Editor.CustomBuildUtilities.Enums;
using Summoner.Utilities.Editor;
using Summoner.Utilities.Extensions;
using UnityEditor;
using UnityEngine;

namespace Summoner.Editor.CustomBuildUtilities
{
    public partial class CustomBuildWindow : EditorWindow
    {
        private const string GAME_TITLE_COLOR = "#FF3131";
        private const string BUILD_BUTTON_TEXT = "BUILD";
        private const string BATCH_BUILD_BUTTON_TEXT = "Batch BUILD";
        private const string DEFAULT_BUILD_PATH = "Builds";
        private const string GAME_TITLE = "SUMMONER";
        private const string PRESET_NAME_FIELD_TITLE = "Preset Name";
        
        private static GUIStyle HeaderStyle => new GUIStyle(GUI.skin.label) { fontSize = 24, fontStyle = FontStyle.Bold, alignment = TextAnchor.MiddleCenter, richText = true};
        private static Color BuildButtonColor => new Color(0, 0.5f, 0);
        private static Vector2 BuildButtonSize => new Vector2(150, 60);
        private static string DefaultExecutableName => Application.productName;
        private static GUILayoutOption BatchBuildColumnWidth => GUILayout.Width(EditorGUIUtility.currentViewWidth / 3 - 10f); 
        
        private string buildPresetsPath;
        private bool batchBuildPresets;
        private CustomBuildOptions currentOptions;

        [MenuItem(EditorToolNames.CUSTOM_BUILD_WINDOW, isValidateFunction: false, priority: 9999)]
        public static void ShowWindow()
        { 
            if (GetWindow(typeof(CustomBuildWindow)) is CustomBuildWindow customBuildWindow)
            {
                customBuildWindow.titleContent = new GUIContent(ToInspectorFieldName(nameof(CustomBuildWindow)));
                customBuildWindow.Show();
            }
        }

        private void OnGUI()
        {
            ShowTitle();
            
            EditorInspectorUtility.DrawHorizontalLine();
            
            EditorGUI.BeginDisabledGroup(Application.isPlaying);
            
            ShowVersion();
            ShowPresetSection();

            EditorInspectorUtility.DrawHorizontalLine();
            
            if (batchBuildPresets)
            {
                ShowBatchBuildSection();
            }
            else
            {
                ShowStandardBuildOptions();
                EditorGUILayout.Space(10);
                ShowSpecialBuildOptions();
                GUILayout.Space(10);
                ShowScriptingDefinesSection();
            }
            
            EditorInspectorUtility.DrawHorizontalLine();
            
            ShowBuildButton();
            
            EditorGUI.EndDisabledGroup();
        }

        private void ShowTitle()
        {
            GUILayout.Label($"<color={GAME_TITLE_COLOR}><b>{GAME_TITLE}</b></color> Builder", HeaderStyle);
        }
        
        private static void ShowVersion()
        {
            PlayerSettings.bundleVersion = EditorGUILayout.TextField(new GUIContent("Version", "Changes the application version in the Player settings."), Application.version);
        }
        
        private void ShowPresetSection()
        {
            BoolField(ref batchBuildPresets, nameof(batchBuildPresets));
            StringField(ref buildPresetsPath, nameof(buildPresetsPath), string.Empty);
            
            if (batchBuildPresets)
            {
                EditorGUILayout.Space(5);
                EditorGUILayout.LabelField("Presets cannot be modified/saved/loaded when batch build is checked.", EditorStyles.helpBox);
            }
        
            if (!batchBuildPresets)
            {
                EditorGUI.BeginDisabledGroup(true);
                EditorGUILayout.TextField(new GUIContent(PRESET_NAME_FIELD_TITLE), GetCurrentPresetNameFromEditorPrefs());
                EditorGUI.EndDisabledGroup();
                
                GUILayout.BeginHorizontal();
                if (GUILayout.Button("Load Preset") && TryLoadPreset(buildPresetsPath, out CustomBuildOptions preset))
                {
                    GUI.FocusControl(null);
                    SaveCurrentPresetNameToEditorPrefs(preset.name);
                    currentOptions = preset;
                    SaveCustomBuildOptionsToEditorPrefs(currentOptions);
                }
        
                if (GUILayout.Button("Save Preset") && TrySavePreset(currentOptions, out string savedName))
                {
                    SaveCurrentPresetNameToEditorPrefs(savedName);
                }
        
                GUILayout.EndHorizontal();
            }
        }
        private void ShowBatchBuildSection()
        {
            GUILayout.BeginHorizontal();
            EditorGUILayout.Space();

            GUILayout.FlexibleSpace();
            EditorGUILayout.LabelField("Preset Name", EditorStyles.boldLabel, BatchBuildColumnWidth, BatchBuildColumnWidth);
            EditorGUILayout.LabelField("Include in Batch", EditorStyles.boldLabel, BatchBuildColumnWidth);
            EditorGUILayout.LabelField("Build Path", EditorStyles.boldLabel, BatchBuildColumnWidth);
            
            EditorGUILayout.Space();
            GUILayout.EndHorizontal();

            CustomBuildOptions[] presets = LoadAllPresets();
            foreach (CustomBuildOptions preset in presets)
            {
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();

                // Preset name
                EditorGUILayout.LabelField(preset.name, BatchBuildColumnWidth, BatchBuildColumnWidth);
        
                // Include in batch
                bool includeInBatchSavedValue = GetIncludeInBatchValueFromEditorPrefs(preset.name);
                bool includeInBatchNewValue = EditorGUILayout.Toggle(includeInBatchSavedValue, BatchBuildColumnWidth);
                SaveIncludeInBatchValueToEditorPrefs(preset.name, includeInBatchNewValue);

                // Build path
                string presetBuildPathSavedValue = GetPresetBuildPathValueFromEditorPrefs(preset.name);
                string presetBuildPathNewValue = EditorGUILayout.TextField(presetBuildPathSavedValue, BatchBuildColumnWidth);
                SavePresetBuildPathValueToEditorPrefs(preset.name, presetBuildPathNewValue);
                
                GUILayout.EndHorizontal();
            }
        }
        
        private void ShowStandardBuildOptions()
        {
            StringField(ref currentOptions.buildPath, nameof(currentOptions.buildPath), defaultValue: DEFAULT_BUILD_PATH);
            StringField(ref currentOptions.executableName, nameof(currentOptions.executableName), DefaultExecutableName);
            EnumField(ref currentOptions.platform, nameof(currentOptions.platform), defaultValue: (int)Platform.StandaloneWindows64);
            BoolField(ref currentOptions.developmentBuild, nameof(currentOptions.developmentBuild));
            EditorGUI.BeginDisabledGroup(!currentOptions.developmentBuild);
            BoolField(ref currentOptions.autoconnectProfiler, nameof(currentOptions.autoconnectProfiler));
            BoolField(ref currentOptions.deepProfilingSupport, nameof(currentOptions.deepProfilingSupport));
            BoolField(ref currentOptions.scriptDebugging, nameof(currentOptions.scriptDebugging));
            EditorGUI.EndDisabledGroup();
            EnumField(ref currentOptions.compressionMethod, nameof(currentOptions.compressionMethod));
        }
        
        private void ShowSpecialBuildOptions()
        {
            EnumField(ref currentOptions.branch, nameof(currentOptions.branch));
            EnumField(ref currentOptions.store, nameof(currentOptions.store));
            BoolField(ref currentOptions.preprocessBuild, nameof(currentOptions.preprocessBuild), defaultValue: true);
            EnumField(ref currentOptions.afterBuild, nameof(currentOptions.afterBuild));
        }
        
        private void ShowScriptingDefinesSection()
        {
            EditorGUILayout.LabelField("Scripting Defines", EditorStyles.boldLabel);
            int scriptingDefinesCount = EditorPrefs.GetInt(GetEditorPrefsKey($"{nameof(currentOptions.extraScriptingDefines)}.Count"), defaultValue: 0);
        
            // Non-interactable group of scripting defines set by enum value choices.
            GUI.enabled = false;
            foreach (string scriptingDefine in currentOptions.ForcedScriptingDefines)
                EditorGUILayout.TextField(scriptingDefine);
        
            GUI.enabled = true;
        
            if (currentOptions.extraScriptingDefines == null)
                currentOptions.extraScriptingDefines = new List<string>();
            currentOptions.extraScriptingDefines.Clear();
            
            for (int i = 0; i < scriptingDefinesCount; i++)
            {
                currentOptions.extraScriptingDefines.Add(string.Empty);
                string value = string.Empty;
                ListStringField(ref value, $"{nameof(currentOptions.extraScriptingDefines)}[{i}]");
                currentOptions.extraScriptingDefines[i] = value;
            }
        
            GUILayout.BeginHorizontal();
            EditorGUI.BeginDisabledGroup(currentOptions.extraScriptingDefines.Count == 0);
            if (GUILayout.Button("-"))
            {
                currentOptions.extraScriptingDefines.RemoveAt(currentOptions.extraScriptingDefines.Count - 1);
            }
            EditorGUI.EndDisabledGroup();
            if (GUILayout.Button("+"))
            {
                currentOptions.extraScriptingDefines.Add(string.Empty);
            }
            GUILayout.EndHorizontal();
            EditorPrefs.SetInt(GetEditorPrefsKey($"{nameof(currentOptions.extraScriptingDefines)}.Count"),
                currentOptions.extraScriptingDefines.Count);
        }
        
        private void ShowBuildButton()
        {
            EditorGUI.BeginDisabledGroup(Application.isPlaying);
            
            GUIStyle buttonStyle = new GUIStyle(GUI.skin.button)
            {
                normal = { textColor = Color.white, background = makeTexture2D(1, 1, BuildButtonColor) },
                fontSize = 18,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter
            };
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            string buttonText = batchBuildPresets ? BATCH_BUILD_BUTTON_TEXT : BUILD_BUTTON_TEXT;
            if (GUILayout.Button(buttonText, buttonStyle, GUILayout.Width(BuildButtonSize.x), GUILayout.Height(BuildButtonSize.y)))
            {
                if (batchBuildPresets)
                    BatchBuild();
                else
                    Build(currentOptions);
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            
            EditorGUI.EndDisabledGroup();
            
            Texture2D makeTexture2D(int width, int height, Color col)
            {
                Color[] pix = new Color[width * height];
                for (int i = 0; i < pix.Length; i++)
                    pix[i] = col;
        
                Texture2D result = new Texture2D(width, height);
                result.SetPixels(pix);
                result.Apply();
                return result;
            }
        }
        
        private void SaveCustomBuildOptionsToEditorPrefs(CustomBuildOptions customBuildOptions)
        {
            SaveStringToEditorPrefs(customBuildOptions.buildPath, nameof(customBuildOptions.buildPath));
            SaveStringToEditorPrefs(customBuildOptions.executableName, nameof(customBuildOptions.executableName));
            SaveEnumToEditorPrefs(customBuildOptions.platform, nameof(customBuildOptions.platform));
            SaveBoolToEditorPrefs(customBuildOptions.developmentBuild, nameof(customBuildOptions.developmentBuild));
            SaveBoolToEditorPrefs(customBuildOptions.autoconnectProfiler, nameof(customBuildOptions.autoconnectProfiler));
            SaveBoolToEditorPrefs(customBuildOptions.deepProfilingSupport, nameof(customBuildOptions.deepProfilingSupport));
            SaveBoolToEditorPrefs(customBuildOptions.scriptDebugging, nameof(customBuildOptions.scriptDebugging));
            SaveEnumToEditorPrefs(customBuildOptions.compressionMethod, nameof(customBuildOptions.compressionMethod));
            SaveEnumToEditorPrefs(customBuildOptions.branch, nameof(customBuildOptions.branch));
            SaveEnumToEditorPrefs(customBuildOptions.store, nameof(customBuildOptions.store));
            SaveBoolToEditorPrefs(customBuildOptions.preprocessBuild, nameof(customBuildOptions.preprocessBuild));
            SaveEnumToEditorPrefs(customBuildOptions.afterBuild, nameof(customBuildOptions.afterBuild));
            SaveListToEditorPrefs(customBuildOptions.extraScriptingDefines, nameof(customBuildOptions.extraScriptingDefines));
        }
        
        private string[] GetFilePathsInDirectory(string directoryPath)
        {
            if (Directory.Exists(directoryPath))
            {
                return Directory.GetFiles(directoryPath);
            }
        
            Debug.LogError("Directory does not exist: " + directoryPath);
            return Array.Empty<string>();
        }
        
        private bool TrySavePreset(CustomBuildOptions preset, out string savedName)
        {
            try
            {
                string savePath = EditorUtility.SaveFilePanelInProject(
                    "Save Preset",
                    $"{GetCurrentPresetNameFromEditorPrefs()}.{CustomBuildOptions.FILE_EXTENSION}",
                    CustomBuildOptions.FILE_EXTENSION,
                    string.Empty,
                    "Assets/" + buildPresetsPath);
                
                if (!string.IsNullOrEmpty(savePath))
                {
                    GUI.FocusControl(null);

                    string presetName = savePath
                        .Substring(savePath.LastIndexOf('/') + 1)
                        .Replace($".{CustomBuildOptions.FILE_EXTENSION}", string.Empty);
                    
                    CustomBuildOptions customBuildOptions = new CustomBuildOptions()
                    {
                        name = presetName,
                        buildPath = preset.buildPath,
                        executableName = preset.executableName,
                        platform = preset.platform,
                        developmentBuild = preset.developmentBuild,
                        autoconnectProfiler = preset.autoconnectProfiler,
                        deepProfilingSupport = preset.deepProfilingSupport,
                        scriptDebugging = preset.scriptDebugging,
                        compressionMethod = preset.compressionMethod,
                        branch = preset.branch,
                        store = preset.store,
                        preprocessBuild = preset.preprocessBuild,
                        afterBuild = preset.afterBuild,
                        extraScriptingDefines = preset.extraScriptingDefines,
                    };

                    string json = JsonUtility.ToJson(customBuildOptions, prettyPrint: true);
                    FileReadWrite.Write(json, savePath, isAssetsPath: true);
                    savedName = presetName;
                    return true;
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"Couldn't save preset {preset.name}: {e}");
            }

            savedName = string.Empty;
            return false;
        }
        
        private bool TryLoadPreset(string directoryPath, out CustomBuildOptions preset)
        {
            string loadPath = EditorUtility.OpenFilePanel("Load Preset", "Assets/" + directoryPath, CustomBuildOptions.FILE_EXTENSION);
            return TryLoadPresetAtFilePath(loadPath, out preset);
        }

        private bool TryLoadPresetAtFilePath(string filePath, out CustomBuildOptions preset)
        {
            if (!string.IsNullOrEmpty(filePath) &&
                Path.GetFileName(filePath).EndsWith(CustomBuildOptions.FILE_EXTENSION) &&
                FileReadWrite.TryRead(filePath, out string fileText))
            {
                try
                {
                    preset = JsonUtility.FromJson<CustomBuildOptions>(fileText);
                    preset.name = Path.GetFileName(filePath).Replace($".{CustomBuildOptions.FILE_EXTENSION}", string.Empty);
                    return true;
                }
                catch (Exception e)
                {
                    Debug.LogError($"Couldn't read preset {filePath}: {e.Message}");
                }
            }

            preset = default;
            return false;
        }

        private CustomBuildOptions[] LoadAllPresets()
        {
            List<CustomBuildOptions> presets = new();
            string[] paths = GetFilePathsInDirectory(Application.dataPath + "/" + buildPresetsPath);
            foreach (string path in paths)
            {
                if (TryLoadPresetAtFilePath(path, out CustomBuildOptions preset))
                {
                    presets.Add(preset);
                }
            }

            return presets.ToArray();
        }
        
        private void SaveStringToEditorPrefs(string saveValue, string fieldName)
        {
            string editorPrefsKey = GetEditorPrefsKey(fieldName);
            EditorPrefs.SetString(editorPrefsKey, saveValue);
        }
        
        private void SaveBoolToEditorPrefs(bool saveValue, string fieldName)
        {
            string editorPrefsKey = GetEditorPrefsKey(fieldName);
            EditorPrefs.SetBool(editorPrefsKey, saveValue);
        }
        
        private void SaveEnumToEditorPrefs<TEnum>(TEnum saveValue, string fieldName) where TEnum : Enum
        {
            string editorPrefsKey = GetEditorPrefsKey(fieldName);
            EditorPrefs.SetInt(editorPrefsKey, Convert.ToInt32(saveValue));
        }
        
        private void SaveListToEditorPrefs(IReadOnlyList<string> saveValue, string fieldName)
        {
            string countKey = GetEditorPrefsKey($"{fieldName}.Count");
            EditorPrefs.SetInt(countKey, saveValue.Count);
        
            for (int i = 0; i < saveValue.Count; i++)
            {
                string listElementKey = GetEditorPrefsKey($"{fieldName}[{i}]");
                EditorPrefs.SetString(listElementKey, saveValue[i]);
            }
        }
        
        private void StringField(ref string stringValue, string fieldName, string defaultValue = "")
        {
            string editorPrefsKey = GetEditorPrefsKey(fieldName);
            string savedValue = EditorPrefs.GetString(editorPrefsKey, defaultValue);
            stringValue = EditorGUILayout.TextField(new GUIContent(ToInspectorFieldName(fieldName)), savedValue);
            EditorPrefs.SetString(editorPrefsKey, stringValue);
        }
        
        private void ListStringField(ref string stringValue, string fieldName, string defaultValue = "")
        {
            string editorPrefsKey = GetEditorPrefsKey(fieldName);
            string savedValue = EditorPrefs.GetString(editorPrefsKey, defaultValue);
            stringValue = EditorGUILayout.TextField(savedValue);
            EditorPrefs.SetString(editorPrefsKey, stringValue);
        }
        
        private void BoolField(ref bool boolean, string fieldName, bool defaultValue = false)
        {
            string editorPrefsKey = GetEditorPrefsKey(fieldName);
            bool savedValue = EditorPrefs.GetBool(editorPrefsKey, defaultValue);
            boolean = EditorGUILayout.Toggle(new GUIContent(ToInspectorFieldName(fieldName)), savedValue);
            EditorPrefs.SetBool(editorPrefsKey, boolean);
        }
        
        private void EnumField<TEnum>(ref TEnum enumValue, string fieldName, int defaultValue = 0) where TEnum : Enum
        {
            string editorPrefsKey = GetEditorPrefsKey(fieldName);
            int editorPrefsInt = EditorPrefs.GetInt(editorPrefsKey, defaultValue);
            TEnum savedValue = (TEnum)Enum.ToObject(typeof(TEnum), editorPrefsInt);
            enumValue = (TEnum)EditorGUILayout.EnumPopup(new GUIContent(ToInspectorFieldName(fieldName)), savedValue);
            EditorPrefs.SetInt(editorPrefsKey, Convert.ToInt32(enumValue));
        }

        private string GetCurrentPresetNameFromEditorPrefs() => EditorPrefs.GetString(GetEditorPrefsKey(PRESET_NAME_FIELD_TITLE), string.Empty);
        
        private void SaveCurrentPresetNameToEditorPrefs(string presetName) => EditorPrefs.SetString(GetEditorPrefsKey(PRESET_NAME_FIELD_TITLE), presetName);
        
        private string GetBatchDetailsEditorPrefsKey(string presetName, string detailName) => GetEditorPrefsKey($"BuildPreset_{presetName}_{detailName}");

        private bool GetIncludeInBatchValueFromEditorPrefs(string presetName)
        {
            return EditorPrefs.GetBool(GetBatchDetailsEditorPrefsKey(presetName, "includeInBatch"), false);
        }
        
        private void SaveIncludeInBatchValueToEditorPrefs(string presetName, bool value)
        {
            EditorPrefs.SetBool(GetBatchDetailsEditorPrefsKey(presetName, "includeInBatch"), value);
        }
        
        private string GetPresetBuildPathValueFromEditorPrefs(string presetName)
        {
            return EditorPrefs.GetString(GetBatchDetailsEditorPrefsKey(presetName, "presetBuildPath"), currentOptions.buildPath + "/" + presetName);
        }
        
        private void SavePresetBuildPathValueToEditorPrefs(string presetName, string presetBuildPath)
        {
            EditorPrefs.SetString(GetBatchDetailsEditorPrefsKey(presetName, "presetBuildPath"), presetBuildPath);
        }
        
        private string GetEditorPrefsKey(string fieldName) => $"{nameof(CustomBuildWindow)}_{fieldName}";
        
        private static string ToInspectorFieldName(string s) => s.SpaceBetweenCapitalizedWords().CapitalizeFirst();
    }
}