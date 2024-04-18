using System.Collections;
using StellarMass.Animation;
using StellarMass.Input;
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
                InputReceiver.OnAnyKeyDown += handleAcceptDown;
            }
            
            void handleAcceptDown()
            {
                InputReceiver.OnAnyKeyDown -= handleAcceptDown;
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