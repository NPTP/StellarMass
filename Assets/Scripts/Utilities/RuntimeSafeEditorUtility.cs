using System;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Summoner.Utilities
{
    /// <summary>
    /// Contains methods with editor-only functionality that are safe to call at runtime.
    /// </summary>
    public static class RuntimeSafeEditorUtility
    {
        public static bool IsDomainReloadDisabled()
        {
#if UNITY_EDITOR
            return EditorSettings.enterPlayModeOptionsEnabled &&
                   EditorSettings.enterPlayModeOptions.HasFlag(EnterPlayModeOptions.DisableDomainReload);
#endif

#pragma warning disable CS0162
            // Domain is always reloaded in a build.
            return false;
#pragma warning restore CS0162
        }

        public static void RegisterApplicationQuittingCallback(Action callback)
        {
            if (callback == null)
            {
                return;
            }
            
#if UNITY_EDITOR
            EditorApplication.playModeStateChanged -= handlePlayModeStateChanged;
            EditorApplication.playModeStateChanged += handlePlayModeStateChanged;
            void handlePlayModeStateChanged(PlayModeStateChange playModeStateChange)
            {
                if (playModeStateChange is PlayModeStateChange.ExitingPlayMode)
                {
                    EditorApplication.playModeStateChanged -= handlePlayModeStateChanged;
                    callback?.Invoke();
                }
            }
#else
            UnityEngine.Application.quitting -= callback;
            UnityEngine.Application.quitting += callback;
#endif
        }
    }
}