using System;
using UnityEditor;
using UnityEngine;

namespace StellarMass.InputManagement.Data.Editor
{
    [CustomEditor(typeof(BindingData))]
    public class BindingDataEditor : UnityEditor.Editor
    {
        private const string MOUSE = "Generate Mouse Control Paths";
        private const string KEYBOARD = "Generate Keyboard Control Paths";
        private const string GAMEPAD = "Generate Gamepad Control Paths";
        
        private BindingData targetBindingData;

        private void OnEnable()
        {
            targetBindingData = (BindingData)target;
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            GeneratorButton(MOUSE);
            GeneratorButton(KEYBOARD);
            GeneratorButton(GAMEPAD);
        }

        private void GeneratorButton(string label)
        {
            if (!GUILayout.Button(label))
            {
                return;
            }
            
            string[] controls = Array.Empty<string>();
            switch (label)
            {
                case MOUSE:
                    controls = InputDataHelper.MouseControls;
                    break;
                case KEYBOARD:
                    controls = InputDataHelper.KeyboardControls;
                    break;
                case GAMEPAD:
                    controls = InputDataHelper.GamepadControls;
                    break;
            }

            targetBindingData.BindingDisplayInfo.EDITOR_Clear();
            foreach (string s in controls)
            {
                targetBindingData.BindingDisplayInfo.EDITOR_Add(s, new BindingPathInfo());
            }
        }
    }
}