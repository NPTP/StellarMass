using System.Collections;
using UnityEngine;

namespace StellarMass.GameControl
{
    [CreateAssetMenu]
    public class DisableDisplayPhase : GamePhase
    {
        protected override IEnumerator Execution(GameController gameController)
        {
            gameController.MainDisplay.TurnOff();
            yield break;
        }
    }
}