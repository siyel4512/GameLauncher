using System;
using System.Diagnostics;
using System.IO;
using UnityEngine;

using Debug = UnityEngine.Debug;

//#if UNITY_EDITOR
//using UnityEditor.Callbacks;
//#endif

public class DEV : MonoBehaviour
{
    public static DEV instance;

    [Space(10)]
    public bool isTEST_Login;
    public bool isTEST_UpdatePlayerState;
    public bool isUsingFolderDialog;
    public bool isTEST_Contents;
    public bool isManualRefreshAllData = false;

    [Space(10)]
    public bool isAdmin;
    public bool isProtectFileDownload; // 다운로드 중 버튼 누름 방지
    public bool isFileDownload; // 파일 다운로드 중
    public bool isScienceMuseum;
    public bool isUsingTokenForFriendList; // 친구 리스트 요청시 token값 사용
    //public bool isUsingTmpForInputField; // input field에서 TMPro를 사용할 것인지
    public bool isUsingAbnormalShutdown;

    [Space(10)]
    public bool isUsingTestServer;
    public bool isLoginToTestServer; // 테스트 서버로 로그인

    [Header("==========================")]
    public bool isTempBannerMode; // cdn 이미지 다운로드 문제 해결시 삭제 예정
    public bool isCdnTest;

    [Space(10)]
    public FilePath filePath;

    [Space(10)]
    public UGCManager ugcManager;

    [Space(10)]
    public SelectServer selectServer;
    public int selectedServerNum; // div 용

    [Space(10)]
    public DefaultSettings defaultSettings;

    [Space(10)]
    public WindowSizeInitializer windowSizeInitializer;

    public PCPowerManager pcPowerManager;
    public VersionManager versionManager;
    public Process process;

    public GameObject downloadProtectGaurd;

    [Space(10)]
    public string CDN_Live;
    public string CDN_Test;

    // Start is called before the first frame update
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        defaultSettings = new DefaultSettings();
    }

    private void Start()
    {
        if (isScienceMuseum)
        {
            pcPowerManager.gameObject.SetActive(true);
        }
    }

    public void Test()
    {
        var desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        var myDocumentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        var programFilesPath = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
        var windowsPath = Environment.GetFolderPath(Environment.SpecialFolder.Windows);
        var systemPath = Environment.GetFolderPath(Environment.SpecialFolder.System);
        string downloadsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");

        Debug.Log($"desktopPath : {desktopPath}");
        Debug.Log($"myDocumentsPath : {myDocumentsPath}");
        Debug.Log($"programFilesPath : {programFilesPath}");
        Debug.Log($"windowsPath : {windowsPath}");
        Debug.Log($"systemPath : {systemPath}");
        Debug.Log($"downloadsPath : {downloadsPath}");
    }

    public void Test_CheckProcessState()
    {
        if (process == null)
            return;

        if (process.HasExited)
        {
            Debug.Log("실행파일은 종료됨");
        }
        else
        {
            Debug.Log("아직 실행파일 실행중...");
        }
    }


    private void OnApplicationQuit()
    {
        //if (LoadSettingValues().isFirstDownload)
        //{
        //    PlayerPrefs.DeleteAll();
        //    SetSettingValue(false);
        //    //ResetSettingValue();
        //}

        PlayerPrefs.DeleteAll();
    }

    // Todo : 
    #region Default Settings 
    // reset setting value
    public void SetSettingValue(bool isDownloadState)
    {
        defaultSettings.isFirstDownload = isDownloadState;

        string jsonData = JsonUtility.ToJson(defaultSettings, true);
        string path = Path.Combine(Application.streamingAssetsPath + "/Default Settings", "DefaultSettings.json");
        File.WriteAllText(path, jsonData);
    }

    // load setting value
    public DefaultSettings LoadSettingValues()
    {
        string path = Path.Combine(Application.streamingAssetsPath + "/Default Settings", "DefaultSettings.json");
        string jsonData = File.ReadAllText(path);
        defaultSettings = JsonUtility.FromJson<DefaultSettings>(jsonData);

        return defaultSettings;
    }

    // reset setting value
    public void ResetSettingValue()
    {
        defaultSettings.isFirstDownload = true;

        string jsonData = JsonUtility.ToJson(defaultSettings, true);
        string path = Path.Combine(Application.streamingAssetsPath + "/Default Settings", "DefaultSettings.json");
        File.WriteAllText(path, jsonData);
    }
    #endregion
}

public class DefaultSettings
{
    public bool isFirstDownload;
}
