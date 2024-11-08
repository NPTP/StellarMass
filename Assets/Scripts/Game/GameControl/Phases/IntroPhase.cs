using System.Collections;
using StellarMass.Systems.Animation;
using UnityEngine;
using UnityEngine.InputSystem;
using Input = NPTP.InputSystemWrapper.Input;

namespace StellarMass.Game.GameControl.Phases
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
                Input.OnAnyButtonPress += handleAnyButtonPress;
            }
            
            void handleAnyButtonPress(InputControl inputControl)
            {
                Input.OnAnyButtonPress -= handleAnyButtonPress;
                
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