using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Diagnostics;
using UnityEngine;
using Cysharp.Threading.Tasks;

using Debug = UnityEngine.Debug;
using System.Windows.Forms;
using static SaveData;

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
    public URL url;
    public PlayerManager playerManager;
    public PopupManager popupManager;
    public FriendListManager friendListManager;
    public RequestFriendManager requestFriendManager;
    public BannerNoticeManager bannerNoticeManager;
    public API api;
    public JsonData jsonData;

    [Space(10)]
    [Header("[ File Download Buttons ]")]
    public FileDownload[] SelectButtons;

    [Space(10)]
    [Header("[ Friend List Settings ]")]
    public int friendCount; // 삭제 예정
    public int requestFriendCount; // 삭제 예정

    [Space(10)]
    [Header("[ Player State Settings ]")]
    public int playerLimitTime;

    [Space(10)]
    [Header("[ Selected Server ]")]
    public int selectedServerNum;

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
    }

    // Start is called before the first frame update
    void Start()
    {
        playerManager.limitTime = playerLimitTime;

        //jsonData.friendListValues = JsonUtility.FromJson<SaveData>(api.friendList).frndInfoList;
        //jsonData.requestFriendListValues = JsonUtility.FromJson<SaveData>(api.requestFriendList).requestFriend_List;
        //friendListManager.friendCount = jsonData.friendListValues.Count;
        //requestFriendManager.requestfriendCount = jsonData.requestFriendListValues.Count;

        sw = new Stopwatch();
    }

    public void Update()
    {
        RefreshTimer();
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

        SelectButtons[buttonNum].isSelected = true;
        SelectButtons[buttonNum].selectImage.SetActive(true);
        UniTask.SwitchToThreadPool();
        SelectButtons[buttonNum].CheckForUpdates().Forget();
        UniTask.SwitchToMainThread();
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
        login.SetLogOut();
        playerManager.SetPlayerState(3);

        SetPage(0);
    }
    #endregion

    #region refresh data
    // timer
    [Space(10)]
    [Header("[ Reflesh Timer ]")]
    public int currentTimeCount;
    public int limitTime;
    public bool isUsingRefreshTimer;
    public Stopwatch sw;

    public void RefreshTimer()
    {
        if (isLogin)
        {
            if (!isUsingRefreshTimer)
            {
                isUsingRefreshTimer = true;
                sw.Start();
            }

            currentTimeCount = limitTime - (int)(sw.ElapsedMilliseconds / 1000f);

            // time out
            if (currentTimeCount < 0)
            {
                currentTimeCount = limitTime;
                sw.Restart();

                RefreshAllData();
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
    public void RefreshAllData()
    {
        // friend list
        api.Request_FriendList().Forget();// create friedn list
        
        // request friend list
        api.Request_RequestFriendList().Forget(); // create request friend list

        // file download url
        switch(selectedServerNum)
        {
            case 0:
                // dev server
                break;
            case 1:
                // test server
                break;
            case 2:
                // staging server
                break;
            case 3:
                // live server
                break;
        }

        // event banner
        api.Request_EventBanner().Forget();

        // notice
        api.Request_Notice().Forget();

        // curiverse notice
        api.Request_CuriverseNotice().Forget();

        // guide download
        api.Request_GuideDownload1().Forget();
        api.Request_GuideDownload2().Forget();

        Debug.Log("Request Data");
    }

    #endregion
}
