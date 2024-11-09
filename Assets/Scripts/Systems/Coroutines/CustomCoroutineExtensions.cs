namespace Summoner.Systems.Coroutines
{
    public static class CoroutineExtensions
    {
        public static CustomCoroutine StopOnSceneUnload(this CustomCoroutine customCoroutine)
        {
            customCoroutine.lifecycleMode = CustomCoroutine.LifecycleMode.SceneIsolated;
            return customCoroutine;
        }
    }
}