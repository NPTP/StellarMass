using System.Collections;
using UnityEngine;

namespace StellarMass.GameControl.Phases
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