using UnityEditor;
using UnityEngine;

namespace Summoner.Utilities.Editor
{
    public class EditorInspectorUtility : MonoBehaviour
    {
        public static void DrawHorizontalLine()
        {
            EditorGUILayout.LabelField(string.Empty, GUI.skin.horizontalSlider);
        }
    }
}