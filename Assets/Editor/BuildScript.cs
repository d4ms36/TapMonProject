using System;
using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

/// <summary>
/// Automatiza la build Android de TapMon desde Unity en modo batch.
/// Ejecutar con: -batchmode -quit -executeMethod BuildScript.BuildAndroidAdMob
/// </summary>
public static class BuildScript
{
    private const string OutputPath = "Builds/TapMon_AdMob.apk";
    private const string ScenePath = "Assets/Scenes/MainMenu.unity";

    /// <summary>
    /// Genera el APK Android con la escena principal y ADMOB_ENABLED activo.
    /// El SDK de Google Mobile Ads debe estar instalado antes de ejecutar este metodo.
    /// </summary>
    public static void BuildAndroidAdMob()
    {
        string projectPath = Directory.GetParent(Application.dataPath).FullName;
        string absoluteOutputPath = Path.Combine(projectPath, OutputPath.Replace('/', Path.DirectorySeparatorChar));
        string absoluteLogPath = Path.Combine(projectPath, "build.log");

        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(absoluteOutputPath));
            ConfigureBuildSettings();

            BuildReport report = BuildPipeline.BuildPlayer(
                new BuildPlayerOptions
                {
                    scenes = new[] { ScenePath },
                    locationPathName = absoluteOutputPath,
                    target = BuildTarget.Android,
                    options = BuildOptions.None
                });

            if (report.summary.result != BuildResult.Succeeded)
            {
                throw new BuildFailedException($"La build termino con estado {report.summary.result}. Revisa {absoluteLogPath}.");
            }

            long sizeBytes = new FileInfo(absoluteOutputPath).Length;
            Debug.Log($"[BuildScript] APK generado: {absoluteOutputPath} ({sizeBytes} bytes).");
        }
        catch (Exception exception)
        {
            Debug.LogError($"[BuildScript] Error: {exception}");
            EditorApplication.Exit(1);
            return;
        }

        EditorApplication.Exit(0);
    }

    private static void ConfigureBuildSettings()
    {
        EditorUserBuildSettings.SwitchActiveBuildTarget(BuildPipeline.GetBuildTargetGroup(BuildTarget.Android), BuildTarget.Android);

        EditorBuildSettings.scenes = new[]
        {
            new EditorBuildSettingsScene(ScenePath, true)
        };

        BuildTargetGroup targetGroup = BuildPipeline.GetBuildTargetGroup(BuildTarget.Android);
        string symbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(targetGroup);
        if (!symbols.Contains("ADMOB_ENABLED", StringComparison.Ordinal))
        {
            symbols = string.IsNullOrEmpty(symbols) ? "ADMOB_ENABLED" : symbols + ";ADMOB_ENABLED";
            PlayerSettings.SetScriptingDefineSymbolsForGroup(targetGroup, symbols);
        }

        PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARM64;
    }
}
