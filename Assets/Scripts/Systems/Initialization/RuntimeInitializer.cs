using UnityEngine;
using Input = StellarMass.Systems.InputManagement.Input;

namespace StellarMass.Systems.Initialization
{
    public static class RuntimeInitializer
    {
        public static bool FirstSceneLoadCompleted { get; private set; }
        
        /// <summary>
        /// Step 1: Callback invoked when starting up the runtime. Called before the first scene is loaded.
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void SubsystemRegistration()
        {
        }

        /// <summary>
        /// Step 2: Callback invoked when all assemblies are loaded and preloaded assets are initialized. At this time the objects of the first scene have not been loaded yet.
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
        private static void AfterAssembliesLoaded()
        {
        }

        /// <summary>
        /// Step 3: Callback invoked before the splash screen is shown. At this time the objects of the first scene have not been loaded yet.
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
        private static void BeforeSplashScreen()
        {
        }

        /// <summary>
        /// Step 4: Callback invoked when the first scene's objects are loaded into memory but before Awake has been called.
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void BeforeSceneLoad()
        {
            Input.InitializeBeforeSceneLoad();
        }

        /// <summary>
        /// Step 5: Callback invoked when the first scene's objects are loaded into memory and after Awake & OnEnable has been called.
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void AfterSceneLoad()
        {
            FirstSceneLoadCompleted = true;
            
            Input.InitializeAfterSceneLoad();
        }
    }
}