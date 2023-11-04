using UnityEngine;
using UnityEditor;

#if UNITY_EDITOR
[CustomEditor(typeof(DEV))]
public class EditorManager : Editor
{
    //DefaultSettings defaultSettings;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        DEV dev = (DEV)target;

        // reset default setting
        if (GUILayout.Button("Default Reset"))
        {
            Debug.Log("Reset Data!!!");
            dev.filePath.ResetDataPath();
            dev.filePath.ResetDownloadURL();
            dev.ugcManager.ResetUGCFilePath();
            
            //dev.filePath.ResetDownloadInfoData();
            //dev.selectServer.ResetSelectedServer();
            //dev.ResetSettingValue();
        }

        // reset version number
        if (GUILayout.Button("Version Reset"))
        {
            dev.versionManager.ResetVersion();
            PlayerSettings.bundleVersion = "0.0.0";
        }
    }
}
#endif