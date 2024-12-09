using UnityEngine;

namespace Summoner.Utilities.VFX
{
    public class RendererController : MonoBehaviour
    {
        [SerializeField] protected Renderer[] renderers;

        private bool[] lastActiveStatus;
        
        private void OnValidate()
        {
            renderers = GetComponentsInChildren<Renderer>();
        }

        private void Start()
        {
            lastActiveStatus = new bool[renderers.Length];
            for (int i = 0; i < renderers.Length; i++)
            {
                lastActiveStatus[i] = renderers[i].enabled;
            }
        }

        public void EnableRenderers()
        {
            for (int i = 0; i < renderers.Length; i++)
            {
                renderers[i].enabled = lastActiveStatus[i];
            }
        }

        public void DisableRenderers()
        {
            for (int i = 0; i < renderers.Length; i++)
            {
                lastActiveStatus[i] = renderers[i].enabled;
                renderers[i].enabled = false;
            }
        }
    }
}