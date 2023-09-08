using System;
using System.Net;
using System.IO;
using System.IO.Compression;
using System.Diagnostics;
using System.ComponentModel;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Cysharp.Threading.Tasks;

using Debug = UnityEngine.Debug;
using JetBrains.Annotations;

public enum LauncherStatus
{
    ready,
    failed,
    downloadGame,
    downloadUpdate
}

public class FileDownload : MonoBehaviour
{
    private LauncherStatus _status;

    internal LauncherStatus Status
    {
        get => _status;
        set
        {
            _status = value;
            switch (_status)
            {
                case LauncherStatus.ready:
                    //excuteButton_txt.text = "Play";
                    excuteButton_txt.text = "Start";
                    break;
                case LauncherStatus.failed:
                    //excuteButton_txt.text = "Update Failed - Retry";
                    excuteButton_txt.text = "Update Failed";
                    break;
                case LauncherStatus.downloadGame:
                    //excuteButton_txt.text = "Download Game";
                    excuteButton_txt.text = "Download";
                    break;
                case LauncherStatus.downloadUpdate:
                    //excuteButton_txt.text = "Download Update";
                    excuteButton_txt.text = "Update";
                    break;
                default:
                    break;
            }
        }
    }

    [Header("[ Select Button Settings ]")]
    public int buttonNum = 0;
    
    public Button selectButton;
    public GameObject selectImage;
    public bool isSelected = false;

    [Space(10)]
    [Header("[ Excute Button Settings ]")]
    public Button excuteButton;
    public TMP_Text excuteButton_txt;

    [Space(10)]
    [Header("[ File Download ]")]
    public GameObject folderDialog;
    public Button installButton;
    public Prograss prograss;
    public string gameExcutePath;

    public GameObject downloadFailedPopup_1;
    public GameObject downloadFailedPopup_2;

    public bool isNeedDownload;

    public GameObject donwloadStateMessage_1;
    public GameObject donwloadStateMessage_2;

    // Start is called before the first frame update
    void Start()
    {
        selectButton.onClick.AddListener(SetButtonState);
        excuteButton.onClick.AddListener(Execute);
        installButton.onClick.AddListener(InstallFile);
        CheckBuidDirectory();

        donwloadStateMessage_1.SetActive(false);
        donwloadStateMessage_2.SetActive(false);
    }

    #region File Check
    public async UniTaskVoid CheckForUpdates()
    {
        Debug.Log("[SY] " + FilePath.Instance.ExeFolderPaths[buttonNum]);

        //excuteButton_txt.text = "-";
        excuteButton_txt.text = FilePath.Instance.fileCheckText;

        // Todo : file check
        excuteButton.interactable = false;

        if (isNeedDownload)
        {
            Status = LauncherStatus.downloadUpdate;
            excuteButton.interactable = true;
            return;
        }

        // file check
        if (Directory.Exists(FilePath.Instance.ExeFolderPaths[buttonNum]))
        {
            Debug.Log(FilePath.Instance.ExeFolderPaths[buttonNum] + " [SY] 경로에 파일 있음");

            CheckBuidDirectory();

            try
            {
                //await CheckData();
                Status = LauncherStatus.ready;

                // Todo : file check
                excuteButton.interactable = true;
            }
            catch (Exception ex)
            {
                Status = LauncherStatus.failed;
                Debug.LogError($"Error checking for game updates: {ex}");
            }
        }
        else if (FilePath.Instance.ExeFolderPaths[buttonNum] == null)
        {
            Debug.Log("[SY] 초반 세팅");
            //excuteButton_txt.text = "-";
            excuteButton_txt.text = FilePath.Instance.fileCheckText;
        }
        else
        {
            Debug.Log("[SY] 다운로드 준비");
            Status = LauncherStatus.downloadGame;

            // Todo : file check
            excuteButton.interactable = true;
        }
    }

    private async UniTask CheckData()
    {
        WebClient webClient = new WebClient();
        var onlineJson = webClient.DownloadString(FilePath.Instance.JsonFileUrls[buttonNum]); // download json file

        JObject jObject = JObject.Parse(onlineJson);

        await UniTask.SwitchToThreadPool();
        int resultCount = await Checksum.ChecksumMD5(jObject, FilePath.Instance.ExeFolderPaths[buttonNum]);
        await UniTask.SwitchToMainThread();

        if (resultCount == 0)
        {
            Status = LauncherStatus.ready;
        }
        else
        {
            Status = LauncherStatus.downloadUpdate;
        }

        // Todo : file check
        excuteButton.interactable = true;
    }
    #endregion

