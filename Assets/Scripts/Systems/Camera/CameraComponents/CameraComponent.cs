using Summoner.Utilities.Extensions;
using UnityEngine;

namespace Summoner.Systems.Camera.CameraComponents
{
    public abstract class CameraComponent : MonoBehaviour
    {
        private bool initialized;

        public static void Create<T>(CameraController cameraController) where T : CameraComponent
        {
            T customUpdater = cameraController.gameObject.GetOrAddComponent<T>();
            customUpdater.OnCreated(cameraController);
        }
        
        public static void Remove<T>(CameraController cameraController) where T : CameraComponent
        {
            cameraController.gameObject.DestroyComponent<T>();
        }

        protected abstract void OnCreated(CameraController cameraController);

        protected abstract void Update();
    }
}