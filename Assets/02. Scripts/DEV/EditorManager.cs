using UnityEngine;
using UnityEditor;

#if UNITY_EDITOR
[CustomEditor(typeof(DEV))]
public class EditorManager : Editor
{
    DefaultSettings defaultSettings;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        DEV dev = (DEV)target;

        if (GUILayout.Button("Reset"))
        {
            Debug.Log("Reset Data!!!");
            dev.filePath.ResetDataPath();

            dev.versionManager.ResetVersion();
            PlayerSettings.bundleVersion = "0.0.0";
            //dev.filePath.ResetDownloadInfoData();
            //dev.selectServer.ResetSelectedServer();
            //dev.ResetSettingValue();
        }
    }
}
#endif