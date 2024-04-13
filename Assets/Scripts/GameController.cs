using System;
using StellarMass.Input;
using UnityEngine;

namespace StellarMass
{
    public class GameController : MonoBehaviour
    {
        [SerializeField] private GameObject title;
        [SerializeField] private GameObject prompt;
        [SerializeField] private GameObject player;

        private void Awake()
        {
            InputEvents.OnReturnDown += HandleReturnDown;
        }

        private void OnDestroy()
        {
            InputEvents.OnReturnDown -= HandleReturnDown;
        }

        private void Start()
        {
#if !UNITY_EDITOR
            Cursor.visible = false;
#endif
        }

        private void HandleReturnDown()
        {
            title.SetActive(false);
            prompt.SetActive(false);
            player.SetActive(true);
        }
    }
}