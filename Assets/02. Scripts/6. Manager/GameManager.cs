using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

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
    public FrientListManager friendListManager;
    public RequestFriendManager requestFriendManager;
    public BannerNoticeManager bannerNoticeManager;

    [Space(10)]
    [Header("[ File Download Buttons ]")]
    public FileDownload[] SelectButtons;

    [Space(10)]
    [Header("[ Friend List Settings ]")]
    public int friendCount;
    public int requestFriendCount;

    [Space(10)]
    [Header("[ Player State Settings ]")]
    public int playerLimitTime;

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
        friendListManager.friendCount = friendCount;
        requestFriendManager.requestfriendCount = requestFriendCount;
    }

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

    public void ResetLauncher()
    {
        login.SetLogOut();
        playerManager.SetPlayerState(3);

        SetPage(0);
    }
}
