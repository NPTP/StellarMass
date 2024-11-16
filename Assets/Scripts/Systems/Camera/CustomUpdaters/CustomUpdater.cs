using UnityEngine;

namespace Summoner.Systems.Camera.CustomUpdaters
{
    public abstract class CustomUpdater : MonoBehaviour
    {
        public virtual void Initialize() { }
        protected abstract void Update();
    }
}