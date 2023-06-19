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

using Debug = UnityEngine.Debug;
using UnityEngine.UI;
using TMPro;

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

    // Start is called before the first frame update
    void Start()
    {
        selectButton.onClick.AddListener(SetButtonState);
    }

    // Update is called once per frame
    void Update()
    {

    }

    #region File Check
    public void CheckForUpdates()
    {
        Debug.Log("[sss] : " + FilePath.Instance.GameBuildPath);

        //if (Directory.Exists(gameBuildPath))
        if (Directory.Exists(FilePath.Instance.GameBuildPath))
        {
            try
            {
                WebClient webClient = new WebClient();
                //var onlineJson = webClient.DownloadString(jsonFileUrl); // download json file
                var onlineJson = webClient.DownloadString(FilePath.Instance.JsonFileUrl); // download json file

                JObject jObject = JObject.Parse(onlineJson);
                //int resultCount = ChecksumMD5(jObject, gameBuildPath);
                Debug.Log("[sss] : " + FilePath.Instance.GameBuildPath);
                //int resultCount = Checksum.ChecksumMD5(jObject, FilePath.Instance.GameBuildPath);
                //int resultCount = Checksum.ChecksumMD5(jObject, FilePath.Instance.RootPath);
                int resultCount = Checksum.ChecksumMD5(jObject, FilePath.Instance.GameBuildPath);

                if (resultCount == 0)
                {
                    Status = LauncherStatus.ready;
                }
                else
                {

                    //InstallGameFiles(true);
                    Status = LauncherStatus.downloadingUpdate;
                }
            }
            catch (Exception ex)
            {
                Status = LauncherStatus.failed;
                Debug.LogError($"Error checking for game updates: {ex}");
            }
        }
        else
        {
            //InstallGameFiles(false);
            Status = LauncherStatus.downloadingGame;
        }
    }
    #endregion

    #region File Download
    private void InstallGameFiles(bool _isUpdate)
    {
        try
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
            //webClient.DownloadFileAsync(new Uri(buildFileUrl), gameZipPath); // download build file
            webClient.DownloadFileAsync(new Uri(FilePath.Instance.BuildFileUrl), FilePath.Instance.GameZipPath); // download build file
        }
        catch (Exception ex)
        {
            Status = LauncherStatus.failed;
            Debug.Log($"Error installing game files: {ex}");
        }
    }

    private void DownloadProgressCallback(object sender, DownloadProgressChangedEventArgs e)
    {
        //PrograssBar.Value = e.ProgressPercentage;
        Debug.Log(e.ProgressPercentage);
        prograss.SetBarState(e.ProgressPercentage);
        prograss.SetPersent(e.ProgressPercentage);
    }

    private void DownloadGameCompletedCallback(object sender, AsyncCompletedEventArgs e)
    {
        try
        {
            //if (Directory.Exists(rootPath + "\\Build"))
            if (Directory.Exists(FilePath.Instance.RootPath + "\\Build"))
            {
                //Directory.CreateDirectory(rootPath + "\\Build");
                Directory.CreateDirectory(FilePath.Instance.RootPath + "\\Build");
            }
            //ZipFile.ExtractToDirectory(gameZipPath, rootPath + "\\Build", true);
            ZipFile.ExtractToDirectory(FilePath.Instance.GameZipPath, FilePath.Instance.RootPath + "\\Build", true);
            //File.Delete(gameZipPath);
            File.Delete(FilePath.Instance.GameZipPath);

            Status = LauncherStatus.ready;

            excuteButton.interactable = true;
            prograss.gameObject.SetActive(false);
            prograss.ResetState();
        }
        catch (Exception ex)
        {
            Status = LauncherStatus.failed;
            Debug.Log($"Error finishing download: {ex}");
        }
    }
    #endregion

    #region File Execute
    private void PlayButton_Click(object sender/*, RoutedEventArgs e*/)
    {
        //if (File.Exists(gameExePath) && Status == LauncherStatus.ready)
        if (File.Exists(FilePath.Instance.GameExePath) && Status == LauncherStatus.ready)
        {
            //ProcessStartInfo startInfo = new ProcessStartInfo(gameExePath);
            ProcessStartInfo startInfo = new ProcessStartInfo(FilePath.Instance.GameExePath);
            //startInfo.WorkingDirectory = Path.Combine(rootPath, "Build");
            startInfo.WorkingDirectory = Path.Combine(FilePath.Instance.RootPath, "Build");
            Process.Start(startInfo);

            //Close(); // launcher window close
        }
        else if (Status == LauncherStatus.failed)
        {
            CheckForUpdates();
        }
    }

    public void Excute()
    {
        // excute
        //if (File.Exists(gameExePath) && Status == LauncherStatus.ready)
        if (File.Exists(FilePath.Instance.GameExePath) && Status == LauncherStatus.ready)
        {
            //ProcessStartInfo startInfo = new ProcessStartInfo(gameExePath);
            ProcessStartInfo startInfo = new ProcessStartInfo(FilePath.Instance.GameExePath);
            //startInfo.WorkingDirectory = Path.Combine(rootPath, "Build");
            startInfo.WorkingDirectory = Path.Combine(FilePath.Instance.RootPath, "Build");
            Process.Start(startInfo);

            //Close(); // launcher window close
        }
        // update
        else if (File.Exists(FilePath.Instance.GameExePath) && Status == LauncherStatus.downloadingUpdate)
        {
            InstallGameFiles(true);
        }
        // download
        else if (!File.Exists(FilePath.Instance.GameExePath) && Status == LauncherStatus.downloadingGame)
        {
            InstallGameFiles(false);
        }
        else if (Status == LauncherStatus.failed)
        {
            CheckForUpdates();
        }
    }
    #endregion

    public void SetButtonState()
    {
        if (!isSelected)
        {
            //CheckForUpdates();
            GameManager.instance.SetSelectButton(buttonNum);
        }
    }
}
