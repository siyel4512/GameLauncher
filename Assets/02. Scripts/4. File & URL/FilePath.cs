using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using UnityEditor;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Windows.Forms;

using Application = UnityEngine.Application;
using Debug = UnityEngine.Debug;
using Ookii.Dialogs;
using System.Diagnostics;

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
        //FilePathCheck(0);

        InitDataPath();

        //SetDownloadURL();
        //Test_SetDownloadURL();

        //FilePathCheck();
    }

    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.Space))
        //{
        //    Debug.Log("삭제 시작");
        //    Directory.Delete("C:\\Users\\LSY\\Desktop\\TEST", true);
        //    Debug.Log("삭제 완료");
        //}

        //if (Input.GetKeyDown(KeyCode.Space)) 
        //{
        //    Test_SetDownloadURL();
        //    FilePathCheck();
        //}
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

        //for (int i = 0; i < 4; i++)
        //{
        //    SaveDownloadURL(i, buildFileUrls[i]);
        //    SaveDownloadFolderPath(i, exeFolderPaths[i]); // Todo : maybe delete
        //}
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

        //switch (GameManager.instance.selectedServerNum)
        switch (serverNum)
        {
            case 0:
                // dev server
                Debug.Log("[SY] dev server");
                await GameManager.instance.api.Request_FileDownloadURL(ServerType.dev, FileType.pc);
                await GameManager.instance.api.Request_FileDownloadURL(ServerType.dev, FileType.vr);
                await GameManager.instance.api.Request_FileDownloadURL(ServerType.dev, FileType.prod);
                await GameManager.instance.api.Request_FileDownloadURL(ServerType.dev, FileType.colca);
                break;
            case 1:
                // test server
                Debug.Log("[SY] test server");
                await GameManager.instance.api.Request_FileDownloadURL(ServerType.test, FileType.pc);
                await GameManager.instance.api.Request_FileDownloadURL(ServerType.test, FileType.vr);
                await GameManager.instance.api.Request_FileDownloadURL(ServerType.test, FileType.prod);
                await GameManager.instance.api.Request_FileDownloadURL(ServerType.test, FileType.colca);
                break;
            case 2:
                // staging server
                Debug.Log("[SY] staging server");
                await GameManager.instance.api.Request_FileDownloadURL(ServerType.stage, FileType.pc);
                await GameManager.instance.api.Request_FileDownloadURL(ServerType.stage, FileType.vr);
                await GameManager.instance.api.Request_FileDownloadURL(ServerType.stage, FileType.prod);
                await GameManager.instance.api.Request_FileDownloadURL(ServerType.stage, FileType.colca);
                break;
            case 3:
                // live server
                Debug.Log("[SY] live server");
                await GameManager.instance.api.Request_FileDownloadURL_live(FileType.pc);
                await GameManager.instance.api.Request_FileDownloadURL_live(FileType.vr);
                await GameManager.instance.api.Request_FileDownloadURL_live(FileType.prod);
                await GameManager.instance.api.Request_FileDownloadURL_live(FileType.colca);
                break;
        }

        CompareToFileDownloadURL();

        SetFilePath();
    }

    public void CompareToFileDownloadURL()
    {
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
                    try
                    {
                        Debug.Log("[SY] 파일 존재 " + exeFolderPaths[i]);
                        Debug.Log("이름 변경 시작");
                        //Directory.Delete(exeFolderPaths[i], true);
                        Directory.Move(exeFolderPaths[i], exeFolderPaths[i] + "_temp");
                        Debug.Log("이름 변경 완료");
                        DeleteOldFile(exeFolderPaths[i] + "_temp").Forget();
                        //exeFolderPaths[i] = "";
                    }
                    catch (Exception e)
                    {
                        Debug.LogError(e);
                    }
                    
                }

                // 경로 갱신
                buildFileUrls[i] = GameManager.instance.jsonData.temp_donwloadUrlList[i].zip_path;
                jsonFileUrls[i] = GameManager.instance.jsonData.temp_donwloadUrlList[i].json_path;

                SaveDownloadURL(i, buildFileUrls[i]);
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

    private async UniTaskVoid DeleteOldFile(string deleteFilePath)
    {
        await UniTask.SwitchToThreadPool();
        Debug.Log("삭제 시작");
        Directory.Delete(deleteFilePath, true);
        Debug.Log("삭제 완료");
        await UniTask.SwitchToMainThread();
    }

    //---------------------------------------------//
    //---------------------------------------------//
    //---------------------------------------------//
    //---------------------------------------------//
    //---------------------------------------------//

    #region File Path Check
    //public void FilePathCheck()
    //{
    //    Test_SetDownloadURL();
    //    //Test_SetDownloadURL2();

    //    for (int i = 0; i < 4; i++)
    //    {
    //        // Check URL Value
    //        if (buildFileUrls[i] != LoadDownloadInfoData().downloadURL[i])
    //        {
    //            Debug.Log(i + " 값 다름 바로 삭제 필요");

    //            Debug.Log("경로 : " + LoadDownloadInfoData().exeFolderPaths[i]);

    //            //if (LoadDownloadInfoData().exeFolderPaths[i] != "")
    //            {
    //                if (Directory.Exists(LoadDownloadInfoData().exeFolderPaths[i]))
    //                {
    //                    Debug.Log("파일 삭제");
    //                    Directory.Delete(LoadDownloadInfoData().exeFolderPaths[i], true);
    //                }
    //            }

    //            // 해당 폴더 밑에 존재하는 모든 파일 삭제 기능필요
    //            //Debug.Log("Root Paths : " + RootPaths[i]);
    //        }
    //        //else
    //        //{
    //        //    Debug.Log(i + "값 같음 체크썸 진행");
    //        //}

    //        SaveDownloadURL(i, buildFileUrls[i]);
    //        SaveDownloadFolderPath(i, exeFolderPaths[i]);
    //    }
    //}

    public void DeleteExeFiles(int serverNum)
    {
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

    //#region Exe file path Save & Load
    ////save exe file download url
    //public void SaveDownloadURL(int _index, string _path)
    //{
    //    exeFilePath.downloadURL[_index] = _path;

    //    string jsonData = JsonUtility.ToJson(exeFilePath, true);
    //    string serverNum = Path.Combine(Application.streamingAssetsPath + "/Data Path", "exeFilePath.json");
    //    File.WriteAllText(serverNum, jsonData);
    //}

    //// save exe file download folder path
    //public void SaveDownloadFolderPath(int _buttonNum, string _path)
    //{
    //    exeFilePath.exeFolderPaths[_buttonNum] = _path;

    //    string jsonData = JsonUtility.ToJson(exeFilePath, true);
    //    string serverNum = Path.Combine(Application.streamingAssetsPath + "/Data Path", "exeFilePath.json");
    //    File.WriteAllText(serverNum, jsonData);
    //}

    //// load download info data
    //public DownloadInfoData LoadDownloadInfoData()
    //{
    //    string serverNum = Path.Combine(Application.streamingAssetsPath + "/Data Path", "exeFilePath.json");
    //    string jsonData = File.ReadAllText(serverNum);
    //    exeFilePath = JsonUtility.FromJson<DownloadInfoData>(jsonData);

    //    return exeFilePath;
    //}

    //public void ResetDownloadInfoData()
    //{
    //    //defaultDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");
    //    //defaultDataPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
    //    //defaultDataPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
    //    //defaultDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
    //    //defaultDataPath = "C:\\Curiverse";
    //    defaultDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "TEST Folder");

    //    for (int i = 0; i < 4; i++)
    //    {
    //        exeFilePath.downloadURL[i] = "";
    //        exeFilePath.exeFolderPaths[i] = "";
    //    }

    //    string jsonData = JsonUtility.ToJson(exeFilePath, true);
    //    string path = Path.Combine(Application.streamingAssetsPath + "/Data Path", "exeFilePath.json");
    //    File.WriteAllText(path, jsonData);
    //}
    //#endregion

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