    #region File Download
    private async UniTaskVoid InstallGameFiles(bool _isUpdate)
    {
        try
        {
            await DownloadFile(_isUpdate);
        }
        catch (Exception ex)
        {
            Status = LauncherStatus.failed;
            Debug.Log($"Error installing game files: {ex}");
        }
    }

    // Todo : 파일 다운로드
    private async UniTask DownloadFile(bool _isUpdate)
    {
        DEV.instance.isFileDownload = true;

        if (DEV.instance.isProtectFileDownload)
        {
            // enble protect guard
            DEV.instance.downloadProtectGaurd.SetActive(DEV.instance.isFileDownload);
        }

        excuteButton.interactable = false;
        prograss.gameObject.SetActive(true);

        WebClient webClient = new WebClient();

        if (_isUpdate)
        {
            Status = LauncherStatus.downloadUpdate;
        }
        else
        {
            Status = LauncherStatus.downloadGame;
        }
        Debug.Log("[SY] 다운로드 시작");

        donwloadStateMessage_1.SetActive(true);

        webClient.DownloadProgressChanged += new DownloadProgressChangedEventHandler(DownloadProgressCallback);
        webClient.DownloadFileCompleted += new AsyncCompletedEventHandler(DownloadGameCompletedCallback);
        webClient.DownloadFileAsync(new Uri(FilePath.Instance.BuildFileUrls[buttonNum]), FilePath.Instance.ExeZipFilePaths[buttonNum]); // download build file
    }

    private void DownloadProgressCallback(object sender, DownloadProgressChangedEventArgs e)
    {
        prograss.SetBarState(e.ProgressPercentage);
        prograss.SetPersent(e.ProgressPercentage);
    }

    private void DownloadGameCompletedCallback(object sender, AsyncCompletedEventArgs e)
    {
        try
        {
            if (Directory.Exists(FilePath.Instance.RootPaths[buttonNum] + "\\" + FilePath.Instance.ExeFolderNames[buttonNum]))
            {
                Directory.CreateDirectory(FilePath.Instance.RootPaths[buttonNum] + "\\" + FilePath.Instance.ExeFolderNames[buttonNum]);
                Debug.Log(FilePath.Instance.RootPaths[buttonNum] + "\\" + FilePath.Instance.ExeFolderNames[buttonNum]);
            }

            //ZipFile.ExtractToDirectory(FilePath.Instance.ExeZipFilePaths[buttonNum], FilePath.Instance.RootPaths[buttonNum] + "\\" + FilePath.Instance.ExeFolderNames[buttonNum], true);
            //File.Delete(FilePath.Instance.ExeZipFilePaths[buttonNum]);

            //if (buttonNum == 2)
            //{
            //    GameManager.instance.ugcManager.CreateBatchFile();
            //}

            //Status = LauncherStatus.ready;

            //excuteButton.interactable = true;
            //prograss.gameObject.SetActive(false);
            //prograss.ResetState();

            //CheckBuidDirectory();

            UnzipFile().Forget();
        }
        catch (Exception ex)
        {
            Status = LauncherStatus.failed;
            Debug.Log($"Error finishing download: {ex}");

            donwloadStateMessage_1.SetActive(false);
            donwloadStateMessage_2.SetActive(false);

            if (DEV.instance.isUsingFolderDialog)
            {
                //Debug.Log("해당 경로에 다운로드할 수 없습니다. 다른 경로에서 설치를 진행해 주세요.");
                downloadFailedPopup_1.SetActive(true);
            }
            else 
            {
                //Debug.Log("해당 경로에 다운로드할 수 없습니다. 다른 경로에서 설치를 진행해 주세요.");
                downloadFailedPopup_2.SetActive(true);
            }
        }

        DEV.instance.isFileDownload = false;

        if (DEV.instance.isProtectFileDownload)
        {
            // disable protect guard
            DEV.instance.downloadProtectGaurd.SetActive(DEV.instance.isFileDownload);
        }
    }

