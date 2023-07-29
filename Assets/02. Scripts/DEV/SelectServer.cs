using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.IO;

public enum ServerType
{
    dev,
    test,
    stage,
    live
}

public enum FileType
{
    pc,
    vr,
    prod,
    colca
}

public class SelectServer : MonoBehaviour
{
    public TMP_Dropdown selectServer;
    public int selectServerNum;
    public SeletedServerState selectedServerState;

    public string jsonFilePath;

    // Start is called before the first frame update
    void Start()
    {
        selectServer.onValueChanged.AddListener(delegate {
            OnChangedValue(selectServer);
        });

        jsonFilePath = Path.Combine(Application.streamingAssetsPath + "/Default Settings", "SeletedServerState.json");

        //selectServerNum = 0;
        selectServerNum = LoadData().selectedServerNum;
        selectServer.value = selectServerNum;


        //if (selectServerNum == 0)
        //{
        //    GameManager.instance.filePath.FilePathCheck();
        //}

        //FilePath.Instance.Test_SetDownloadURL();
        FilePath.Instance.Test_SetDownloadURL2(selectServerNum);
    }

    public void OnChangedValue(TMP_Dropdown change)
    {
        if (selectServerNum != change.value)
        {
            //FilePath.Instance.FilePathCheck();
            FilePath.Instance.DeleteExeFiles(change.value);
        }

        selectServerNum = change.value;
        SaveData(selectServerNum);
        GameManager.instance.selectedServerNum = selectServerNum;

        //Debug.Log("Server Value: " + change.value);
        Debug.Log("Server Name: " + change.options[change.value].text);
        //Debug.Log("Server captionText: " + change.captionText.text);
    }

    // save server num data
    public void SaveData(int _serverNum)
    {
        selectedServerState.selectedServerNum = _serverNum;

        if (DEV.instance.isTEST_Login)
        {
            DEV.instance.selectedServerNum = _serverNum;
        }

        string jsonData = JsonUtility.ToJson(selectedServerState, true);
        string serverNum = jsonFilePath;
        File.WriteAllText(serverNum, jsonData);
    }

    // load server num data
    public SeletedServerState LoadData()
    {
        string serverNum = jsonFilePath;
        string jsonData = File.ReadAllText(serverNum);
        selectedServerState = JsonUtility.FromJson<SeletedServerState>(jsonData);
        Debug.Log(selectedServerState.selectedServerNum);
        return selectedServerState;
    }

    public void ResetSelectedServer()
    {
        selectedServerState.selectedServerNum = 0;

        string jsonData = JsonUtility.ToJson(selectedServerState, true);
        string path = jsonFilePath;
        File.WriteAllText(path, jsonData);
    }
}

[System.Serializable]
public class SeletedServerState
{
    public int selectedServerNum;
}
