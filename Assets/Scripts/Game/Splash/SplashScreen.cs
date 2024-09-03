using FMODUnity;
using StellarMass.Game.Splash;
using StellarMass.Systems.SceneLoading;
using StellarMass.Utilities.FMODUtilities;
using StellarMass.Utilities.References;
using UnityEngine;

public class SplashScreen : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private EventReference logoAppearEvent;
    [SerializeField] private SceneReference nextScene;

    private AnimationStateExitBehaviour animationStateExitBehaviour;
    
    private void Awake()
    {
        animationStateExitBehaviour = animator.GetBehaviour<AnimationStateExitBehaviour>();
        animationStateExitBehaviour.OnAnimationStateExit += HandleAnimationStateExit;
    }

    private void HandleAnimationStateExit()
    {
        animationStateExitBehaviour.OnAnimationStateExit -= HandleAnimationStateExit;
        SceneLoader.LoadScene(nextScene);
    }

    public void PlayLogoAppearSound()
    {
        logoAppearEvent.PlayOneShot();
    }
}
