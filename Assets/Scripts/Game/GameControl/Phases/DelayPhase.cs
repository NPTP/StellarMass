using System.Collections;
using UnityEngine;

namespace Summoner.Game.GameControl.Phases
{
    [CreateAssetMenu]
    public class DelayPhase : GamePhase
    {
        [SerializeField] private float delayTime = 1f;

        protected override IEnumerator Execution(GameController gameController)
        {
            yield return new WaitForSeconds(delayTime);
        }
    }
}