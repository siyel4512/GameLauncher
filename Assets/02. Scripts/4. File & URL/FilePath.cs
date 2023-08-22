using System.IO;
using System.Diagnostics;
using UnityEngine;
using Cysharp.Threading.Tasks;

using Application = UnityEngine.Application;
using Debug = UnityEngine.Debug;


public class FilePath : LoadFile
{
    public static FilePath Instance;

    //---------- old version ----------//
    // file download path
    private string rootPath;

    public string[] buildFileUrls = new string[4];
    public string[] jsonFileUrls = new string[4];

    //public string[] temp_buildFileUrls = new string[4];
    //public string[] temp_jsonFileUrls = new string[4];

    private string[] exeFolderPaths = new string[4];
    private string[] exeZipFilePaths = new string[4];

    public string RootPath => rootPath;
    public string[] BuildFileUrls => buildFileUrls;
    public string[] JsonFileUrls => jsonFileUrls;
    public string[] ExeFolderPaths => exeFolderPaths;
    public string[] ExeZipFilePaths => exeZipFilePaths;

    private string[] exeFolderNames = new string[4];
    public string[] ExeFolderNames => exeFolderNames;
    //---------------------------------//

    //---------- new version ----------//
    [Header("[ Download File Path ]")]
    public DataPath dataPath;
    //public string defaultDataPath = "C:\\Program Files";
    public string defaultDataPath;
    public string[] rootPaths = new string[4];
    public string[] RootPaths => rootPaths;
    //---------------------------------//

    //public DownloadInfoData exeFilePath;
    public DownloadURL downloadURL;

    public string fileCheckText = "File Check";

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
        rootPaths[2] = dataPath.ugcPath;
        rootPaths[3] = dataPath.batchPath;

