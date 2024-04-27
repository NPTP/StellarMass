using UnityEngine;
using UnityEngine.InputSystem.UI;
using Utilities;

namespace StellarMass.UI
{
    public class UIController : ClosedSingleton<UIController>
    {
        [SerializeField] private InputSystemUIInputModule uiInputModule;
        public static InputSystemUIInputModule UIInputModule => PrivateInstance.uiInputModule;
    }
}