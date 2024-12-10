using UnityEditor;
using UnityEngine;

namespace Summoner.Utilities.VFX.Editor
{
    [CustomEditor(typeof(SpriteRendererGroup))]
    public class SpriteRendererGroupEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            if (GUILayout.Button("Get Components In Children"))
            {
                ((SpriteRendererGroup)target).EDITOR_GetComponentsInChildren();
            }
        }
    }
}