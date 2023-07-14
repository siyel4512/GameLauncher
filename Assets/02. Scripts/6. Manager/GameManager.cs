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
    
    public bool isLogin;
    
    public GameObject[] pages;
    public FileDownload[] SelectButtons;

    public Login login;
    public FilePath filePath;
    public URL url;
    public PlayerManager playerManager;
    public PopupManager popupManager;
    public FrientListManager friendListManager;
    public RequestFriendManager requestFriendManager;
    public BannerNoticeManager bannerNoticeManager;

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
