using System;
using System.Collections.Generic;
using System.Reflection;
using StellarMass.Game.ScreenLoop;
using StellarMass.Game.VFX;
using StellarMass.Systems.Data;
using StellarMass.Utilities.Extensions;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

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
			new FolderShortcut("Scripts", "Assets/Scripts"),
			new FolderShortcut("Scenes", "Assets/Scenes"),
			new FolderShortcut("Prefabs", "Assets/Prefabs"),
			new FolderShortcut("Data", "Assets/Data"),
			new FolderShortcut("Game Phases", "Assets/Data/GamePhases")
		};

		private bool RefreshRequired => globalPostProcessing == null || loopBoundingBox == null || dataScriptables.IsEmpty();

		private PostProcessingChanger globalPostProcessing;
		private bool postProcessingEnabled;
		private LoopBoundingBox loopBoundingBox;
		private bool loopBoundingBoxVisualizerEnabled;
		
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
			if (globalPostProcessing != null) postProcessingEnabled = globalPostProcessing.Volume.enabled;
			loopBoundingBox = FindObjectOfType<LoopBoundingBox>(includeInactive: true);
			if (loopBoundingBox != null) loopBoundingBoxVisualizerEnabled = loopBoundingBox.BoundsVisualizer.enabled;
			GetData();
		}

		private void OnGUI()
		{
			if (RefreshRequired)
			{
				Refresh();
			}

			GUILayout.BeginVertical();
			{
				PostProcessingToggle();
				BoundsVisualizeToggle();
			}
			GUILayout.EndVertical();

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

		private void PostProcessingToggle()
		{
			if (globalPostProcessing == null)
			{
				return;
			}
			
			ValueToggle(ref postProcessingEnabled, globalPostProcessing.Volume, "Enable PostProcessing");
		}

		private void BoundsVisualizeToggle()
		{
			if (loopBoundingBox == null)
			{
				return;
			}
			
			ValueToggle(ref loopBoundingBoxVisualizerEnabled, loopBoundingBox.BoundsVisualizer, "Visualize Loop Bounds");
		}

		private void ValueToggle(ref bool toggle, Behaviour toggleable, string label)
		{
			bool previousValue = toggle;
			toggle = EditorGUILayout.Toggle(label, toggle);
			toggleable.enabled = toggle;
			if (toggle != previousValue)
			{
				EditorUtility.SetDirty(toggleable.gameObject);
			}
		}
		
		private void ValueToggle(ref bool toggle, Renderer toggleable, string label)
		{
			bool previousValue = toggle;
			toggle = EditorGUILayout.Toggle(label, toggle);
			toggleable.enabled = toggle;
			if (toggle != previousValue)
			{
				EditorUtility.SetDirty(toggleable.gameObject);
			}
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
					OpenFolder(folderShortcut.path);
				}
			}
		}

		private void OpenFolder(string folderPath)
		{
			EditorUtility.FocusProjectWindow();
			Object folder = AssetDatabase.LoadAssetAtPath<Object>(folderPath);
			Type projectBrowserType = Type.GetType("UnityEditor.ProjectBrowser,UnityEditor");
			object lastInteractedBrowser = projectBrowserType.GetField("s_LastInteractedProjectBrowser", BindingFlags.Static | BindingFlags.Public).GetValue(null);
			MethodInfo showFolderContentsMethod = projectBrowserType.GetMethod("ShowFolderContents", BindingFlags.NonPublic | BindingFlags.Instance);
			showFolderContentsMethod?.Invoke(lastInteractedBrowser, new object[] { folder.GetInstanceID(), true });

			// Puts us at the top of the folder we just opened.
			EditorWindow projectWindow = GetWindow(projectBrowserType);
			if (projectWindow != null)
			{
				Event e = new()
				{
					type = EventType.KeyDown,
					keyCode = KeyCode.Home
				};
				projectWindow.SendEvent(e);
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