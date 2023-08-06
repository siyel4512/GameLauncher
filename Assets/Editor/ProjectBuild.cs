using System;
using System.IO;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEditor;
using UnityEditor.Build.Reporting;
using Ookii.Dialogs;

using MenuItem = UnityEditor.MenuItem;
using Application = UnityEngine.Application;

public class ProjectBuild : MonoBehaviour
{
    [DllImport("user32.dll")]
    private static extern IntPtr GetActiveWindow();

    public static Version version = new Version();

    public static string OpenFolderDialog()
    {
        VistaFolderBrowserDialog openDialog = new VistaFolderBrowserDialog();
        openDialog.Description = "Select Folder";
        openDialog.UseDescriptionForTitle = true;

        string path = "";

        if (openDialog.ShowDialog(new WindowWrapper(GetActiveWindow())) == DialogResult.OK)
        {
            Debug.Log(openDialog.SelectedPath);
            path = openDialog.SelectedPath;
        }

        return path;
    }

    [MenuItem("Launcher Build/Current Version Check")]
    public static void CheckVersion()
    {
        UnityEngine.Debug.Log(PlayerSettings.bundleVersion);
    }

    [MenuItem("Launcher Build/Major Version")]
    public static void IncreaseMajorVersion()
    {
        SetBundleVersion(0);
    }

    [MenuItem("Launcher Build/Minor Version")]
    public static void IncreaseMinorVersion()
    {
        SetBundleVersion(1);
    }

    [MenuItem("Launcher Build/Patch Version")]
    public static void IncreasePatchVersion()
    {
        SetBundleVersion(2);
    }

    public static void SetBundleVersion(int updateBundleNumber)
    {
        //------------------- select build file path -------------------//
        // select build file path
        string outputPath = OpenFolderDialog();

        // cancel build
        if (string.IsNullOrEmpty(outputPath))
        {
            return;
        }


        //------------------- update bunndle version -------------------//
        // parsing version values
        string[] versionNumbers = PlayerSettings.bundleVersion.Split('.');
        string[] temp_versionNumbers = new string[versionNumbers.Length];
        int[] parsingNumbers = new int[versionNumbers.Length];

        for (int i = 0; i < versionNumbers.Length; i++)
        {
            parsingNumbers[i] = int.Parse(versionNumbers[i]);
        }

        // update version number
        switch (updateBundleNumber)
        {
            case 0:
                parsingNumbers[0]++;
                parsingNumbers[1] = 0;
                parsingNumbers[2] = 0;
                break;
            case 1:
                parsingNumbers[1]++;
                parsingNumbers[2] = 0;
                break;
            case 2:
                parsingNumbers[2]++;
                break;
        }

        for (int i = 0; i < parsingNumbers.Length; i++)
        {
            temp_versionNumbers[i] = parsingNumbers[i].ToString();
        }

        // set in project
        PlayerSettings.bundleVersion = $"{temp_versionNumbers[0]}.{temp_versionNumbers[1]}.{temp_versionNumbers[2]}";

        // set in build file
        SaveVersion(temp_versionNumbers);


        //------------------------- file build -------------------------//
        // build setting
        BuildTarget buildTarget = BuildTarget.StandaloneWindows64;
        BuildOptions buildOptions = BuildOptions.None;

        // set new folder name
        string newFolderName = PlayerSettings.productName + ".exe";

        // create new folder
        string newFolderPath = System.IO.Path.Combine(outputPath, newFolderName);

        // Check folder with the same name already exists
        if (System.IO.Directory.Exists(newFolderPath))
        {
            // Delete the folder that already exists and recreate it
            System.IO.Directory.Delete(newFolderPath, true);
        }

        // build project
        BuildReport buildReport = BuildPipeline.BuildPlayer(EditorBuildSettings.scenes, newFolderPath, buildTarget, buildOptions);

        // output build result file path
        BuildSummary buildSummary = buildReport.summary;
        UnityEngine.Debug.Log("빌드 파일 경로 : " + buildSummary.outputPath);
        UnityEngine.Debug.Log(PlayerSettings.bundleVersion);
    }

    // save build version in json file
    private static void SaveVersion(string[] versionValues)
    {
        version.major = versionValues[0];
        version.minor = versionValues[1];
        version.patch = versionValues[2];

        string jsonData = JsonUtility.ToJson(version, true);
        string path = Path.Combine(Application.streamingAssetsPath + "/Version", "version.json");
        File.WriteAllText(path, jsonData);
    }
}
