using TMPro;
using UnityEngine;

namespace StellarMass.Game.Score
{
    public class ScoreDisplay : MonoBehaviour
    {
        [SerializeField] private TextMeshPro textMeshPro;

        public void SetScore(int score)
        {
            textMeshPro.text = $"{score:0000}";
        }
    }
}