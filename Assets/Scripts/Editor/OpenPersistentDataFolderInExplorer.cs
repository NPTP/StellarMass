using System.IO;
using UnityEditor;
using UnityEngine;

namespace Summoner.Editor
{
	public static class OpenPersistentDataFolderInExplorer
	{
		[MenuItem(EditorToolNames.OPEN_PERSISTENT_DATA_FOLDER, isValidateFunction: false, priority: 9999999)]
		private static void OpenPersistentDataFolder()
		{
			EditorUtility.RevealInFinder(Application.persistentDataPath + Path.DirectorySeparatorChar);
		}
	}
}