    private async UniTaskVoid UnzipFile()
    {
        donwloadStateMessage_1.SetActive(false);
        donwloadStateMessage_2.SetActive(true);

        await UniTask.SwitchToThreadPool();
        ZipFile.ExtractToDirectory(FilePath.Instance.ExeZipFilePaths[buttonNum], FilePath.Instance.RootPaths[buttonNum] + "\\" + FilePath.Instance.ExeFolderNames[buttonNum], true);
        File.Delete(FilePath.Instance.ExeZipFilePaths[buttonNum]);

        if (buttonNum == 2)
        {
            GameManager.instance.ugcManager.CreateBatchFile();
        }

        await UniTask.SwitchToMainThread();

        Status = LauncherStatus.ready;
        excuteButton.interactable = true;
        prograss.gameObject.SetActive(false);
        prograss.ResetState();

        donwloadStateMessage_2.SetActive(false);

        CheckBuidDirectory();
    }

    private void CheckBuidDirectory()
    {
        if (buttonNum == 2 && Directory.Exists(FilePath.Instance.ExeFolderPaths[buttonNum]))
        {
            Debug.Log("[ugc] 프로젝트 파일 존재");
            string batchFilePath = GameManager.instance.ugcManager.LoadUGCFilePath().batchFilePath;
            Debug.Log("[ugc] batchFilePath : " + batchFilePath);
            if (File.Exists(batchFilePath))
            {
                Debug.Log("[ugc] 파일 존재");
                // excute file name
                gameExcutePath = batchFilePath;
            }
            else
            {
                Debug.Log("[ugc] 파일 없음");
            }
            //else
            //{
            //    Debug.Log("배치 파일 없음 배치 파일 재생성");
            //    GameManager.instance.ugcManager.CreateBatchFile();
            //    gameExcutePath = GameManager.instance.ugcManager.LoadUGCFilePath().batchFilePath;
            //}
        }
        else if (buttonNum != 2 !& Directory.Exists(FilePath.Instance.ExeFolderPaths[buttonNum]))
        {
            string filePath = FilePath.Instance.ExeFolderPaths[buttonNum];
            string[] searchFile = Directory.GetFiles(filePath, "*.exe");

            for (int i = 0; i < searchFile.Length; i++)
            {
                string[] fileName = searchFile[i].Split('\\');

                if (fileName[fileName.Length - 1] != "UnityCrashHandler64.exe")
                {
                    // excute file name
                    gameExcutePath = Path.Combine(FilePath.Instance.RootPaths[buttonNum], FilePath.Instance.ExeFolderPaths[buttonNum], fileName[fileName.Length - 1]);
                }
            }
        }
    }
    #endregion

