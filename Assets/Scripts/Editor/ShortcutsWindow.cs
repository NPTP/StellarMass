using System.Collections.Generic;
using StellarMass.Data;
using UnityEditor;
using UnityEngine;

namespace StellarMass.Editor
{
	public class ShortcutsWindow : EditorWindow
	{
		private List<DataScriptable> dataScriptables = new List<DataScriptable>();
		
		[MenuItem(EditorToolNames.SHORTCUTS_WINDOW)]
		public static void ShowWindow()
		{
			GetWindow(typeof(ShortcutsWindow));
		}

		private void OnGUI()
		{
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
			if (dataScriptables.Count == 0)
			{
				GetData();
			}

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