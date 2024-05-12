using System.Collections;
using UnityEngine;

namespace StellarMass.Game.GameControl.Phases
{
    [CreateAssetMenu]
    public class GameplayPhase : GamePhase
    {
        protected override IEnumerator Execution(GameController gameController)
        {
            while (true)
            {
                yield return null;
            }
        }
    }
}