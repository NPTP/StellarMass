using System.Collections;
using UnityEngine;

namespace StellarMass.Game.GameControl.Phases
{
    [CreateAssetMenu]
    public class DisableDisplayPhase : GamePhase
    {
        protected override IEnumerator Execution(GameController gameController)
        {
            GameController.GameState = GameState.Cutscene;
            gameController.MainDisplay.TurnOff();
            yield break;
        }
    }
}