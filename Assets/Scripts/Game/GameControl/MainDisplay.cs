using Summoner.Systems.Animation;
using UnityEngine;

namespace Summoner.Game.GameControl
{
    public class MainDisplay : MonoBehaviour
    {
        [SerializeField] private GameObject title;
        public GameObject Title => title;
        [SerializeField] private GameObject score;
        public GameObject Score => score;
        [SerializeField] private GameObject prompt;
        public GameObject Prompt => prompt;
        [SerializeField] private GameObject stars;
        public GameObject Stars => stars;
        [SerializeField] private AnimatorController animatorController;
        public AnimatorController AnimatorController => animatorController;

        public void TurnOn()
        {
            gameObject.SetActive(true);
        }
        
        public void TurnOff()
        {
            gameObject.SetActive(false);
        }
    }
}