using TMPro;
using UnityEngine;

namespace StellarMass.Score
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