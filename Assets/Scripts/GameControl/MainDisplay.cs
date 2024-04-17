using StellarMass.Animation;
using UnityEngine;

namespace StellarMass.GameControl
{
    public class MainDisplay : MonoBehaviour
    {
        [SerializeField] private GameObject title;
        public GameObject Title => title;
        [SerializeField] private GameObject prompt;
        public GameObject Prompt => prompt;
        [SerializeField] private GameObject stars;
        public GameObject Stars => stars;
        [SerializeField] private AnimatorController animatorController;
        public AnimatorController AnimatorController => animatorController;

        public void TurnOn()
        {
            this.gameObject.SetActive(true);
        }
        
        public void TurnOff()
        {
            this.gameObject.SetActive(false);
        }
    }
}