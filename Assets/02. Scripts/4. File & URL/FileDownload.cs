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
                    excuteButton_txt.text = "Play";
                    break;
                case LauncherStatus.failed:
                    excuteButton_txt.text = "Update Failed - Retry";
                    break;
                case LauncherStatus.downloadGame:
                    excuteButton_txt.text = "Download Game";
                    break;
                case LauncherStatus.downloadUpdate:
                    excuteButton_txt.text = "Download Update";
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
    private string gameExcutePath;

    // Start is called before the first frame update
    void Start()
    {
        selectButton.onClick.AddListener(SetButtonState);
        excuteButton.onClick.AddListener(Execute);
        installButton.onClick.AddListener(InstallFile);
        CheckBuidDirectory();
    }

    // Update is called once per frame
    void Update()
    {

    }

    #region File Check
    public async UniTaskVoid CheckForUpdates()
    {
        // file check
        if (Directory.Exists(FilePath.Instance.ExeFolderPaths[buttonNum]))
        {
            try
            {
                await CheckData();
            }
            catch (Exception ex)
            {
                Status = LauncherStatus.failed;
                Debug.LogError($"Error checking for game updates: {ex}");
            }
        }
        else
        {
            Status = LauncherStatus.downloadGame;
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

    private async UniTask DownloadFile(bool _isUpdate)
    {
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
            //if (Directory.Exists(FilePath.Instance.RootPath + "\\" + FilePath.Instance.ExeFolderNames[buttonNum]))
            //if (Directory.Exists(FilePath.Instance.RootPaths[buttonNum] + "\\" + FilePath.Instance.ExeFolderNames[buttonNum]))
            if (Directory.Exists(FilePath.Instance.RootPaths[buttonNum] + "\\" + FilePath.Instance.ExeFolderNames[buttonNum]))
            {
                //Directory.CreateDirectory(FilePath.Instance.RootPath + "\\" + FilePath.Instance.ExeFolderNames[buttonNum]);
                Directory.CreateDirectory(FilePath.Instance.RootPaths[buttonNum] + "\\" + FilePath.Instance.ExeFolderNames[buttonNum]);
                //Debug.Log(FilePath.Instance.RootPath + "\\" + FilePath.Instance.ExeFolderNames[buttonNum]);
                Debug.Log(FilePath.Instance.RootPaths[buttonNum] + "\\" + FilePath.Instance.ExeFolderNames[buttonNum]);
            }

            //ZipFile.ExtractToDirectory(FilePath.Instance.ExeZipFilePaths[buttonNum], FilePath.Instance.RootPath + "\\" + FilePath.Instance.ExeFolderNames[buttonNum], true);
            ZipFile.ExtractToDirectory(FilePath.Instance.ExeZipFilePaths[buttonNum], FilePath.Instance.RootPaths[buttonNum] + "\\" + FilePath.Instance.ExeFolderNames[buttonNum], true);
            File.Delete(FilePath.Instance.ExeZipFilePaths[buttonNum]);

            Status = LauncherStatus.ready;

            excuteButton.interactable = true;
            prograss.gameObject.SetActive(false);
            prograss.ResetState();

            //-------------------------------------------------------------------------------------//

            CheckBuidDirectory();
        }
        catch (Exception ex)
        {
            Status = LauncherStatus.failed;
            Debug.Log($"Error finishing download: {ex}");
        }
    }

    private void CheckBuidDirectory()
    {
        if (Directory.Exists(FilePath.Instance.ExeFolderPaths[buttonNum]))
        {
            string filePath = FilePath.Instance.ExeFolderPaths[buttonNum];
            string[] searchFile = Directory.GetFiles(filePath, "*.exe");

            for (int i = 0; i < searchFile.Length; i++)
            {
                string[] fileName = searchFile[i].Split('\\');

                if (fileName[fileName.Length - 1] != "UnityCrashHandler64.exe")
                {
                    // excute file name
                    //gameExcutePath = Path.Combine(FilePath.Instance.RootPath, FilePath.Instance.ExeFolderPaths[buttonNum], fileName[fileName.Length - 1]);
                    gameExcutePath = Path.Combine(FilePath.Instance.RootPaths[buttonNum], FilePath.Instance.ExeFolderPaths[buttonNum], fileName[fileName.Length - 1]);
                }
            }
        }
    }
    #endregion

    #region File Execute
    public void Execute()
    {
        // execute
        if (File.Exists(gameExcutePath) && Status == LauncherStatus.ready)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo(gameExcutePath);
            //startInfo.WorkingDirectory = Path.Combine(FilePath.Instance.RootPath, FilePath.Instance.ExeFolderNames[buttonNum]);
            startInfo.WorkingDirectory = Path.Combine(FilePath.Instance.RootPaths[buttonNum], FilePath.Instance.ExeFolderNames[buttonNum]);
            Process.Start(startInfo);
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
            folderDialog.SetActive(true);
        }
        else if (Status == LauncherStatus.failed)
        {
            UniTask.SwitchToThreadPool();
            CheckForUpdates().Forget();
            UniTask.SwitchToMainThread();
        }
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
            GameManager.instance.SetSelectButton(buttonNum);
        }
    }
}
