using System.IO;
using System.Diagnostics;
using UnityEngine;
using Cysharp.Threading.Tasks;

using Application = UnityEngine.Application;
using Debug = UnityEngine.Debug;


public class FilePath : MonoBehaviour
{
    public static FilePath Instance;

    // file download path
    private string rootPath;

    public string[] buildFileUrls = new string[5];
    public string[] jsonFileUrls = new string[5];

    private string[] exeFolderPaths = new string[5];
    private string[] exeZipFilePaths = new string[5];

    public string RootPath => rootPath;
    public string[] BuildFileUrls => buildFileUrls;
    public string[] JsonFileUrls => jsonFileUrls;
    public string[] ExeFolderPaths => exeFolderPaths;
    public string[] ExeZipFilePaths => exeZipFilePaths;

    private string[] exeFolderNames = new string[5];
    public string[] ExeFolderNames => exeFolderNames;
    
    [Header("[ Download File Path ]")]
    public DataPath dataPath;
    public string defaultDataPath;
    public string[] rootPaths = new string[5];
    public string[] RootPaths => rootPaths;

    public DownloadURL downloadURL;

    public UGCManager ugcManager;

    public string fileCheckText = "File Check";

    [Header("[ Select Server ]")]
    public GameObject settingSelectServer;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        ugcManager = GetComponent<UGCManager>();
        InitDataPath();
    }

    //---------- new version ----------//
    public void InitDataPath()
    {
        string path = Path.Combine(Application.streamingAssetsPath + "/Data Path", "dataPath.json");
        string jsonData = File.ReadAllText(path);
        dataPath = JsonUtility.FromJson<DataPath>(jsonData);

        rootPaths[0] = dataPath.pcPath;
        rootPaths[1] = dataPath.vrPath;
        //rootPaths[2] = dataPath.ugcPath; // ugc는 별도로 저장
        rootPaths[3] = dataPath.batchPath;

        rootPaths[2] = ugcManager.LoadUGCFilePath().objectUGCProjectDownloadPath;
    }

    // save path data
    public void SavePath(int buttonNum, string _path)
    {
        switch (buttonNum)
        {
            case 0:
                dataPath.pcPath = _path;
                break;
            case 1:
                dataPath.vrPath = _path;
                break;
            case 2:
                dataPath.ugcPath = _path;
                break;
            case 3:
                dataPath.batchPath = _path;
                break;
            case 4:
                dataPath.batchPath_admin = _path;
                break;
        }

        rootPaths[buttonNum] = _path;

        string jsonData = JsonUtility.ToJson(dataPath, true);
        string path = Path.Combine(Application.streamingAssetsPath + "/Data Path", "dataPath.json");
        File.WriteAllText(path, jsonData);

        SetFilePath();
    }

    // load path data
    public DataPath LoadPath()
    {
        string path = Path.Combine(Application.streamingAssetsPath + "/Data Path", "dataPath.json");
        string jsonData = File.ReadAllText(path);
        dataPath = JsonUtility.FromJson<DataPath>(jsonData);

        return dataPath;
    }

    public void ResetDataPath()
    {
        //defaultDataPath = "C:\\Curiverse";
        defaultDataPath = "C:\\MetaPly";
        dataPath.pcPath = dataPath.vrPath = dataPath.ugcPath = dataPath.batchPath = defaultDataPath;

        string jsonData = JsonUtility.ToJson(dataPath, true);
        string path = Path.Combine(Application.streamingAssetsPath + "/Data Path", "dataPath.json");
        File.WriteAllText(path, jsonData);

        InitDataPath();
    }
    
    //public async void SetDownloadURL(int serverNum)
    //public async void SetDownloadURL(int serverNum, bool isDeleteMode = false)
    public async void SetDownloadURL(int serverNum, int buttonNum = 0, bool isDeleteMode = false)
    {
        Debug.Log("SetDownloadURL 함수 호출");
        if (CheckRunningFiles())
            return;

        API api = GameManager.instance.api;

        //switch (GameManager.instance.selectedServerNum)
        switch (serverNum)
        {
            case 0:
                // dev server
                Debug.Log("[SY] dev server");
                await api.Request_FileDownloadURL(ServerType.dev, FileType.pc);
                await api.Request_FileDownloadURL(ServerType.dev, FileType.vr);
                await api.Request_FileDownloadURL(ServerType.dev, FileType.prod);
                await api.Request_FileDownloadURL(ServerType.dev, FileType.colca);
                await api.Request_FileDownloadURL(ServerType.dev, FileType.admin);
                break;
            case 1:
                // test server
                Debug.Log("[SY] test server");
                await api.Request_FileDownloadURL(ServerType.test, FileType.pc);
                await api.Request_FileDownloadURL(ServerType.test, FileType.vr);
                await api.Request_FileDownloadURL(ServerType.test, FileType.prod);
                await api.Request_FileDownloadURL(ServerType.test, FileType.colca);
                await api.Request_FileDownloadURL(ServerType.test, FileType.admin);
                break;
            case 2:
                // staging server
                Debug.Log("[SY] staging server");
                await api.Request_FileDownloadURL(ServerType.stage, FileType.pc);
                await api.Request_FileDownloadURL(ServerType.stage, FileType.vr);
                await api.Request_FileDownloadURL(ServerType.stage, FileType.prod);
                await api.Request_FileDownloadURL(ServerType.stage, FileType.colca);
                await api.Request_FileDownloadURL(ServerType.stage, FileType.admin);
                break;
            case 3:
                // live server
                Debug.Log("[SY] live server");
                await api.Request_FileDownloadURL_live(FileType.pc);
                await api.Request_FileDownloadURL_live(FileType.vr);
                await api.Request_FileDownloadURL_live(FileType.prod);
                await api.Request_FileDownloadURL_live(FileType.colca);
                await api.Request_FileDownloadURL_live(FileType.admin);
                break;
        }

        if (isDeleteMode)
        {
            //for (int i = 0; i < 4; i++)
            for (int i = 0; i < 5; i++)
            {
                buildFileUrls[i] = GameManager.instance.jsonData.temp_donwloadUrlList[i].zip_path;
                jsonFileUrls[i] = GameManager.instance.jsonData.temp_donwloadUrlList[i].json_path;
                SaveDownloadURL(i, buildFileUrls[i]);
            }
            SetFilePath();
            GameManager.instance.SetSelectButton(buttonNum);
        }
        else
        {
            CompareToFileDownloadURL(buttonNum);
        }
    }

    public void CompareToFileDownloadURL(int buttonNum)
    {
        if (GameManager.instance.jsonData.temp_donwloadUrlList.Count <= 0)
        {
            return;
        }

        Debug.Log("[Compare] 비교 시작");
        DownloadURL temp_downloadURL = LoadDownloadURL();

        for (int i = 0; i < buildFileUrls.Length; i++)
        {
            //if (BuildFileUrls[i] == "")
            if (temp_downloadURL.downloadURLs[i] == "")
            {
                Debug.Log($"[Compare] {i}번 공백 초기화");
                buildFileUrls[i] = GameManager.instance.jsonData.temp_donwloadUrlList[i].zip_path;
                jsonFileUrls[i] = GameManager.instance.jsonData.temp_donwloadUrlList[i].json_path;

                SaveDownloadURL(i, buildFileUrls[i]);
            }
            //else if (buildFileUrls[i] != GameManager.instance.jsonData.temp_donwloadUrlList[i].zip_path)
            else if (temp_downloadURL.downloadURLs[i] != GameManager.instance.jsonData.temp_donwloadUrlList[i].zip_path)
            {
                Debug.Log("[Compare] 경로 변경됨");
                Debug.Log($"[Compare] : %%{exeFolderPaths[i]}$$");
                if (exeFolderPaths[i] == "")
                {
                    Debug.Log($"[Compare] : {exeFolderPaths[i]}");
                    // 임시 저장
                    buildFileUrls[i] = temp_downloadURL.downloadURLs[i];

                    // set path
                    string[] folderFullName = buildFileUrls[i].Split("/");
                    string[] exeFolderName = folderFullName[folderFullName.Length - 1].Split(".");

                    exeFolderNames[i] = exeFolderName[0];
                    exeFolderPaths[i] = Path.Combine(rootPaths[i], exeFolderName[0]);
                    exeZipFilePaths[i] = Path.Combine(rootPaths[i], exeFolderPaths[i] + ".zip");
                    Debug.Log($"[Compare] : {exeFolderPaths[i]}");
                }
                else if (exeFolderPaths[i] == null)
                {
                    Debug.Log($"[Compare] : {exeFolderPaths[i]}");
                    // 임시 저장
                    buildFileUrls[i] = temp_downloadURL.downloadURLs[i];

                    // set path
                    string[] folderFullName = buildFileUrls[i].Split("/");
                    string[] exeFolderName = folderFullName[folderFullName.Length - 1].Split(".");

                    exeFolderNames[i] = exeFolderName[0];
                    exeFolderPaths[i] = Path.Combine(rootPaths[i], exeFolderName[0]);
                    exeZipFilePaths[i] = Path.Combine(rootPaths[i], exeFolderPaths[i] + ".zip");
                    Debug.Log($"[Compare] : {exeFolderPaths[i]}");
                }

                // Todo : 2023.12.11 update check
                // 이미 설치된 파일 (최신) & 런처가 가지고 있는 경로가 구버전일 경우 (파일 이름이 다른 경우)
                string[] tempFolderFullName = GameManager.instance.jsonData.temp_donwloadUrlList[i].zip_path.Split("/");
                string[] tempExeFolderName = tempFolderFullName[tempFolderFullName.Length - 1].Split(".");
                string tempExeFolderPath = Path.Combine(rootPaths[i], tempExeFolderName[0]);

                // 파일 삭제
                // 이미 설치된 파일 (구버전) & 런처가 가지고 있는 경로가 구버전일 경우
                if (Directory.Exists(exeFolderPaths[i]))
                {
                    Debug.Log("[Compare] 파일 존재1 " + exeFolderPaths[i]);
                    GameManager.instance.SelectButtons[i].isNeedUpdate = true;
                }
                // Todo : 2023.12.11 update check
                // 이미 설치된 파일(최신) & 런처가 가지고 있는 경로가 구버전일 경우 (파일 이름이 다른 경우)
                else if (Directory.Exists(tempExeFolderPath)) 
                {
                    Debug.Log("[Compare] 파일 존재2 " + tempExeFolderPath);

                    // set path
                    string[] folderFullName = GameManager.instance.jsonData.temp_donwloadUrlList[i].zip_path.Split("/");
                    string[] exeFolderName = folderFullName[folderFullName.Length - 1].Split(".");

                    exeFolderNames[i] = exeFolderName[0];
                    exeFolderPaths[i] = Path.Combine(rootPaths[i], exeFolderName[0]);
                    exeZipFilePaths[i] = Path.Combine(rootPaths[i], exeFolderPaths[i] + ".zip");

                    // download path update
                    buildFileUrls[i] = GameManager.instance.jsonData.temp_donwloadUrlList[i].zip_path;
                    jsonFileUrls[i] = GameManager.instance.jsonData.temp_donwloadUrlList[i].json_path;
                    SaveDownloadURL(i, buildFileUrls[i]);
                }
                else
                {
                    Debug.Log("[Compare] 파일 없음");
                    GameManager.instance.SelectButtons[i].isNeedUpdate = false;
                }
            }
            else if (temp_downloadURL.downloadURLs[i] == GameManager.instance.jsonData.temp_donwloadUrlList[i].zip_path)
            {
                Debug.Log("[Compare] 파일 경로 같음");
                buildFileUrls[i] = GameManager.instance.jsonData.temp_donwloadUrlList[i].zip_path;
                jsonFileUrls[i] = GameManager.instance.jsonData.temp_donwloadUrlList[i].json_path;

                SaveDownloadURL(i, buildFileUrls[i]);

                // set path
                string[] folderFullName = buildFileUrls[i].Split("/");
                string[] exeFolderName = folderFullName[folderFullName.Length - 1].Split(".");

                exeFolderNames[i] = exeFolderName[0];
                exeFolderPaths[i] = Path.Combine(rootPaths[i], exeFolderName[0]);
                exeZipFilePaths[i] = Path.Combine(rootPaths[i], exeFolderPaths[i] + ".zip");
            }
        }
        Debug.Log("[Compare] 비교 완료");
        GameManager.instance.SetSelectButton(buttonNum);
    }

    public bool CheckRunningFiles()
    {
        Process[] _runningFiles = GameManager.instance.runningFiles;
        bool isRunning = false;

        for (int i = 0; i < _runningFiles.Length; i++)
        {
            if (_runningFiles[i] != null)
            {
                if (!_runningFiles[i].HasExited)
                {
                    isRunning = true;
                    break;
                }
                else
                {
                    isRunning = false;
                }
            }
        }
        Debug.Log("[check Running files] 실행파일 상태 확인 결과 : " + isRunning);
        return isRunning;
    }

    private void SetFilePath()
    {
        //for (int i = 0; i < 4; i++)
        for (int i = 0; i < 5; i++)
        {
            string[] folderFullName = buildFileUrls[i].Split("/");
            string[] exeFolderName = folderFullName[folderFullName.Length - 1].Split(".");

            exeFolderNames[i] = exeFolderName[0];
            exeFolderPaths[i] = Path.Combine(rootPaths[i], exeFolderName[0]);
            exeZipFilePaths[i] = Path.Combine(rootPaths[i], exeFolderPaths[i] + ".zip");
        }
    }

    public string ChangeDeleteFileName(int buttonNum)
    {
        Debug.Log("[check Running files] 이름 변경 시작");
        Directory.Move(exeFolderPaths[buttonNum], exeFolderPaths[buttonNum] + "_temp");
        Debug.Log("[check Running files] 이름 변경 완료");

        return exeFolderPaths[buttonNum] + "_temp";
    }

    public void SetNewPaht(int buttonNum)
    {
        Debug.Log("[check Running files] 갱신 시작");
        // 경로 갱신
        buildFileUrls[buttonNum] = GameManager.instance.jsonData.temp_donwloadUrlList[buttonNum].zip_path;
        jsonFileUrls[buttonNum] = GameManager.instance.jsonData.temp_donwloadUrlList[buttonNum].json_path;
        //SaveDownloadURL(buttonNum, buildFileUrls[buttonNum]);
        Debug.Log("[check Running files] 갱신 완료");

        SetFilePath();
        Debug.Log("[check Running files] 세팅 완료");
    }

    public async UniTaskVoid DeleteOldFile(string deleteFilePath)
    {
        await UniTask.SwitchToThreadPool();
        Debug.Log("[old file delete] 삭제 시작");
        Directory.Delete(deleteFilePath, true);
        Debug.Log("[old file delete] 삭제 완료");
        await UniTask.SwitchToMainThread();
    }
    //---------------------------------------------//

    #region File Path Check
    //public async UniTaskVoid DeleteExeFiles(int serverNum)
    public void DeleteExeFiles(int serverNum)
    {
        FileDownload[] selectButtons = GameManager.instance.SelectButtons;

        // reset button state
        for (int i = 0; i < selectButtons.Length; i++)
        {
            FileDownload fileDownload = selectButtons[i].GetComponent<FileDownload>();

            fileDownload.Status = LauncherStatus.downloadGame;
            fileDownload.isNeedUpdate = false;

            fileDownload.excuteButton.interactable = true;
            
            fileDownload.prograss.gameObject.SetActive(false);
            fileDownload.prograss.ResetState();

            fileDownload.donwloadStateMessage_1.SetActive(false);
            fileDownload.donwloadStateMessage_2.SetActive(false);

            //fileDownload.CheckBuidDirectory();

            DEV.instance.isFileDownload = false;

            if (DEV.instance.isProtectFileDownload)
            {
                // disable protect guard
                DEV.instance.downloadProtectGaurd.SetActive(DEV.instance.isFileDownload);
            }
        }
        
        // update download url
        SetDownloadURL(serverNum, 0, true);

        // delete all files
        if (Directory.Exists(defaultDataPath))
        {
            // delete all files in a directory
            string[] files = Directory.GetFiles(defaultDataPath);
            foreach (string file in files)
            {
                File.Delete(file);
            }
            // delete all subdirectories within a directory
            string[] subdirectories = Directory.GetDirectories(defaultDataPath);
            foreach (string subdirectory in subdirectories)
            {
                Directory.Delete(subdirectory, recursive: true);
            }
        }
    }
    #endregion

    public void SaveDownloadURL(int i, string newDonwloadURL)
    {
        downloadURL.downloadURLs[i] = newDonwloadURL;

        string jsonData = JsonUtility.ToJson(downloadURL, true);
        string path = Path.Combine(Application.streamingAssetsPath + "/Data Path", "downloadURL.json");
        File.WriteAllText(path, jsonData);
    }

    public DownloadURL LoadDownloadURL()
    {
        string path = Path.Combine(Application.streamingAssetsPath + "/Data Path", "downloadURL.json");
        string jsonData = File.ReadAllText(path);
        downloadURL = JsonUtility.FromJson<DownloadURL>(jsonData);

        return downloadURL;
    }

    public void ResetDownloadURL()
    {
        //for (int i = 0; i < 4; i++)
        for (int i = 0; i < 5; i++)
        {
            downloadURL.downloadURLs[i] = null;
        }

        string jsonData = JsonUtility.ToJson(downloadURL, true);
        string path = Path.Combine(Application.streamingAssetsPath + "/Data Path", "downloadURL.json");
        File.WriteAllText(path, jsonData);
    }
}

[System.Serializable]
public class DataPath
{
    public string pcPath;
    public string vrPath;
    public string ugcPath;
    public string batchPath;
    public string batchPath_admin;
}

[System.Serializable]
public class DownloadURL
{
    //public string[] downloadURLs = new string[4];
    public string[] downloadURLs = new string[5];
}