    #region File Execute
    public void Execute()
    {
        Debug.Log($"[SY] : {gameExcutePath}");
        Debug.Log($"[SY] Execute result : {File.Exists(gameExcutePath)} / {Status}");
        //Debug.Log($"[SY] : {FilePath.Instance.defaultDataPath}");

        //if (!isNeedDownload)
        {
            // create folder
            if (Directory.Exists(FilePath.Instance.defaultDataPath))
            {
                Debug.Log("exist directory");
            }
            else
            {
                Debug.Log("not exist directory and create directory");
                Directory.CreateDirectory(FilePath.Instance.defaultDataPath);
            }

            // execute 1
            if (File.Exists(gameExcutePath) && Status == LauncherStatus.ready)
            {
                if (buttonNum == 2)
                {
                    if (GameManager.instance.ugcManager.LoadUGCFilePath().batchFilePath == "")
                    {
                        Debug.Log("[ugc] batch file path 없음");
                        if (!GameManager.instance.ugcManager.UnityProjectExeFileCheck())
                        {
                            Debug.Log("[ugc] 유니티 경로 문제 발생!!!");
                            return;
                        }
                        else
                        {
                            Debug.Log("[ugc] 유니티 경로 문제 없음");
                        }
                            

                        GameManager.instance.ugcManager.CreateBatchFile();
                    }

                    ProcessStartInfo startInfo = new ProcessStartInfo(GameManager.instance.ugcManager.LoadUGCFilePath().batchFilePath);
                    startInfo.WorkingDirectory = GameManager.instance.ugcManager.LoadUGCFilePath().objectUGCProjectDownloadPath;
                    //Process.Start(startInfo);

                    // Todo : process Test
                    //DEV.instance.process = Process.Start(startInfo);

                    Process[] _runningFiles = GameManager.instance.runningFiles;

                    if (_runningFiles[buttonNum] != null && !_runningFiles[buttonNum].HasExited)
                    {
                        Debug.Log("[SY] 파일 강제 종료");
                        _runningFiles[buttonNum].Kill();
                    }

                    _runningFiles[buttonNum] = Process.Start(startInfo);
                }
                else
                {
                    ProcessStartInfo startInfo = new ProcessStartInfo(gameExcutePath);
                    startInfo.WorkingDirectory = Path.Combine(FilePath.Instance.RootPaths[buttonNum], FilePath.Instance.ExeFolderNames[buttonNum]);

                    Process[] _runningFiles = GameManager.instance.runningFiles;

                    if (_runningFiles[buttonNum] != null && !_runningFiles[buttonNum].HasExited)
                    {
                        Debug.Log("[SY] 파일 강제 종료");
                        _runningFiles[buttonNum].Kill();
                    }

                    _runningFiles[buttonNum] = Process.Start(startInfo);
                }
            }
            // exwcuate 2
            else if (!File.Exists(gameExcutePath) && Status == LauncherStatus.ready)
            {
                if (buttonNum == 2)
                {
                    //if (gameExcutePath == "" || gameExcutePath != "")
                    {
                        Debug.Log("[ugc] batch file path 없음");
                        if (!GameManager.instance.ugcManager.UnityProjectExeFileCheck())
                        {
                            Debug.Log("[ugc] 유니티 경로 문제 발생!!!");
                            return;
                        }
                        else
                        {
                            Debug.Log("[ugc] 유니티 경로 문제 없음");
                        }


                        GameManager.instance.ugcManager.CreateBatchFile();
                    }

                    gameExcutePath = GameManager.instance.ugcManager.LoadUGCFilePath().batchFilePath;

                    //ProcessStartInfo startInfo = new ProcessStartInfo(GameManager.instance.ugcManager.LoadUGCFilePath().batchFilePath);
                    ProcessStartInfo startInfo = new ProcessStartInfo(gameExcutePath);
                    startInfo.WorkingDirectory = GameManager.instance.ugcManager.LoadUGCFilePath().objectUGCProjectDownloadPath;

                    Process[] _runningFiles = GameManager.instance.runningFiles;

                    if (_runningFiles[buttonNum] != null && !_runningFiles[buttonNum].HasExited)
                    {
                        Debug.Log("[SY] 파일 강제 종료");
                        _runningFiles[buttonNum].Kill();
                    }

                    _runningFiles[buttonNum] = Process.Start(startInfo);
                }
            }
            // update
            else if (File.Exists(gameExcutePath) && Status == LauncherStatus.downloadUpdate)
            {
                UniTask.SwitchToThreadPool();
                InstallGameFiles(true).Forget();
                UniTask.SwitchToMainThread();
            }
            // download
            else if (!File.Exists(gameExcutePath) && Status == LauncherStatus.downloadGame)
            {
                if (buttonNum == 2 && !GameManager.instance.ugcManager.UnityProjectExeFileCheck())
                    return;

                if (DEV.instance.isUsingFolderDialog)
                {
                    folderDialog.SetActive(true);
                }
                else
                {
                    InstallFile();
                }
            }
            else if (Status == LauncherStatus.failed)
            {
                UniTask.SwitchToThreadPool();
                //CheckForUpdates().Forget();
                UniTask.SwitchToMainThread();
            }
        }
        //else
        //{
        //    isNeedDownload = false;
            
        //    // Todo : ugc 용 구분	
        //    string deleteFilePaht = FilePath.Instance.ChangeDeleteFileName(buttonNum);
            
        //    FilePath.Instance.SetNewPaht(buttonNum);
        //    FilePath.Instance.DeleteOldFile(deleteFilePaht).Forget();
        //    FilePath.Instance.SaveDownloadURL(buttonNum, GameManager.instance.jsonData.temp_donwloadUrlList[buttonNum].zip_path);
            
        //    UniTask.SwitchToThreadPool();
        //    InstallGameFiles(true).Forget();
        //    UniTask.SwitchToMainThread();
        //}
    }

    public void InstallFile()
    {
        folderDialog.SetActive(false);
        UniTask.SwitchToThreadPool();
        InstallGameFiles(false).Forget();
        UniTask.SwitchToMainThread();
    }
    #endregion

    public void SetButtonState()
    {
        if (!isSelected)
        {
            Debug.Log("Set Button State");
            GameManager.instance.SetSelectButton(buttonNum);
        }
    }

    #region File Download
    public void BTN_ConfirmDownloadFailed()
    {
        if (DEV.instance.isUsingFolderDialog)
        {
            downloadFailedPopup_1.SetActive(false);
            folderDialog.SetActive(true);
        }
        else
        {
            downloadFailedPopup_2.SetActive(false);

            prograss.gameObject.SetActive(false);

            excuteButton.interactable = true;
            Status = LauncherStatus.downloadGame;
        }
    }
    #endregion
}
