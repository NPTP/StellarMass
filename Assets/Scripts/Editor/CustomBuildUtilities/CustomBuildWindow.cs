using System;
using System.Collections.Generic;
using Summoner.Editor.CustomBuildUtilities.Enums;
using Summoner.Utilities.Editor;
using Summoner.Utilities.Extensions;
using UnityEditor;
using UnityEngine;

namespace Summoner.Editor.CustomBuildUtilities
{
    public partial class CustomBuildWindow : EditorWindow
    {
        private const string WINDOW_HEADER = "<color=#FF3131><b>SUMMONER</b></color> Builder";
        private const string BUILD_BUTTON_TEXT = "BUILD";
        private const string DEFAULT_BUILD_PATH = "Builds";
        private const string PROJECT_OPTIONS_SUBHEADER = "Summoner Options";

        private string[] DefaultScriptingDefines => Array.Empty<string>();

        private GUIStyle HeaderStyle => new GUIStyle(GUI.skin.label) { fontSize = 24, fontStyle = FontStyle.Bold, alignment = TextAnchor.MiddleCenter, richText = true};
        private Color BuildButtonColor => new Color(0, 0.5f, 0);
        private string DefaultExecutableName => Application.version;

        // Build Options Preset
        private string customBuildPresetsPath;
        private string buildPresetName;
        private CustomBuildPreset currentPreset;

        // Build Options
        private string buildPath;
        private string executableName;
        private Platform platform = Platform.StandaloneWindows64;
        private bool developmentBuild;
        private bool autoconnectProfiler;
        private bool deepProfilingSupport;
        private bool scriptDebugging;
        private CompressionMethod compressionMethod;
        
        // Summoner Specific Options
        private Branch branch;
        private Store store;
        private bool preprocessBuild;
        private List<string> scriptingDefines = new();

        [MenuItem(EditorToolNames.CUSTOM_BUILD_WINDOW, isValidateFunction: false, priority: 9999)]
        public static void ShowWindow()
        { 
            if (GetWindow(typeof(CustomBuildWindow)) is CustomBuildWindow customBuildWindow)
            {
                customBuildWindow.titleContent = new GUIContent(ToInspectorFieldName(nameof(CustomBuildWindow)));
            }
        }

        private void OnEnable()
        {
        }

        private void OnDisable()
        {
        }
        
        // Updates 5-10 times per second
        public void OnInspectorUpdate()
        {
        }

