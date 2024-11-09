using FMODUnity;
using Summoner.Utilities.FMODUtilities;
using Summoner.Game.SaveAndLoad;
using Summoner.Systems.SaveAndLoad;
using Summoner.Systems.SceneManagement;
using UnityEngine;
using UnityEngine.InputSystem;
using Input = NPTP.InputSystemWrapper.Input;

namespace Summoner.Game.Splash
{
    public class SplashScreen : MonoBehaviour
    {
        [SerializeField] private Animator animator;
        [SerializeField] private EventReference logoAppearEvent;
        [SerializeField] private SceneReference nextScene;

        private AnimationStateExitBehaviour animationStateExitBehaviour;
    
        private void Awake()
        {
            if (SaveLoad.Get<GameSettings>().hasSeenSplashScreen)
            {
                Input.OnAnyButtonPress += HandleAnyButtonPress;
            }
            
            animationStateExitBehaviour = animator.GetBehaviour<AnimationStateExitBehaviour>();
            animationStateExitBehaviour.OnAnimationStateExit += HandleAnimationStateExit;
        }

        private void HandleAnyButtonPress(InputControl inputControl) => StopAnimationAndLoadNextScene();
        private void HandleAnimationStateExit() => StopAnimationAndLoadNextScene();
        private void StopAnimationAndLoadNextScene()
        {
            Input.OnAnyButtonPress -= HandleAnyButtonPress;
            animationStateExitBehaviour.OnAnimationStateExit -= HandleAnimationStateExit;
            SaveLoad.Get<GameSettings>().hasSeenSplashScreen = true;
            SceneLoader.LoadScene(nextScene);
        }

        public void PlayLogoAppearSound()
        {
            logoAppearEvent.PlayOneShot();
        }
    }
}