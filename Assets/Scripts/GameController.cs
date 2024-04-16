using StellarMass.Animation;
using StellarMass.Input;
using UnityEngine;

namespace StellarMass
{
    public class GameController : MonoBehaviour
    {
        [SerializeField] private GameObject title;
        [SerializeField] private GameObject prompt;
        [SerializeField] private GameObject player;
        [SerializeField] private AnimatorController animatorController;
        
        private readonly int startupAnimation = Animator.StringToHash("Startup");

        private void OnDestroy()
        {
            InputReceiver.RemoveListeners(Inputs.Accept, HandleAcceptDown);
        }

        private void Start()
        {
#if !UNITY_EDITOR
            Cursor.visible = false;
#endif

            animatorController.OnAnimationCompleted += handleAnimationCompleted;
            animatorController.Play(startupAnimation);

            void handleAnimationCompleted()
            {
                InputReceiver.AddListeners(Inputs.Accept, HandleAcceptDown);
            }
        }

        private void HandleAcceptDown()
        {
            InputReceiver.RemoveListeners(Inputs.Accept, HandleAcceptDown);
            title.SetActive(false);
            prompt.SetActive(false);
            player.SetActive(true);
        }
    }
}