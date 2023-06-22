using System;
using System.Collections.Generic;
using System.Net;
using System.IO;
using System.IO.Compression;
using System.Diagnostics;
using System.ComponentModel;
using System.Security.Cryptography;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Cysharp.Threading.Tasks;

using Debug = UnityEngine.Debug;
using static UnityEngine.PlayerLoop.PreUpdate;

public enum LauncherStatus
{
    ready,
    failed,
    downloadingGame,
    downloadingUpdate
}

public class FileIO : MonoBehaviour
{
    public int buttonNum = 0;
    // TCP
    private TCP_Server tcp_Server = new TCP_Server();

    public LauncherStatus _status;
    internal LauncherStatus Status
    {
        get => _status;
        set
        {
            _status = value;
            switch (_status)
            {
                case LauncherStatus.ready:
                    
                    // start TCP server
                    tcp_Server.StartServer();

                    excuteButton_txt.text = "Play";
                    break;
                case LauncherStatus.failed:
                    excuteButton_txt.text = "Update Failed - Retry";
                    break;
                case LauncherStatus.downloadingGame:
                    excuteButton_txt.text = "Downloading Game";
                    break;
                case LauncherStatus.downloadingUpdate:
                    excuteButton_txt.text = "Downloading Update";
                    break;
                default:
                    break;
            }
        }
    }

    public Button selectButton;
    public Button excuteButton;
    public TMP_Text excuteButton_txt;
    public GameObject selectImage;
    public bool isSelected = false;

    public Prograss prograss;
    private string gameExcutePath;

    // Start is called before the first frame update
    void Start()
    {
        selectButton.onClick.AddListener(SetButtonState);
        CheckBuidDirectory();
    }

    // Update is called once per frame
    void Update()
    {

    }

    #region File Check
    public async UniTaskVoid CheckForUpdates()
    {
        if (Directory.Exists(FilePath.Instance.GameBuildPaths[buttonNum]))
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
            Status = LauncherStatus.downloadingGame;
        }
    }

    private async UniTask CheckData()
    {
        WebClient webClient = new WebClient();
        var onlineJson = webClient.DownloadString(FilePath.Instance.JsonFileUrls[buttonNum]); // download json file

        JObject jObject = JObject.Parse(onlineJson);

        await UniTask.SwitchToThreadPool();
        int resultCount = await Checksum.ChecksumMD5(jObject, FilePath.Instance.GameBuildPaths[buttonNum]);
        await UniTask.SwitchToMainThread();

        if (resultCount == 0)
        {
            Status = LauncherStatus.ready;
        }
        else
        {
            Status = LauncherStatus.downloadingUpdate;
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
            Status = LauncherStatus.downloadingUpdate;
        }
        else
        {
            Status = LauncherStatus.downloadingGame;
        }

        webClient.DownloadProgressChanged += new DownloadProgressChangedEventHandler(DownloadProgressCallback);
        webClient.DownloadFileCompleted += new AsyncCompletedEventHandler(DownloadGameCompletedCallback);
        webClient.DownloadFileAsync(new Uri(FilePath.Instance.BuildFileUrls[buttonNum]), FilePath.Instance.GameZipPaths[buttonNum]); // download build file
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
            if (Directory.Exists(FilePath.Instance.RootPath + "\\" + FilePath.Instance.BuildFileNames[buttonNum]))
            {
                Directory.CreateDirectory(FilePath.Instance.RootPath + "\\" + FilePath.Instance.BuildFileNames[buttonNum]);
                Debug.Log(FilePath.Instance.RootPath + "\\" + FilePath.Instance.BuildFileNames[buttonNum]);
            }

            ZipFile.ExtractToDirectory(FilePath.Instance.GameZipPaths[buttonNum], FilePath.Instance.RootPath + "\\" + FilePath.Instance.BuildFileNames[buttonNum], true);
            File.Delete(FilePath.Instance.GameZipPaths[buttonNum]);

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
        if (Directory.Exists(FilePath.Instance.GameBuildPaths[buttonNum]))
        {
            string filePath = FilePath.Instance.GameBuildPaths[buttonNum];
            string[] searchFile = Directory.GetFiles(filePath, "*.exe");

            for (int i = 0; i < searchFile.Length; i++)
            {
                string[] fileName = searchFile[i].Split('\\');

                if (fileName[fileName.Length - 1] != "UnityCrashHandler64.exe")
                {
                    // excute file name
                    gameExcutePath = Path.Combine(FilePath.Instance.RootPath, FilePath.Instance.GameBuildPaths[buttonNum], fileName[fileName.Length - 1]);
                }
            }
        }
    }
    #endregion

    #region File Execute
    public void Excute()
    {
        // excute
        if (File.Exists(gameExcutePath) && Status == LauncherStatus.ready)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo(gameExcutePath);
            startInfo.WorkingDirectory = Path.Combine(FilePath.Instance.RootPath, FilePath.Instance.BuildFileNames[buttonNum]);
            Process.Start(startInfo);

            //Close(); // launcher window close
        }
        // update
        else if (File.Exists(gameExcutePath) && Status == LauncherStatus.downloadingUpdate)
        {
            UniTask.SwitchToThreadPool();
            InstallGameFiles(true).Forget();
            UniTask.SwitchToMainThread();
        }
        // download
        else if (!File.Exists(gameExcutePath) && Status == LauncherStatus.downloadingGame)
        {
            UniTask.SwitchToThreadPool();
            InstallGameFiles(false).Forget();
            UniTask.SwitchToMainThread();
        }
        else if (Status == LauncherStatus.failed)
        {
            UniTask.SwitchToThreadPool();
            CheckForUpdates().Forget();
            UniTask.SwitchToMainThread();
        }
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
