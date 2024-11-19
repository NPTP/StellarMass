using System.Collections;
using NPTP.InputSystemWrapper.Enums;
using UnityEngine;
using Input = NPTP.InputSystemWrapper.Input;

namespace Summoner.Game.GameControl.Phases
{
    [CreateAssetMenu]
    public class GameplayPhase : GamePhase
    {
        protected override IEnumerator Execution(GameController gameController)
        {
            GameState.OnGameStateChanged += HandleGameStateChanged;
            GameState.InGameplay = true;
            Input.Context = InputContext.Gameplay;
            
            while (true)
            {
                yield return null;
            }
            
            GameState.OnGameStateChanged -= HandleGameStateChanged;
        }

        private void HandleGameStateChanged()
        {
            if (GameState.InGameplay) Input.Context = InputContext.Gameplay;
        }
    }
}