using UnityEngine;
using UnityEditor;

#if UNITY_EDITOR
[CustomEditor(typeof(UrgentNoticeManager))]
public class UrgentNoticeEditorManager : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        UrgentNoticeManager urgentNotice = (UrgentNoticeManager)target;

        if (GUILayout.Button("Urgent Notice Data Reset"))
        {
            Debug.Log("Reset Data!!!");
            urgentNotice.ResetCheckData();
        }
    }
}
#endif