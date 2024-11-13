using System;
using FMOD.Studio;
using FMODUnity;
using Summoner.Utilities.FMODUtilities;
using Summoner.Game.SaveAndLoad;
using Summoner.Systems.SaveAndLoad;
using Summoner.Systems.SceneManagement;
using Summoner.Utilities.Attributes;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using Input = NPTP.InputSystemWrapper.Input;

namespace Summoner.Game.Splash
{
    public class SplashScreen : MonoBehaviour
    {
        private static readonly int splashScreenAnimation = Animator.StringToHash("SplashScreenAnimation");
        
        [SerializeField] private string titleText = "SUMMONER";
        [Line]
        [SerializeField] private SceneReference nextScene;
        [Line]
        [SerializeField] private Animator animator;
        [SerializeField] private TextMeshPro tmpText;
        [SerializeField] private EventReference logoAppearEvent;

        private EventInstance eventInstance;
        private AnimationStateExitBehaviour animationStateExitBehaviour;

        private void OnValidate()
        {
            tmpText.text = titleText;
        }

        private void Awake()
        {
            if (SaveLoad.Get<GameSettings>().hasSeenSplashScreen)
            {
                Input.OnAnyButtonPress += HandleAnyButtonPress;
            }
            
            animationStateExitBehaviour = animator.GetBehaviour<AnimationStateExitBehaviour>();
            animationStateExitBehaviour.OnAnimationStateExit += HandleAnimationStateExit;

            tmpText.text = titleText;
            
            animator.Play(splashScreenAnimation);
        }

        private void HandleAnyButtonPress(InputControl inputControl) => StopAnimationAndLoadNextScene();
        private void HandleAnimationStateExit() => StopAnimationAndLoadNextScene();
        private void StopAnimationAndLoadNextScene()
        {
            eventInstance.StopImmediate();
            Input.OnAnyButtonPress -= HandleAnyButtonPress;
            animationStateExitBehaviour.OnAnimationStateExit -= HandleAnimationStateExit;
            SaveLoad.Get<GameSettings>().hasSeenSplashScreen = true;
            SceneLoader.LoadScene(nextScene);
        }

        public void PlayLogoAppearSound()
        {
            eventInstance = logoAppearEvent.PlayOneShot();
        }
    }
}