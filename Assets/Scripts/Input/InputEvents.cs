using System;
using UnityEngine;

namespace StellarMass.Input
{
    public class InputEvents : MonoBehaviour
    {
        public static event Action OnReturnDown;
        
        private void Update()
        {
            if (UnityEngine.Input.GetKeyDown(KeyCode.Return))
            {
                OnReturnDown?.Invoke();
            }
        }
    }
}