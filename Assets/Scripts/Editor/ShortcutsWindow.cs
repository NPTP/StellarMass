using System.Collections.Generic;
using StellarMass.Data;
using StellarMass.VFX;
using UnityEditor;
using UnityEngine;

namespace StellarMass.Editor
{
	public class ShortcutsWindow : EditorWindow
	{
		private class FolderShortcut
		{
			public readonly string name;
			public readonly string path;

			public FolderShortcut(string name, string path)
			{
				this.name = name;
				this.path = path;
			}
		}

		private readonly List<DataScriptable> dataScriptables = new List<DataScriptable>();
		private readonly FolderShortcut[] folderShortcuts = new[]
		{
			new FolderShortcut("Resources", "Assets/Resources"),
			new FolderShortcut("Game Phases", "Assets/ScriptableObjects/GamePhases"),
			new FolderShortcut("Sounds", "Assets/ScriptableObjects/Audio"),
		};

		private PostProcessingChanger globalPostProcessing;
		private bool postProcessingEnabled;
		
		[MenuItem(EditorToolNames.SHORTCUTS_WINDOW)]
		public static void ShowWindow()
		{
			GetWindow(typeof(ShortcutsWindow));
		}

		private void Awake()
		{
			Refresh();
		}

		private void Refresh()
		{
			globalPostProcessing = FindObjectOfType<PostProcessingChanger>(includeInactive: true);
			postProcessingEnabled = globalPostProcessing.gameObject.activeSelf;
			GetData();
		}

		private void OnGUI()
		{
			if (GUILayout.Button("Refresh", GUILayout.Width(100)))
			{
				Refresh();
			}
			
			postProcessingEnabled = EditorGUILayout.Toggle("Enable PostProcessing", postProcessingEnabled);
			globalPostProcessing.gameObject.SetActive(postProcessingEnabled);
				
			GUILayout.BeginHorizontal();
			{
				GUILayout.BeginVertical();
				{
					ShowDataShortcuts();
				}
				GUILayout.EndVertical();

				GUILayout.BeginVertical();
				{
					ShowFolderShortcuts();
				}
				GUILayout.EndVertical();
			}
			GUILayout.EndHorizontal();
		}

		private void ShowDataShortcuts()
		{
			GUILayout.Label("Data");
			
			foreach (DataScriptable data in dataScriptables)
			{
				if (data == null)
				{
					continue;
				}

				if (GUILayout.Button(data.name.Replace("Data", string.Empty)))
				{
					SelectData(data);
				}
			}
		}

		private void ShowFolderShortcuts()
		{
			GUILayout.Label("Folders");

			foreach (FolderShortcut folderShortcut in folderShortcuts)
			{
				if (GUILayout.Button(folderShortcut.name))
				{
					EditorUtility.FocusProjectWindow();
					Object obj = AssetDatabase.LoadAssetAtPath<Object>(folderShortcut.path);
					Selection.activeObject = obj;
				}
			}
		}

		private void GetData()
		{
			string[] guids = AssetDatabase.FindAssets($"t:{nameof(DataScriptable)}");
			dataScriptables.Clear();
			foreach (string guid in guids)
			{
				DataScriptable globalData = AssetDatabase.LoadAssetAtPath<DataScriptable>(AssetDatabase.GUIDToAssetPath(guid));
				dataScriptables.Add(globalData);
			}
		}
		
		private void SelectData(DataScriptable selectedData)
		{
			if (selectedData == null)
			{
				return;
			}
        
			Selection.activeObject = selectedData;
		}
	}
}