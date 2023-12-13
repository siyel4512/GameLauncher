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

public class TEST_LauncherDownload : MonoBehaviour
{
    public string path;

    public bool isCampletedDownloand;

    public bool isDownloading;

    // Start is called before the first frame update
    void Start()
    {
        path = Path.GetDirectoryName(Environment.GetFolderPath(Environment.SpecialFolder.Personal));
        path = Path.Combine(path, "Downloads");
        Debug.LogError(path + "\\Launcher_download_Test.exe"); // 저장할 .exe 파일 변경하기
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.LogError("런처 다운로드 테스트 시작");
            Test_FileDownload().Forget();
        }

        if (isCampletedDownloand)
        {
            isCampletedDownloand = false;
            Application.Quit();
        }
    }

    private async UniTaskVoid Test_FileDownload()
    {
        await UniTask.SwitchToThreadPool();
        WebClient webClient = new WebClient();

        path = Path.GetDirectoryName(Environment.GetFolderPath(Environment.SpecialFolder.Personal));
        path = Path.Combine(path, "Downloads");

        webClient.DownloadProgressChanged += new DownloadProgressChangedEventHandler(DownloadProgressCallBack);
        webClient.DownloadFileCompleted += new AsyncCompletedEventHandler(DownloadGameCompletedCallback);
        webClient.DownloadFileAsync(new Uri("http://101.101.218.135:5002/onlineScienceMuseumAPI/downloadInstallFile.do"), path + "\\Launcher_download_Test.exe"); // download build file
        await UniTask.SwitchToMainThread();
    }

    private void DownloadProgressCallBack(object sender, DownloadProgressChangedEventArgs e)
    {
        isDownloading = true;
        Debug.LogError($"런처 다운르도 테스트 : {e.ProgressPercentage}%");
    }

    private async void DownloadGameCompletedCallback(object sender, AsyncCompletedEventArgs e)
    {
        if (isDownloading)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo(path + "\\Launcher_download_Test.exe");
            startInfo.WorkingDirectory = GameManager.instance.ugcManager.LoadUGCFilePath().objectUGCProjectDownloadPath;

            Process.Start(startInfo);

            Debug.LogError("런처 다운로드 완료");

            isCampletedDownloand = true;
        }
        else
        {
            Debug.LogError("런처 재다운로드");
            await UniTask.Delay(3000);
            Test_FileDownload().Forget();
        }
    }
}
