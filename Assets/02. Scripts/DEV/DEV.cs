using Org.BouncyCastle.Bcpg.OpenPgp;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

//#if UNITY_EDITOR
//using UnityEditor.Callbacks;
//#endif

public class DEV : MonoBehaviour
{
    public static DEV instance;

    public bool isTEST;
    public bool isUsingFolderDialog;
    public bool isTEST_Contents;
    public bool isTEST_Server;

    public FilePath filePath;
    public string[] rootPaths; // div 용

    public SelectServer selectServer;
    public int selectedServerNum; // div 용

    public DefaultSettings defaultSettings;

    public WindowSizeInitializer windowSizeInitializer;

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

        rootPaths = new string[4];
        defaultSettings = new DefaultSettings();

        //Debug.LogError("[SY] 1 : " + LoadSettingValues().isFirstDownload);
        ////PlayerPrefs.DeleteAll();

        //if (LoadSettingValues().isFirstDownload)
        //{
        //    Debug.LogError("[SY] 2 : " + LoadSettingValues().isFirstDownload);
        //    SetSettingValue(false);
        //    Debug.LogError("[SY] 3 : " + LoadSettingValues().isFirstDownload);
        //    windowSizeInitializer.SetWindowSize();
        //    Debug.LogError("[SY] 4 : " + LoadSettingValues().isFirstDownload);
        //}
    }

    //    private void Start()
    //    {
    //#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
    //        Debug.LogError("[SY] 1 : " + LoadSettingValues().isFirstDownload);
    //        //PlayerPrefs.DeleteAll();

    //        if (LoadSettingValues().isFirstDownload)
    //        {
    //            Debug.LogError("[SY] 2 : " + LoadSettingValues().isFirstDownload);
    //            SetSettingValue(false);
    //            Debug.LogError("[SY] 3 : " + LoadSettingValues().isFirstDownload);
    //            windowSizeInitializer.SetWindowSize();
    //            Debug.LogError("[SY] 4 : " + LoadSettingValues().isFirstDownload);
    //        }
    //#endif
    //    }

    //private void Start()
    //{
    //    //if (LoadSettingValues().isFirstDownload)
    //    {
    //        PlayerPrefs.DeleteAll();
    //        SetSettingValue(false);
    //        //ResetSettingValue();
    //    }
    //}

    public async void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("다운로드 경로 테스트");
            //await GameManager.instance.api.Request_FileDownloadURL(ServerType.dev.ToString(), FileType.pc.ToString());
            await GameManager.instance.api.Request_FileDownloadURL(ServerType.dev, FileType.pc);
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
