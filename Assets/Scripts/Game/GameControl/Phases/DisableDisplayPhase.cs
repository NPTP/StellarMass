using System.Collections;
using UnityEngine;

namespace Summoner.Game.GameControl.Phases
{
    [CreateAssetMenu]
    public class DisableDisplayPhase : GamePhase
    {
        protected override IEnumerator Execution(GameController gameController)
        {
            GameState.InCutscene = true;
            gameController.MainDisplay.TurnOff();
            yield break;
        }
    }
}