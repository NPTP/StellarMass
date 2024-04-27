using System.Collections;
using StellarMass.Animation;
using StellarMass.InputManagement;
using UnityEngine;

namespace StellarMass.GameControl.Phases
{
    [CreateAssetMenu]
    public class IntroPhase : GamePhase
    {
        private readonly int startupAnimation = Animator.StringToHash("Startup");

        protected override IEnumerator Execution(GameController gameController)
        {
            gameController.MainDisplay.TurnOn();
            AnimatorController animatorController = gameController.MainDisplay.AnimatorController;
            animatorController.OnAnimationCompleted += handleAnimationCompleted;
            animatorController.Play(startupAnimation);

            bool inputReceived = false;
            yield return new WaitUntil(() => inputReceived);
            
            void handleAnimationCompleted()
            {
                InputManager.OnAnyButtonPressed += handleAnyButtonPressed;
            }
            
            void handleAnyButtonPressed()
            {
                InputManager.OnAnyButtonPressed -= handleAnyButtonPressed;
                
                inputReceived = true;
                gameController.MainDisplay.Title.SetActive(false);
                gameController.MainDisplay.Prompt.SetActive(false);
                gameController.MainDisplay.Stars.SetActive(false);
                gameController.MainDisplay.Score.SetActive(true);
                gameController.Player.SetActive(true);
            }
        }
    }
}