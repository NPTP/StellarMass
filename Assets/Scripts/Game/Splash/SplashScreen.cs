using System.Collections;
using FMOD.Studio;
using FMODUnity;
using Summoner.Utilities.FMODUtilities;
using Summoner.Game.SaveAndLoad;
using Summoner.Systems.SaveAndLoad;
using Summoner.Systems.SceneManagement;
using Summoner.Utilities.Attributes;
using Summoner.Utilities.FMODUtilities.Enums;
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
        [SerializeField] private EventReference splashScreenFmodEventRef;

        private EventInstance splashScreenFmodEventInstance;
        private AnimationStateExitBehaviour animationStateExitBehaviour;

        private void OnValidate()
        {
            tmpText.text = titleText;
        }

        private IEnumerator Start()
        {
            // Ensure splash screen audio is loaded ahead of time so it syncs to animation.
            splashScreenFmodEventRef.LoadSampleData();
            while (!splashScreenFmodEventRef.IsLoaded())
            {
                yield return null;
            }
            
            // Let player skip with any button press if they've seen the splash screen animation once before.
            GameSettings gameSettings = SaveLoad.Get<GameSettings>();
            if (gameSettings.hasSeenSplashScreen)
                Input.OnAnyButtonPress += HandleAnyButtonPress;
            else
                gameSettings.hasSeenSplashScreen = true;

            animationStateExitBehaviour = animator.GetBehaviour<AnimationStateExitBehaviour>();
            animationStateExitBehaviour.OnAnimationStateExit += HandleAnimationStateExit;

            tmpText.text = titleText;

            splashScreenFmodEventInstance = splashScreenFmodEventRef.PlayOneShot();
            animator.Play(splashScreenAnimation);
        }

        private void HandleAnyButtonPress(InputControl inputControl) => StopAnimationAndLoadNextScene();
        private void HandleAnimationStateExit() => StopAnimationAndLoadNextScene();
        private void StopAnimationAndLoadNextScene()
        {
            splashScreenFmodEventInstance.Stop(StopFlags.Immediate | StopFlags.Release | StopFlags.ClearHandle);
            splashScreenFmodEventRef.UnloadSampleData();
            
            Input.OnAnyButtonPress -= HandleAnyButtonPress;
            animationStateExitBehaviour.OnAnimationStateExit -= HandleAnimationStateExit;
            
            SceneLoader.LoadScene(nextScene);
        }
    }
}