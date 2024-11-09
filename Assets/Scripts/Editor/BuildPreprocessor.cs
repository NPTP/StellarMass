using UnityEditor.Build;

namespace Summoner.Editor
{
    public class BuildPreprocessor: BuildPlayerProcessor
    {
        // Callbacks with lower values are called before ones with higher values.
        public override int callbackOrder => 0;

        public override void PrepareForBuild(BuildPlayerContext buildPlayerContext)
        {
            
        }
    }
}