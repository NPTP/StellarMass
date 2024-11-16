using System.Collections.Generic;
using Summoner.Editor.CustomBuildUtilities.Enums;

namespace Summoner.Editor.CustomBuildUtilities
{
    internal struct CustomBuildOptions
    {
        public const string FILE_EXTENSION = "cbp";

        public string name;
        
        public string buildPath;
        public string executableName;
        public Platform platform;
        public bool developmentBuild;
        public bool autoconnectProfiler;
        public bool deepProfilingSupport;
        public bool scriptDebugging;
        public CompressionMethod compressionMethod;
        
        public Branch branch;
        public Store store;
        public bool preprocessBuild;
        public AfterBuild afterBuild;
        public List<string> extraScriptingDefines;

        public string[] ForcedScriptingDefines => ScriptingDefineHelper.GetPresetScriptingDefines(branch, store);
    }
}
