using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(DEV))]
public class FolderDialogManager : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        DEV dev = (DEV)target;

        if (GUILayout.Button("Reset Data Paths"))
        {
            Debug.Log("Reset Data Paths!!!");
            dev.filePath.ResetDataPath();
        }
    }
}
