using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.IO;
using System.Diagnostics;
using Debug = UnityEngine.Debug;
using TMPro;

public class LauncherDownload : MonoBehaviour
{
    public string exeFileName;
    public string path;

    public bool isCampletedDownloand;
    public bool isDownloading;

    public GameObject preparingUpdatePopup;
    public TMP_Text preparePersent;

    // Start is called before the first frame update
    void Start()
    {
        path = Path.GetDirectoryName(Environment.GetFolderPath(Environment.SpecialFolder.Personal));
        path = Path.Combine(path, "Downloads");
        //Debug.LogError(path + "\\" + exeFileName);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            //preparingUpdatePopup.SetActive(true);
            //FileDownload().Forget();
            GameManager.instance.popupManager.ShowLauncherUpdatePopup();
        }

        if (isCampletedDownloand)
        {
            isCampletedDownloand = false;
            preparingUpdatePopup.SetActive(false);
            Application.Quit();
        }
    }

    public async UniTaskVoid FileDownload()
    {
        //await UniTask.SwitchToMainThread();
        await UniTask.Yield();
        preparingUpdatePopup.SetActive(true);

        WebClient webClient = new WebClient();

        path = Path.GetDirectoryName(Environment.GetFolderPath(Environment.SpecialFolder.Personal));
        path = Path.Combine(path, "Downloads");

        webClient.DownloadProgressChanged += new DownloadProgressChangedEventHandler(DownloadProgressCallBack);
        webClient.DownloadFileCompleted += new AsyncCompletedEventHandler(DownloadGameCompletedCallback);
        //webClient.DownloadFileAsync(new Uri("http://101.101.218.135:5002/onlineScienceMuseumAPI/downloadInstallFile.do"), path + "\\" + exeFileName); // download build file
        webClient.DownloadFileAsync(new Uri(GameManager.instance.api.launcherDownloadURL), path + "\\" + exeFileName); // download build file
    }

    private void DownloadProgressCallBack(object sender, DownloadProgressChangedEventArgs e)
    {
        isDownloading = true;
        preparePersent.text = e.ProgressPercentage.ToString() + "%";
        //Debug.LogError($"[Launcher Donwload] Downloading : {preparePersent.text}%");
    }
    private async void DownloadGameCompletedCallback(object sender, AsyncCompletedEventArgs e)
    {
        if (isDownloading)
        {
            try
            {
                ProcessStartInfo startInfo = new ProcessStartInfo(path + "\\" + exeFileName);
                startInfo.WorkingDirectory = GameManager.instance.ugcManager.LoadUGCFilePath().objectUGCProjectDownloadPath;

                Process.Start(startInfo);

                //Debug.LogError("[Launcher Download] Download Complete");
                Debug.Log("[Launcher Download] Download Complete");

                isCampletedDownloand = true;
            }
            catch (Exception ex)
            {
                Debug.LogError($"[Launcher Donwload] Execute Error : {ex}");
                isCampletedDownloand = true;
            }
        }
        else
        {
            //Debug.LogError("[Launcher Donwload] Redownload");
            Debug.Log("[Launcher Donwload] Redownload");
            await UniTask.Delay(3000);
            FileDownload().Forget();
        }
    }
}
