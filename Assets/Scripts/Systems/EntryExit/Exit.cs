#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Summoner.Systems.EntryExit
{
    public static class Exit
    {
        public static void QuitApplication()
        {
#if UNITY_EDITOR
            EditorApplication.ExitPlaymode();
#else
            Application.Quit();
#endif
        }
    }
}