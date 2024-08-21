using System.Collections;
using NPTP.InputSystemWrapper.Enums;
using UnityEngine;
using Input = NPTP.InputSystemWrapper.Input;

namespace StellarMass.Game.GameControl.Phases
{
    [CreateAssetMenu]
    public class GameplayPhase : GamePhase
    {
        protected override IEnumerator Execution(GameController gameController)
        {
            // Input.EnableContext(InputContext.Gameplay);
            
            while (true)
            {
                yield return null;
            }
        }
    }
}