        private void OnGUI()
        {
            GUILayout.Label(WINDOW_HEADER, HeaderStyle);
            
            EditorInspectorUtility.DrawHorizontalLine();
            
            EditorGUI.BeginDisabledGroup(Application.isPlaying);
            
            StringField(ref customBuildPresetsPath, nameof(customBuildPresetsPath), "Assets");
            StringField(ref buildPresetName, nameof(buildPresetName), string.Empty);
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Load Preset"))
            {
                string loadPath = EditorUtility.OpenFilePanel("Load Preset", customBuildPresetsPath, "cbp");
                if (!string.IsNullOrEmpty(loadPath) && FileReadWrite.TryRead(loadPath, out string fileText))
                {
                    try
                    {
                        CustomBuildPreset customBuildPreset = JsonUtility.FromJson<CustomBuildPreset>(fileText);
                        
                        buildPresetName = customBuildPreset.buildPresetName;
                        buildPath = customBuildPreset.buildPath;
                        executableName = customBuildPreset.executableName;
                        platform = customBuildPreset.platform;
                        developmentBuild = customBuildPreset.developmentBuild;
                        autoconnectProfiler = customBuildPreset.autoconnectProfiler;
                        deepProfilingSupport = customBuildPreset.deepProfilingSupport;
                        scriptDebugging = customBuildPreset.scriptDebugging;
                        compressionMethod = customBuildPreset.compressionMethod;
                        branch = customBuildPreset.branch;
                        store = customBuildPreset.store;
                        preprocessBuild = customBuildPreset.preprocessBuild;
                        scriptingDefines = customBuildPreset.scriptingDefines;
                        
                        SaveString(customBuildPreset.buildPresetName, nameof(buildPresetName));
                        SaveString(customBuildPreset.buildPath, nameof(buildPath));
                        SaveString(customBuildPreset.executableName, nameof(executableName));
                        SaveEnum(customBuildPreset.platform, nameof(platform));
                        SaveBool(customBuildPreset.developmentBuild, nameof(developmentBuild));
                        SaveBool(customBuildPreset.autoconnectProfiler, nameof(autoconnectProfiler));
                        SaveBool(customBuildPreset.deepProfilingSupport, nameof(deepProfilingSupport));
                        SaveBool(customBuildPreset.scriptDebugging, nameof(scriptDebugging));
                        SaveEnum(customBuildPreset.compressionMethod, nameof(compressionMethod));
                        SaveEnum(customBuildPreset.branch, nameof(branch));
                        SaveEnum(customBuildPreset.store, nameof(store));
                        SaveBool(customBuildPreset.preprocessBuild, nameof(preprocessBuild));
                        SaveList(customBuildPreset.scriptingDefines, nameof(scriptingDefines));
                    }
                    catch (Exception e)
                    {
                        Debug.LogError($"Couldn't read preset: {e.Message}");
                    }
                }
            }

            if (GUILayout.Button("Save Preset"))
            {
                string savePath = EditorUtility.SaveFilePanelInProject("Save Preset", $"{buildPresetName}.cbp", "cbp", "", customBuildPresetsPath);
                if (!string.IsNullOrEmpty(savePath))
                {
                    CustomBuildPreset customBuildPreset = new CustomBuildPreset()
                    {
                        buildPresetName = buildPresetName,
                        buildPath = buildPath,
                        executableName = executableName,
                        platform = platform,
                        developmentBuild = developmentBuild,
                        autoconnectProfiler = autoconnectProfiler,
                        deepProfilingSupport = deepProfilingSupport,
                        scriptDebugging = scriptDebugging,
                        compressionMethod = compressionMethod,
                        branch = branch,
                        store = store,
                        preprocessBuild = preprocessBuild,
                        scriptingDefines = scriptingDefines,
                    };

                    string json = JsonUtility.ToJson(customBuildPreset, prettyPrint: true);
                    FileReadWrite.Write(json, savePath, isAssetsPath: true);
                }
            }
            GUILayout.EndHorizontal();

            EditorInspectorUtility.DrawHorizontalLine();

            EditorGUILayout.LabelField("Build Options", EditorStyles.boldLabel);
            PlayerSettings.bundleVersion = EditorGUILayout.TextField(new GUIContent("Version", "Changes the application version in the Player settings."), Application.version);
            StringField(ref buildPath, nameof(buildPath), defaultValue: DEFAULT_BUILD_PATH);
            if (buildPath.Length > 0 && buildPath[^1] == '/') buildPath.Substring(0, buildPath.Length - 1);
            StringField(ref executableName, nameof(executableName), DefaultExecutableName);
            EnumField(ref platform, nameof(platform), defaultValue: (int)Platform.StandaloneWindows64);
            BoolField(ref developmentBuild, nameof(developmentBuild));
            EditorGUI.BeginDisabledGroup(!developmentBuild);
            BoolField(ref autoconnectProfiler, nameof(autoconnectProfiler));
            BoolField(ref deepProfilingSupport, nameof(deepProfilingSupport));
            BoolField(ref scriptDebugging, nameof(scriptDebugging));
            EditorGUI.EndDisabledGroup();
            EnumField(ref compressionMethod, nameof(compressionMethod));
            
            EditorInspectorUtility.DrawHorizontalLine();
            
            EditorGUILayout.LabelField(PROJECT_OPTIONS_SUBHEADER, EditorStyles.boldLabel);
            EnumField(ref branch, nameof(branch));
            EnumField(ref store, nameof(store));
            BoolField(ref preprocessBuild, nameof(preprocessBuild), defaultValue: true);
            
            GUILayout.Space(10);
            EditorGUILayout.LabelField("Scripting Defines", EditorStyles.boldLabel);
            
            int scriptingDefinesCount = EditorPrefs.GetInt(GetEditorPrefsKey($"{nameof(scriptingDefines)}.Count"), defaultValue: 0);
            if (scriptingDefinesCount == 0)
            {
                scriptingDefines.Clear();
                string value = string.Empty;
                scriptingDefines.Add(value);
                ListStringField(ref value, $"{nameof(scriptingDefines)}[0]");
            }
            else
            {
                scriptingDefines.Clear();
                for (int i = 0; i < scriptingDefinesCount; i++)
                {
                    scriptingDefines.Add(string.Empty);
                    string value = string.Empty;
                    ListStringField(ref value, $"{nameof(scriptingDefines)}[{i}]");
                    scriptingDefines[i] = value;
                }
            }
            
            GUILayout.BeginHorizontal();
            EditorGUI.BeginDisabledGroup(scriptingDefines.Count == 1);
            if (GUILayout.Button("-")) scriptingDefines.RemoveAt(scriptingDefines.Count - 1);
            EditorGUI.EndDisabledGroup();
            if (GUILayout.Button("+")) scriptingDefines.Add(string.Empty);
            GUILayout.EndHorizontal();
            
            EditorPrefs.SetInt(GetEditorPrefsKey($"{nameof(scriptingDefines)}.Count"), scriptingDefines.Count);

            EditorInspectorUtility.DrawHorizontalLine();

            ShowBuildButton();
            
            EditorGUI.EndDisabledGroup();
        }

        private void ShowBuildButton()
        {
            float buttonWidth = 150f;
            float buttonHeight = 60f;
            GUIStyle buttonStyle = new GUIStyle(GUI.skin.button)
            {
                normal = { textColor = Color.white, background = makeTexture2D(1, 1, BuildButtonColor) },
                fontSize = 18,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter
            };
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button(BUILD_BUTTON_TEXT, buttonStyle, GUILayout.Width(buttonWidth), GUILayout.Height(buttonHeight)))
            {
                Build();
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            
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

        private void SaveString(string saveValue, string fieldName)
        {
            string editorPrefsKey = GetEditorPrefsKey(fieldName);
            EditorPrefs.SetString(editorPrefsKey, saveValue);
        }
        
        private void SaveBool(bool saveValue, string fieldName)
        {
            string editorPrefsKey = GetEditorPrefsKey(fieldName);
            EditorPrefs.SetBool(editorPrefsKey, saveValue);
        }

        private void SaveEnum<TEnum>(TEnum saveValue, string fieldName) where TEnum : Enum
        {
            string editorPrefsKey = GetEditorPrefsKey(fieldName);
            EditorPrefs.SetInt(editorPrefsKey, Convert.ToInt32(saveValue));
        }

        private void SaveList(List<string> saveValue, string fieldName)
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

        private string GetEditorPrefsKey(string fieldName) => nameof(CustomBuildWindow) + fieldName;

        private static string ToInspectorFieldName(string s) => s.SpaceBetweenCapitalizedWords().CapitalizeFirst();
    }
}