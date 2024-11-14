using System;
using System.Collections.Generic;
using Summoner.Editor.CustomBuildUtilities.Enums;

namespace Summoner.Editor.CustomBuildUtilities
{
    internal static class ScriptingDefineHelper
    {
        private static string[] DefaultScriptingDefines => new[]
        {
            "UNITY_POST_PROCESSING_STACK_V2",
            "DOTWEEN"
        };
        
        private static string[] DebugScriptingDefines => new[] { "SUMMONER_DEBUG" };
        private static string[] PlaytestScriptingDefines => new[] { "SUMMONER_PLAYTEST" };
        private static string[] ReleaseScriptingDefines => new[] { "SUMMONER_RELEASE" };
        
        private static string[] SteamScriptingDefines => new[] { "STEAMWORKS" };
        private static string[] GOGScriptingDefines => new[] { "GOG" };
        private static string[] EpicScriptingDefines => new[] { "EPIC" };
        
        internal static string[] GetPresetScriptingDefines(Branch branch, Store store)
        {
            List<string> scriptingDefines = new();
            
            scriptingDefines.AddRange(DefaultScriptingDefines);

            switch (branch)
            {
                case Branch.None:
                    break;
                case Branch.Debug:
                    scriptingDefines.AddRange(DebugScriptingDefines);
                    break;
                case Branch.Playtest:
                    scriptingDefines.AddRange(PlaytestScriptingDefines);
                    break;
                case Branch.Release:
                    scriptingDefines.AddRange(ReleaseScriptingDefines);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(branch), branch, null);
            }

            switch (store)
            {
                case Store.None:
                    break;
                case Store.Steam:
                    scriptingDefines.AddRange(SteamScriptingDefines);
                    break;
                case Store.GOG:
                    scriptingDefines.AddRange(GOGScriptingDefines);
                    break;
                case Store.Epic:
                    scriptingDefines.AddRange(EpicScriptingDefines);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(store), store, null);
            }

            return scriptingDefines.ToArray();
        }
    }
}