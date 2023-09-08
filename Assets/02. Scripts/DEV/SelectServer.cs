using UnityEngine;
using TMPro;
using System.IO;

public enum ServerType
{
    dev,
    test,
    live,
    stage,
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
    }

    public void SetTestServer()
    {
        DEV.instance.isAdmin = true;

        //selectServerNum = 0;
        selectServerNum = LoadData().selectedServerNum;
        selectServer.value = selectServerNum;

        FilePath.Instance.Test_SetDownloadURL2(selectServerNum);
    }

    public void SetLiveServer()
    {
        DEV.instance.isAdmin = false;

        //selectServer.value = selectServerNum;
        selectServerNum = (int)ServerType.live;
        selectServer.value = (int)ServerType.live;

        FilePath.Instance.Test_SetDownloadURL2(selectServerNum);
    }

    public void OnChangedValue(TMP_Dropdown change)
    {
        if (FilePath.Instance.CheckRunningFiles())
        {
            Debug.Log("[SY] 실행 중인 파일 존재 & drapdown 원래대로 변경");
            selectServer.value = selectServerNum;
            return;
        }

        if (selectServerNum != change.value)
        {
            Debug.Log("삭제");
            //FilePath.Instance.FilePathCheck();
            FilePath.Instance.DeleteExeFiles(change.value).Forget();
        }

        selectServerNum = change.value;
        
        if (DEV.instance.isAdmin)
        {
            SaveData(selectServerNum);
        }
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
