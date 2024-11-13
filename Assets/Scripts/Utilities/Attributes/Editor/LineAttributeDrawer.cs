using Summoner.Utilities.Editor;
using UnityEditor;
using UnityEngine;

namespace Summoner.Utilities.Attributes.Editor
{
    [CustomPropertyDrawer(typeof(LineAttribute))]
    public class LineAttributeDrawer : DecoratorDrawer
    {
        public override void OnGUI(Rect position)
        {
            EditorInspectorUtility.DrawHorizontalLine();
        }
    }
}