using System.Collections;
using System.Collections.Generic;
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
            dev.filePath.ResetDownloadInfoData();
            //dev.selectServer.ResetSelectedServer();
            //dev.ResetSettingValue();
        }
    }
}
#endif