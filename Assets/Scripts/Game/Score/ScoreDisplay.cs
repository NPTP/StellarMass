using TMPro;
using UnityEngine;

namespace Summoner.Game.Score
{
    public class ScoreDisplay : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI textMeshPro;

        public void SetScore(int score)
        {
            textMeshPro.text = $"{score:0000}";
        }
    }
}