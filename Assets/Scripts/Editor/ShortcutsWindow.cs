using System;
using System.Collections.Generic;
using System.Reflection;
using StellarMass.Game.ScreenLoop;
using StellarMass.Systems.Camera;
using StellarMass.Systems.Data;
using StellarMass.Utilities;
using StellarMass.Utilities.Extensions;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using Object = UnityEngine.Object;

namespace StellarMass.Editor
{
	public class ShortcutsWindow : EditorWindow
	{
		private const string VOLUME = "volume";
		
		private class WindowItem { }
		private abstract class Shortcut : WindowItem
		{
			public readonly string name;
			public readonly string path;

			protected Shortcut(string name, string path)
			{
				this.name = name;
				this.path = path;
			}
		}
		private sealed class AssetShortcut : Shortcut { public AssetShortcut(string name, string path) : base(name, path) { } }
		private sealed class FolderShortcut : Shortcut { public FolderShortcut(string name, string path) : base(name, path) { } }
		private sealed class Space : WindowItem { }
		private sealed class Line : WindowItem { }

		private readonly List<DataScriptable> dataScriptables = new List<DataScriptable>();
		private readonly WindowItem[] assetAndFolderShortcuts = new WindowItem[]
		{
			new FolderShortcut("Scripts", "Assets/Scripts"),
			new FolderShortcut("Scenes", "Assets/Scenes"),
			new FolderShortcut("Prefabs", "Assets/Prefabs"),
			new FolderShortcut("Data", "Assets/Data"),
			new FolderShortcut("Game Phases", "Assets/Data/GamePhases"),
			new Space(),
			new Line()
		};

		private bool RefreshRequired => cameraController == null || loopBoundingBox == null || dataScriptables.IsEmpty();

		private CameraController cameraController;
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
			cameraController = FindObjectOfType<CameraController>(includeInactive: true);
			if (cameraController != null) postProcessingEnabled = cameraController.GetField<PostProcessVolume>(VOLUME).enabled;
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
					ShowAssetAndFolderShortcuts();
				}
				GUILayout.EndVertical();
			}
			GUILayout.EndHorizontal();
		}

		private void PostProcessingToggle()
		{
			if (cameraController == null)
			{
				return;
			}
			
			ValueToggle(ref postProcessingEnabled, cameraController.GetField<PostProcessVolume>(VOLUME), "Enable PostProcessing");
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
			
			foreach (DataScriptable scriptable in dataScriptables)
			{
				if (scriptable == null)
				{
					continue;
				}

				if (GUILayout.Button(scriptable.name.Replace("Data", string.Empty)))
				{
					SelectData(scriptable);
				}
			}
		}
		
		private void GetData()
		{
			string[] guids = AssetDatabase.FindAssets($"t:{nameof(DataScriptable)}");
			dataScriptables.Clear();
			foreach (string guid in guids)
			{
				DataScriptable scriptable = AssetDatabase.LoadAssetAtPath<DataScriptable>(AssetDatabase.GUIDToAssetPath(guid));
				dataScriptables.Add(scriptable);
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

		private void ShowAssetAndFolderShortcuts()
		{
			GUILayout.Label("Assets & Folders");

			foreach (WindowItem item in assetAndFolderShortcuts)
			{
				switch (item)
				{
					case Space:
						// GUILayout.Space(EditorGUIUtility.singleLineHeight);
						break;
					case Line:
						// EditorGUILayout.LabelField(string.Empty, GUI.skin.horizontalSlider);
						break;
					case AssetShortcut assetShortcut when GUILayout.Button(assetShortcut.name):
						SelectAsset(assetShortcut.path);
						break;
					case FolderShortcut folderShortcut when GUILayout.Button(folderShortcut.name):
						OpenFolder(folderShortcut.path);
						break;
				}
			}
		}

		private static void SelectAsset(string path)
		{
			Object asset = AssetDatabase.LoadAssetAtPath<Object>(path);
			Selection.activeObject = asset;
		}

		private void OpenFolder(string path)
		{
			EditorUtility.FocusProjectWindow();
			Object folder = AssetDatabase.LoadAssetAtPath<Object>(path);
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
	}
}