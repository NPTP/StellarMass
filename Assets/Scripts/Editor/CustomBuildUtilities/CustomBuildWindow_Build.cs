using System;
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
        private void Build()
        {
            if (preprocessBuild)
            {
                PreProcess();
            }

            BuildTarget buildTarget = (BuildTarget)platform;
            BuildTargetGroup buildTargetGroup = BuildTargetGroup.Standalone;
            string targetPath = Path.Combine(Application.dataPath, "../") + $"/{buildPath}/{executableName}.exe";
            string fullBuildPath = Path.GetFullPath(targetPath);
            string buildDirectory = Path.GetDirectoryName(fullBuildPath);

            PlayerSettings.SetScriptingDefineSymbolsForGroup(buildTargetGroup, scriptingDefines.ToArray());

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

            if (developmentBuild) buildOptions |= BuildOptions.Development;
            if (autoconnectProfiler) buildOptions |= BuildOptions.ConnectWithProfiler;
            if (deepProfilingSupport) buildOptions |= BuildOptions.EnableDeepProfilingSupport;
            if (scriptDebugging) buildOptions |= BuildOptions.AllowDebugging;
            switch (compressionMethod)
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
            
            string[] enabledBuildScenes = (from scene in EditorBuildSettings.scenes where scene.enabled select scene.path).ToArray();

            BuildReport buildReport = BuildPipeline.BuildPlayer(enabledBuildScenes, fullBuildPath, buildTarget, buildOptions);
            
            Debug.Log($"Build completed: {fullBuildPath}, {DateTime.Now}");
            if (buildReport.summary.result == BuildResult.Succeeded)
            {
                EditorUtility.OpenWithDefaultApp(buildDirectory);
            }
        }
    }
}