        if (DEV.instance != null && DEV.instance.isTEST_Login)
        {
            DEV.instance.rootPaths[0] = rootPaths[0];
            DEV.instance.rootPaths[1] = rootPaths[1];
            DEV.instance.rootPaths[2] = rootPaths[2];
            DEV.instance.rootPaths[3] = rootPaths[3];
        }
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
        }

        rootPaths[buttonNum] = _path;

        if (DEV.instance.isTEST_Login)
        {
            DEV.instance.rootPaths[buttonNum] = _path;
        }

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
        //defaultDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");
        //defaultDataPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        //defaultDataPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        //defaultDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
        defaultDataPath = "C:\\Curiverse";
        //defaultDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "TEST Folder");

        dataPath.pcPath = dataPath.vrPath = dataPath.ugcPath = dataPath.batchPath = defaultDataPath;

        string jsonData = JsonUtility.ToJson(dataPath, true);
        string path = Path.Combine(Application.streamingAssetsPath + "/Data Path", "dataPath.json");
        File.WriteAllText(path, jsonData);

        InitDataPath();
    }
    //---------------------------------//

    // read setting file content
    public void Test_SetDownloadURL()
    {
        string[] parsingData = ParsingData();

        switch (GameManager.instance.selectedServerNum)
        {
            case 0:
                // dev server
                buildFileUrls[0] = parsingData[5];
                buildFileUrls[1] = parsingData[7];
                buildFileUrls[2] = parsingData[9];
                buildFileUrls[3] = parsingData[11];

                jsonFileUrls[0] = parsingData[6];
                jsonFileUrls[1] = parsingData[8];
                jsonFileUrls[2] = parsingData[10];
                jsonFileUrls[3] = parsingData[12];

                // test
                //await GameManager.instance.api.Request_FileDownloadURL(ServerType.dev.ToString(), FileType.pc.ToString());
                break;
            case 1:
                // test server
                buildFileUrls[0] = parsingData[7];
                buildFileUrls[1] = parsingData[9];
                buildFileUrls[2] = parsingData[11];
                buildFileUrls[3] = parsingData[5];

                jsonFileUrls[0] = parsingData[8];
                jsonFileUrls[1] = parsingData[10];
                jsonFileUrls[2] = parsingData[12];
                jsonFileUrls[3] = parsingData[6];
                break;
            case 2:
                // staging server
                buildFileUrls[0] = parsingData[9];
                buildFileUrls[1] = parsingData[11];
                buildFileUrls[2] = parsingData[5];
                buildFileUrls[3] = parsingData[7];

                jsonFileUrls[0] = parsingData[10];
                jsonFileUrls[1] = parsingData[12];
                jsonFileUrls[2] = parsingData[6];
                jsonFileUrls[3] = parsingData[8];
                break;
            case 3:
                // live server
                buildFileUrls[0] = parsingData[11];
                buildFileUrls[1] = parsingData[5];
                buildFileUrls[2] = parsingData[7];
                buildFileUrls[3] = parsingData[9];

                jsonFileUrls[0] = parsingData[12];
                jsonFileUrls[1] = parsingData[6];
                jsonFileUrls[2] = parsingData[8];
                jsonFileUrls[3] = parsingData[10];
                break;
        }

        SetFilePath();
    }

    public async void Test_SetDownloadURL2(int serverNum)
    {
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
                break;
            case 1:
                // test server
                Debug.Log("[SY] test server");
                await api.Request_FileDownloadURL(ServerType.test, FileType.pc);
                await api.Request_FileDownloadURL(ServerType.test, FileType.vr);
                await api.Request_FileDownloadURL(ServerType.test, FileType.prod);
                await api.Request_FileDownloadURL(ServerType.test, FileType.colca);
                break;
            case 2:
                // staging server
                Debug.Log("[SY] staging server");
                await api.Request_FileDownloadURL(ServerType.stage, FileType.pc);
                await api.Request_FileDownloadURL(ServerType.stage, FileType.vr);
                await api.Request_FileDownloadURL(ServerType.stage, FileType.prod);
                await api.Request_FileDownloadURL(ServerType.stage, FileType.colca);
                break;
            case 3:
                // live server
                Debug.Log("[SY] live server");
                await api.Request_FileDownloadURL_live(FileType.pc);
                await api.Request_FileDownloadURL_live(FileType.vr);
                await api.Request_FileDownloadURL_live(FileType.prod);
                await api.Request_FileDownloadURL_live(FileType.colca);
                break;
        }

        //CompareToFileDownloadURL();

        for (int i = 0; i < 4; i++)
        {
            buildFileUrls[i] = GameManager.instance.jsonData.temp_donwloadUrlList[i].zip_path;
            jsonFileUrls[i] = GameManager.instance.jsonData.temp_donwloadUrlList[i].json_path;

            SaveDownloadURL(i, buildFileUrls[i]);
        }

        SetFilePath();

        GameManager.instance.SetSelectButton(0);
    }

    public void CompareToFileDownloadURL()
    {
        if (GameManager.instance.jsonData.temp_donwloadUrlList.Count <= 0)
        {
            return;
        }

        Debug.Log("[SY] 비교 시작");
        DownloadURL temp_downloadURL = LoadDownloadURL();

        for (int i = 0; i < buildFileUrls.Length; i++)
        {
            //if (BuildFileUrls[i] == "")
            if (temp_downloadURL.downloadURLs[i] == "")
            {
                Debug.Log($"[SY] {i}번 공백 초기화");
                buildFileUrls[i] = GameManager.instance.jsonData.temp_donwloadUrlList[i].zip_path;
                jsonFileUrls[i] = GameManager.instance.jsonData.temp_donwloadUrlList[i].json_path;

                SaveDownloadURL(i, buildFileUrls[i]);
            }
            //else if (buildFileUrls[i] != GameManager.instance.jsonData.temp_donwloadUrlList[i].zip_path)
            else if (temp_downloadURL.downloadURLs[i] != GameManager.instance.jsonData.temp_donwloadUrlList[i].zip_path)
            {
                Debug.Log("[SY] 경로 변경됨");

                // 파일 삭제
                if (Directory.Exists(exeFolderPaths[i]))
                {
                    Debug.Log("[SY] 파일 존재 " + exeFolderPaths[i]);
                    GameManager.instance.SelectButtons[i].isNeedDownload = true;

                }
                else
                {
                    Debug.Log("파일 없음");
                    GameManager.instance.SelectButtons[i].isNeedDownload = false;
                }

                // 임시 저장
                buildFileUrls[i] = temp_downloadURL.downloadURLs[i];
            }
            else if (temp_downloadURL.downloadURLs[i] == GameManager.instance.jsonData.temp_donwloadUrlList[i].zip_path)
            {
                buildFileUrls[i] = GameManager.instance.jsonData.temp_donwloadUrlList[i].zip_path;
                jsonFileUrls[i] = GameManager.instance.jsonData.temp_donwloadUrlList[i].json_path;

                SaveDownloadURL(i, buildFileUrls[i]);
            }
        }
        Debug.Log("[SY] 비교 완료");
        GameManager.instance.SetSelectButton(0);
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
        Debug.Log("[SY] 실행파일 상태 확인 결과 : " + isRunning);
        return isRunning;
    }

    private void SetFilePath()
    {
        for (int i = 0; i < 4; i++)
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
        Debug.Log("[SY] 이름 변경 시작");
        Directory.Move(exeFolderPaths[buttonNum], exeFolderPaths[buttonNum] + "_temp");
        Debug.Log("[SY] 이름 변경 완료");

        return exeFolderPaths[buttonNum] + "_temp";
    }

    public void SetNewPaht(int buttonNum)
    {
        Debug.Log("[SY] 갱신 시작");
        // 경로 갱신
        buildFileUrls[buttonNum] = GameManager.instance.jsonData.temp_donwloadUrlList[buttonNum].zip_path;
        jsonFileUrls[buttonNum] = GameManager.instance.jsonData.temp_donwloadUrlList[buttonNum].json_path;
        //SaveDownloadURL(buttonNum, buildFileUrls[buttonNum]);
        Debug.Log("[SY] 갱신 완료");

        SetFilePath();
        Debug.Log("[SY] 세팅 완료");
    }

    public async UniTaskVoid DeleteOldFile(string deleteFilePath)
    {
        await UniTask.SwitchToThreadPool();
        Debug.Log("[SY] 삭제 시작");
        Directory.Delete(deleteFilePath, true);
        Debug.Log("[SY] 삭제 완료");
        await UniTask.SwitchToMainThread();

        
    }

    //---------------------------------------------//

    #region File Path Check
    public void DeleteExeFiles(int serverNum)
    {
        Debug.Log("삭제 시작");
        Test_SetDownloadURL2(serverNum);

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

        //for (int i = 0; i < 4; i++)
        //{
        //    SaveDownloadURL(i, buildFileUrls[i]);
        //    SaveDownloadFolderPath(i, exeFolderPaths[i]);
        //}
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
        for (int i = 0; i < 4; i++)
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
}

//[System.Serializable]
//public class DownloadInfoData
//{
//    public string[] downloadURL = new string[4];
//    public string[] exeFolderPaths = new string[4];
//}

[System.Serializable]
public class DownloadURL
{
    public string[] downloadURLs = new string[4];
}