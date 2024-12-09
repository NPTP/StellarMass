using UnityEditor;
using UnityEngine;

namespace Summoner.Utilities.VFX.Editor
{
    [CustomEditor(typeof(SpriteRendererFadeGroup))]
    public class SpriteRendererFadeGroupEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            if (GUILayout.Button("Get Components In Children"))
            {
                ((SpriteRendererFadeGroup)target).EDITOR_GetComponentsInChildren();
            }
        }
    }
}