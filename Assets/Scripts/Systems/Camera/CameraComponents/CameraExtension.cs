using Summoner.Utilities.Extensions;
using UnityEngine;

namespace Summoner.Systems.Camera.CameraComponents
{
    public abstract class CameraExtension : MonoBehaviour
    {
        private bool initialized;

        public static void Create<T>(CameraController cameraController) where T : CameraExtension
        {
            T customUpdater = cameraController.gameObject.GetOrAddComponent<T>();
            customUpdater.OnCreated(cameraController);
        }
        
        public static void Remove<T>(CameraController cameraController) where T : CameraExtension
        {
            cameraController.gameObject.DestroyComponent<T>();
        }

        protected abstract void OnCreated(CameraController cameraController);

        protected abstract void Update();
    }
}