using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Summoner.Editor.CustomBuildUtilities.Enums;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace Summoner.Editor.CustomBuildUtilities
{
    public partial class CustomBuildWindow
    {
        private void BatchBuild()
        {
            List<CustomBuildOptions> toBuild = new();
            CustomBuildOptions[] presets = LoadAllPresets();
            foreach (CustomBuildOptions preset in presets)
            {
                if (!GetIncludeInBatchValueFromEditorPrefs(preset.name))
                {
                    continue;
                }
                
                CustomBuildOptions buildPreset = preset;
                buildPreset.buildPath = GetPresetBuildPathValueFromEditorPrefs(preset.name);
                toBuild.Add(buildPreset);
            }

            if (toBuild.Count == 0)
            {
                Debug.Log("Cannot batch build: no presets selected.");
                return;
            }

            Debug.Log("Batch build starting...");

            foreach (CustomBuildOptions customBuildOptions in toBuild)
            {
                Build(customBuildOptions);
            }
            
            Debug.Log($"Batch build operation completed: {DateTime.Now}");
        }
        
        private void Build(CustomBuildOptions options)
        {
            Debug.Log($"Building preset {options.name}...");
            
            if (options.preprocessBuild)
            {
                PreProcess();
            }

            BuildTarget buildTarget = (BuildTarget)options.platform;
            BuildTargetGroup buildTargetGroup = BuildTargetGroup.Standalone;
            string targetPath = Path.Combine(Application.dataPath, "../") + $"/{options.buildPath}/{options.executableName}.exe";
            string fullBuildPath = Path.GetFullPath(targetPath);
            string buildDirectory = Path.GetDirectoryName(fullBuildPath);
        
            if (!Directory.Exists(buildDirectory))
            {
                Directory.CreateDirectory(buildDirectory);
            }
            else
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(buildDirectory);
                foreach (FileInfo file in directoryInfo.GetFiles())
                    file.Delete();
                foreach (DirectoryInfo dir in directoryInfo.GetDirectories())
                    dir.Delete(true);
            }
        
            BuildOptions buildOptions = 0;
            
            if (options.developmentBuild) buildOptions |= BuildOptions.Development;
            if (options.autoconnectProfiler) buildOptions |= BuildOptions.ConnectWithProfiler;
            if (options.deepProfilingSupport) buildOptions |= BuildOptions.EnableDeepProfilingSupport;
            if (options.scriptDebugging) buildOptions |= BuildOptions.AllowDebugging;
            switch (options.compressionMethod)
            {
                case CompressionMethod.Default:
                    break;
                case CompressionMethod.LZ4:
                    buildOptions |= BuildOptions.CompressWithLz4;
                    break;
                case CompressionMethod.LZ4HC:
                    buildOptions |= BuildOptions.CompressWithLz4HC;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            switch (options.afterBuild)
            {
                case AfterBuild.Nothing:
                    break;
                case AfterBuild.OpenFolder:
                    buildOptions |= BuildOptions.ShowBuiltPlayer;
                    break;
                case AfterBuild.RunBuild:
                    buildOptions |= BuildOptions.AutoRunPlayer;
                    break;
                case AfterBuild.OpenFolderAndRunBuild:
                    buildOptions |= BuildOptions.ShowBuiltPlayer;
                    buildOptions |= BuildOptions.AutoRunPlayer;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        
            string[] combinedScriptingDefines = new string[options.extraScriptingDefines.Count + options.ForcedScriptingDefines.Length];
            for (int i = 0; i < options.ForcedScriptingDefines.Length; i++)
                combinedScriptingDefines[i] = options.ForcedScriptingDefines[i];
            for (int i = 0; i < options.extraScriptingDefines.Count; i++)
                combinedScriptingDefines[i + options.ForcedScriptingDefines.Length] = options.extraScriptingDefines[i];
        
            BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions()
            {
                scenes = (from scene in EditorBuildSettings.scenes where scene.enabled select scene.path).ToArray(),
                locationPathName = fullBuildPath,
                options = buildOptions,
                target = buildTarget,
                targetGroup = buildTargetGroup,
                extraScriptingDefines = combinedScriptingDefines
            };
            
            BuildReport buildReport = BuildPipeline.BuildPlayer(buildPlayerOptions);
            
            Debug.Log($"Build completed: {buildReport.summary.result}, {fullBuildPath}, {DateTime.Now}");
        }
    }
}