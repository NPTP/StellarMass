using UnityEngine;

namespace Summoner.Systems.Camera.CustomUpdaters
{
    public abstract class CustomUpdater : MonoBehaviour
    {
        private bool initialized;

        protected void OnEnable()
        {
            if (!initialized)
            {
                Initialize();
                initialized = true;
            }
        }
        
        protected virtual void Initialize() { }
        
        protected abstract void Update();
    }
}