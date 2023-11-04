using UnityEngine;
using UnityEditor;

#if UNITY_EDITOR
[CustomEditor(typeof(SelectServer))]
public class SelectServerManager : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        SelectServer selectServer = (SelectServer)target;

        if (GUILayout.Button("Selected Server Reset"))
        {
            Debug.Log("Reset Data!!!");
            selectServer.ResetSelectedServer();
        }
    }
}
#endif