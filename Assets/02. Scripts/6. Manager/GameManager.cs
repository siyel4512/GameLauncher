using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Diagnostics;
using UnityEngine;
using Cysharp.Threading.Tasks;

using Debug = UnityEngine.Debug;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("[ Login ]")]
    public bool isLogin;

    [Space(10)]
    [Header("[ Page Setting ]")]
    public GameObject[] pages;

    [Space(10)]
    [Header("[ Using Components ]")]
    public Login login;
    public FilePath filePath;
    //public URL url;
    public PlayerManager playerManager;
    public PopupManager popupManager;
    public FriendListManager friendListManager;
    public RequestFriendManager requestFriendManager;
    public BannerNoticeManager bannerNoticeManager;
    public PCPowerManager pcPowerMnager;
    public UGCManager ugcManager;
    public LanguageManager languageManager;
    public API api;
    public JsonData jsonData;

    [Space(10)]
    [Header("[ File Download Buttons ]")]
    public FileDownload[] SelectButtons;
    public int currentSelectButtonNum;

    [Space(10)]
    [Header("[ Player State Settings ]")]
    public int playerLimitTime;

    [Space(10)]
    [Header("[ Selected Server ]")]
    public int selectedServerNum;

    // refresh timer
    [Space(10)]
    [Header("[ Reflesh Timer ]")]
    public int currentTimeCount;
    public int refreshLimitTime;
    public bool isUsingRefreshTimer;
    public Stopwatch sw;

    [Space(10)]
    [Header("[ PC Power Setting ]")]
    public int turnOffLimitTime;

    [Space(10)]
    [Header("[ Running Files ]")]
    //public Process[] runningFiles = new Process[4];
    public Process[] runningFiles = new Process[5];

    public bool isQuit;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        // set limit Time
        playerManager.limitTime = playerLimitTime;
        pcPowerMnager.limitTime = turnOffLimitTime;
    }

    // Start is called before the first frame update
    void Start()
    {
        sw = new Stopwatch();
    }

    public void Update()
    {
        RefreshTimer();

        if (DEV.instance.isManualRefreshAllData && Input.GetKeyDown(KeyCode.F5) && isLogin)
        {
            Debug.Log("F5 누름 !!!!");
            ManualRefreshAllData();
        }
    }

    #region set launcher page
    public void SetPage(int pageNum)
    {
        HidePages();

        pages[pageNum].SetActive(true);
    }

    public void HidePages()
    {
        for (int i = 0; i < pages.Length; i++)
        {
            pages[i].SetActive(false);
        }
    }
    #endregion

    #region set file download button
    public void SetSelectButton(int buttonNum)
    {
        HideSelectButtons();
        
        UniTask.SwitchToThreadPool();
        SelectButtons[buttonNum].CheckForUpdates().Forget();
        UniTask.SwitchToMainThread();

        currentSelectButtonNum = buttonNum;
        SelectButtons[buttonNum].isSelected = true;
        SelectButtons[buttonNum].selectImage.SetActive(true);
        //UniTask.SwitchToThreadPool();
        //SelectButtons[buttonNum].CheckForUpdates().Forget();
        //UniTask.SwitchToMainThread();
        SelectButtons[buttonNum].excuteButton.gameObject.SetActive(true);
    }

    public void HideSelectButtons()
    {
        for (int i = 0; i < SelectButtons.Length; i++)
        {
            SelectButtons[i].isSelected = false;
            SelectButtons[i].selectImage.SetActive(false);
            SelectButtons[i].excuteButton.gameObject.SetActive(false);
        }
    }
    #endregion

    #region set logout
    public void ResetLauncher()
    {
        playerManager.SetPlayerState(0);
        login.SetLogOut();

        SetPage(0);
    }
    #endregion

    #region refresh data
    public void RefreshTimer()
    {
        if (isLogin)
        {
            if (!isUsingRefreshTimer)
            {
                isUsingRefreshTimer = true;
                sw.Start();
            }

            currentTimeCount = refreshLimitTime - (int)(sw.ElapsedMilliseconds / 1000f);

            // time out
            if (currentTimeCount < 0)
            {
                currentTimeCount = refreshLimitTime;
                sw.Restart();

                AutoRefreshAllData();
            }
        }
        else
        {
            isUsingRefreshTimer = false;
            currentTimeCount = 0;
            sw.Reset();
        }
    }

    // request json data
    public void AutoRefreshAllData()
    {
        // friend list
        api.Request_FriendList().Forget();// create friedn list
        
        // request friend list
        api.Request_RequestFriendList().Forget(); // create request friend list

        // event banner && notice && news
        bannerNoticeManager.CreateAllContents();

        // guide download
        //api.Request_GuideDownload1().Forget();
        //api.Request_GuideDownload2().Forget();

        Debug.Log("Auto RefreshAllData");
    }

    public void ManualRefreshAllData()
    {
        // playr state
        playerManager.RequestPlayerStateUpdate(playerManager.currentState);

        // download file
        filePath.SetDownloadURL(selectedServerNum, currentSelectButtonNum);

        // friend list
        api.Request_FriendList().Forget();// create friedn list

        // request friend list
        api.Request_RequestFriendList().Forget(); // create request friend list

        // event banner && notice && news
        bannerNoticeManager.CreateAllContents();

        // guide download
        //api.Request_GuideDownload1().Forget();
        //api.Request_GuideDownload2().Forget();

        Debug.Log("Manual RefreshAllData");
    }
    #endregion

    public void ForceQuit()
    {
        Debug.Log("[SY] File 강제 종료");
        for (int i = 0; i < runningFiles.Length; i++)
        {
            if (runningFiles[i] != null && !runningFiles[i].HasExited)
            {
                runningFiles[i].Kill();
            }
        }
    }

    private void OnApplicationQuit()
    {
        ForceQuit();
    }
}
