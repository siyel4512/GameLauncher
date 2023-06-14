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

public enum LauncherStatus
{
    ready,
    failed,
    downloadingGame,
    downloadingUpdate
}

public class FileIO : MonoBehaviour
{
    // TCP
    private TCP_Server tcp_Server = new TCP_Server();


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
                    SetPage(false, "Play");

                    // start TCP server
                    tcp_Server.StartServer();
                    break;
                case LauncherStatus.failed:
                    SetPage(false, "Update Failed - Retry");
                    break;
                case LauncherStatus.downloadingGame:
                    SetPage(true, "Downloading Game");
                    break;
                case LauncherStatus.downloadingUpdate:
                    SetPage(true, "Downloading Update");
                    break;
                default:
                    break;
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    #region Change file load Page
    // Set launcher UI layout
    private void SetPage(bool _isLoading, string _stateText)
    {
        //if (_isLoading)
        //{
        //    StateText.Visibility = Visibility.Visible;
        //    StateText.Text = _stateText;
        //    PrograssBar.Visibility = Visibility.Visible;

        //    PlayButton.Visibility = Visibility.Hidden;
        //}
        //else
        //{
        //    StateText.Visibility = Visibility.Hidden;
        //    PrograssBar.Visibility = Visibility.Hidden;

        //    PlayButton.Visibility = Visibility.Visible;
        //    PlayButton.Content = _stateText;
        //}
    }
    #endregion

    #region File Check
    private void CheckForUpdates()
    {
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
                int resultCount = Checksum.ChecksumMD5(jObject, FilePath.Instance.GameBuildPath);

                if (resultCount == 0)
                {
                    Status = LauncherStatus.ready;
                }
                else
                {
                    InstallGameFiles(true);
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
            InstallGameFiles(false);
        }
    }
    #endregion

    #region File Download
    private void InstallGameFiles(bool _isUpdate)
    {
        try
        {
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
    #endregion

    //#region Checksum
    //public static int ChecksumMD5(JObject json, string rootPath)
    //{
    //    var result = new List<string>();
    //    foreach (JProperty prop in json.Properties())
    //    {
    //        byte[] btFile = File.ReadAllBytes(rootPath + prop.Name);
    //        byte[] btHash = MD5.Create().ComputeHash(btFile);

    //        if (Convert.ToBase64String(btHash) != prop.Value.ToString())
    //        {
    //            Debug.Log($"{Convert.ToBase64String(btHash)}, {prop.Value.ToString()}");
    //            result.Add(prop.Name);
    //        }
    //    }
    //    Debug.Log(result.Count);
    //    return result.Count;
    //}
    //#endregion

    //private void Window_Closed(object sender, EventArgs e)
    //{
    //    if (tcp_Server.Server != null) tcp_Server.Server.Stop();
    //    if (tcp_Server.Client != null) tcp_Server.Client.Close();
    //}
}
