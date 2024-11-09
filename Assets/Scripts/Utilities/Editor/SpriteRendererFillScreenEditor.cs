using UnityEditor;
using UnityEngine;

namespace Summoner.Utilities.Editor
{
    [CustomEditor(typeof(SpriteRendererFillScreen))]
    public class SpriteRendererFillScreenEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            if (GUILayout.Button("Update Fill"))
            {
                ((SpriteRendererFillScreen)target).UpdateFill();
            }
        }
    }
}