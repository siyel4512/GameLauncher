using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Tables;
using System.IO;
using Cysharp.Threading.Tasks;
using System.Reflection;
using DG.Tweening.Core.Easing;

public class LanguageManager : MonoBehaviour
{
    public int currentLanguageNum = 0; // 0:english 1:korean

    public TMP_Dropdown selectServer; // 관리자가 로그인시 사용할 서버 선택용 드롭다운
    public PlayerManager playerManager;
    public FriendListManager friendListManager;

    bool isChange = false; // localizing 중 대기용 변수

    // json 저정 관련
    public LanguageState languageState;
    public string jsonFilePath;

    public LanguageButtonAnimation languageButtonAnimation;
    private bool isInitLanguage = true;

    // event banner
    public GameObject[] eventBanners;
    public GameObject videoPlayer;

    // guide link
    public string launcherUserGuide;
    public string UGCInstallMaual;

    private void Awake()
    {
        jsonFilePath = Path.Combine(Application.streamingAssetsPath + "/Default Settings", "Language.json");
        currentLanguageNum = LoadData().LanguageNum;
        ChangeLanguage();
    }

    // Start is called before the first frame update
    void Start()
    {
        playerManager = GameManager.instance.playerManager;
        friendListManager = GameManager.instance.friendListManager;
    }

    //public void Update()
    //{
    //    if (Input.GetKeyDown(KeyCode.Space) && GameManager.instance.isLogin)
    //    {
    //        GameManager.instance.bannerNoticeManager.bannerUI.ChangeLanguage();
    //        GameManager.instance.bannerNoticeManager.noticeUI.ChangeLanguage();
    //    }
    //}

    // change language
    public void ChangeLanguage()
    {
        if (isChange)
            return;

        if (!isInitLanguage)
        {
            Debug.Log("[language] change launcher language");
            // change launcher language
            if (currentLanguageNum == 0)
            {
                currentLanguageNum++;
            }
            else if (currentLanguageNum == 1)
            {
                currentLanguageNum--;
            }

            languageButtonAnimation.SetLanguageButton(currentLanguageNum);
            StartCoroutine(Co_ChangeLanguage(currentLanguageNum));
        }
        else
        {
            // init launcher language
            Debug.Log("[language] init launcher language");
            languageButtonAnimation.SetLanguageButton(currentLanguageNum);
            StartCoroutine(Co_ChangeLanguage(currentLanguageNum));
        }
    }

    IEnumerator Co_ChangeLanguage(int index)
    {
        isChange = true;
        //currentLanguageNum = index;
        Debug.Log($"로컬라이징 테스트 : {currentLanguageNum}");
        
        // event banner change
        if (currentLanguageNum == 1)
        {
            // korean
            eventBanners[0].SetActive(true);
            eventBanners[1].SetActive(false);
            videoPlayer.SetActive(false);
        }
        else
        {
            // english
            eventBanners[0].SetActive(true);
            eventBanners[1].SetActive(true);
            videoPlayer.SetActive(true);
        }

        // event banner state reset
        if (!isInitLanguage)
        {
            BannerUI banner = GameManager.instance.bannerNoticeManager.bannerUI;

            if (currentLanguageNum == 1)
            {
                // korean
                if (banner != null && banner.isComplateLoaded)
                {
                    // 타이머 시작 및 위치 초기화
                    GameManager.instance.bannerNoticeManager.bannerUI.ReStartBanner();
                    GameManager.instance.bannerNoticeManager.noticeUI.ReStartBanner();
                }
            }
            else
            {
                // english
                if (banner != null && banner.isComplateLoaded)
                {
                    // 타이머 시작 및 위치 초기화
                    GameManager.instance.bannerNoticeManager.bannerUI.StopBanner();
                    GameManager.instance.bannerNoticeManager.noticeUI.StopBanner();
                }
            }
        }

        yield return LocalizationSettings.InitializationOperation;
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[index];

        isChange = false;

        var localizationLoadOperation = LocalizationSettings.InitializationOperation;

        while (!localizationLoadOperation.IsDone)
        {
            yield return null;
        }

        if (!isInitLanguage)
        {
            Debug.Log("[language] language num save");
            SaveData(currentLanguageNum);
        }

        isInitLanguage = false;

        SetPlayerState();
        SetFriendListState();
        SetAddFriendWarningText();
        SetSelectServerDropdown();
        SetGuideLink();
    }

    // player state localizing
    private void SetPlayerState()
    {
        Debug.Log(LocalizationSettings.StringDatabase.GetLocalizedString("Player State Table", "online button"));

        switch (playerManager.currentState)
        {
            case 1:
                // online
                playerManager.stateName.text = LocalizationSettings.StringDatabase.GetLocalizedString("Player State Table", "online button");
                break;
            case 2:
                // Take a Break
                playerManager.stateName.text = LocalizationSettings.StringDatabase.GetLocalizedString("Player State Table", "take a break button");
                break;
            case 3:
                // Other Work
                playerManager.stateName.text = LocalizationSettings.StringDatabase.GetLocalizedString("Player State Table", "other work button");
                break;
        }
    }

    // friend list localizing
    private void SetFriendListState()
    {
        if (friendListManager.friendList.Count <= 0)
            return;

        for (int i = 0; i < friendListManager.friendList.Count; i++)
        {
            friendListManager.friendList[i].SetSlotValues();
        }
    }

    // friend list warning localizing
    private void SetAddFriendWarningText()
    {
        switch (friendListManager.currentWarningTextNum)
        {
            case 1:
                friendListManager.searchUserWaringText.text = LocalizationSettings.StringDatabase.GetLocalizedString("Friend List Table", "search user waring text 1");
                break;
            case 2:
                friendListManager.searchUserWaringText.text = LocalizationSettings.StringDatabase.GetLocalizedString("Friend List Table", "search user waring text 2");
                break;
            case 3:
                friendListManager.searchUserWaringText.text = LocalizationSettings.StringDatabase.GetLocalizedString("Friend List Table", "search user waring text 3");
                break;
            case 4:
                friendListManager.searchUserWaringText.text = LocalizationSettings.StringDatabase.GetLocalizedString("Friend List Table", "search user waring text 4");
                break;
            case 5:
                friendListManager.searchUserWaringText.text = LocalizationSettings.StringDatabase.GetLocalizedString("Friend List Table", "search user waring text 5");
                break;
        }
    }

    // select server localizing
    private void SetSelectServerDropdown()
    {
        for (int i = 0; i < selectServer.options.Count; i++)
        {
            switch (i)
            {
                case 0:
                    selectServer.options[i].text = LocalizationSettings.StringDatabase.GetLocalizedString("Login Table", "dropdown-Dev server");
                    break;
                case 1:
                    selectServer.options[i].text = LocalizationSettings.StringDatabase.GetLocalizedString("Login Table", "dropdown-Test server");
                    break;
                case 2:
                    selectServer.options[i].text = LocalizationSettings.StringDatabase.GetLocalizedString("Login Table", "dropdown-Staging server");
                    break;
                case 3:
                    selectServer.options[i].text = LocalizationSettings.StringDatabase.GetLocalizedString("Login Table", "dropdown-Live server");
                    break;
            }

        }
        selectServer.captionText.text = selectServer.options[selectServer.value].text;
    }

    public void SetGuideLink()
    {
        BannerNoticeManager bannerNoticeManager = GameManager.instance.bannerNoticeManager;

        // test server
        if (DEV.instance.isUsingTestServer)
        {
            // korean
            if (currentLanguageNum == 1)
            {
                //guideInfo[0].SetLinkURL("http://fgnowlvzhshz17884402.cdn.ntruss.com/onlinemuseum/curiverseUserGuide/launcherUserGuideBook.pdf");
                //guideInfo[1].SetLinkURL("http://fgnowlvzhshz17884402.cdn.ntruss.com/onlinemuseum/ugcAuthoringToolGuide/UGC_install_manual.pdf");
                //bannerNoticeManager.guideInfo[0].SetLinkURL("https://ejrdejzsaflk20717940.cdn.ntruss.com/onlinemuseum/curiverseUserGuide/launcherUserGuideBook.pdf");
                //bannerNoticeManager.guideInfo[1].SetLinkURL("https://ejrdejzsaflk20717940.cdn.ntruss.com/onlinemuseum/ugcAuthoringToolGuide/UGC_install_manual.pdf");

                bannerNoticeManager.guideInfo[0].SetLinkURL(DEV.instance.CDN_Test + bannerNoticeManager.launcherUserGuideLinks[0]);
                bannerNoticeManager.guideInfo[1].SetLinkURL(DEV.instance.CDN_Test + bannerNoticeManager.UGCInstallMenualLinks[0]);
            }
            // english
            else
            {
                //guideInfo[0].SetLinkURL("http://fgnowlvzhshz17884402.cdn.ntruss.com/onlinemuseum/curiverseUserGuide/launcherUserGuideBook.pdf");
                //guideInfo[1].SetLinkURL("http://fgnowlvzhshz17884402.cdn.ntruss.com/onlinemuseum/ugcAuthoringToolGuide/UGC_install_manual.pdf");
                //bannerNoticeManager.guideInfo[0].SetLinkURL("https://ejrdejzsaflk20717940.cdn.ntruss.com/onlinemuseum/curiverseUserGuide/launcherUserGuideBook.pdf");
                //bannerNoticeManager.guideInfo[1].SetLinkURL("https://ejrdejzsaflk20717940.cdn.ntruss.com/onlinemuseum/ugcAuthoringToolGuide/UGC_install_manual.pdf");

                bannerNoticeManager.guideInfo[0].SetLinkURL(DEV.instance.CDN_Test + bannerNoticeManager.launcherUserGuideLinks[1]);
                bannerNoticeManager.guideInfo[1].SetLinkURL(DEV.instance.CDN_Test + bannerNoticeManager.UGCInstallMenualLinks[1]);
            }
            
        }
        // live server
        else
        {
            // korean
            if (currentLanguageNum == 1)
            {
                //guideInfo[0].SetLinkURL("http://alilgjwknwlm18374611.cdn.ntruss.com/onlinemuseum/curiverseUserGuide/launcherUserGuideBook.pdf");
                //guideInfo[1].SetLinkURL("http://alilgjwknwlm18374611.cdn.ntruss.com/onlinemuseum/ugcAuthoringToolGuide/UGC_install_manual.pdf");
                //bannerNoticeManager.guideInfo[0].SetLinkURL("https://yhbdymjatqnq20869625.cdn.ntruss.com/onlinemuseum/curiverseUserGuide/launcherUserGuideBook.pdf");
                //bannerNoticeManager.guideInfo[1].SetLinkURL("https://yhbdymjatqnq20869625.cdn.ntruss.com/onlinemuseum/ugcAuthoringToolGuide/UGC_install_manual.pdf");

                bannerNoticeManager.guideInfo[0].SetLinkURL(DEV.instance.CDN_Live + bannerNoticeManager.launcherUserGuideLinks[0]);
                bannerNoticeManager.guideInfo[1].SetLinkURL(DEV.instance.CDN_Live + bannerNoticeManager.UGCInstallMenualLinks[0]);
            }
            // english
            else
            {
                //guideInfo[0].SetLinkURL("http://alilgjwknwlm18374611.cdn.ntruss.com/onlinemuseum/curiverseUserGuide/launcherUserGuideBook.pdf");
                //guideInfo[1].SetLinkURL("http://alilgjwknwlm18374611.cdn.ntruss.com/onlinemuseum/ugcAuthoringToolGuide/UGC_install_manual.pdf");
                //bannerNoticeManager.guideInfo[0].SetLinkURL("https://yhbdymjatqnq20869625.cdn.ntruss.com/onlinemuseum/curiverseUserGuide/launcherUserGuideBook.pdf");
                //bannerNoticeManager.guideInfo[1].SetLinkURL("https://yhbdymjatqnq20869625.cdn.ntruss.com/onlinemuseum/ugcAuthoringToolGuide/UGC_install_manual.pdf");

                bannerNoticeManager.guideInfo[0].SetLinkURL(DEV.instance.CDN_Live + bannerNoticeManager.launcherUserGuideLinks[1]);
                bannerNoticeManager.guideInfo[1].SetLinkURL(DEV.instance.CDN_Live + bannerNoticeManager.UGCInstallMenualLinks[1]);
            }
        }
    }

    #region Data Save
    public void SaveData(int _languageNum)
    {
        languageState.LanguageNum = _languageNum;

        string jsonData = JsonUtility.ToJson(languageState, true);
        string languageNum = jsonFilePath;
        File.WriteAllText(languageNum, jsonData);
    }

    // load server num data
    public LanguageState LoadData()
    {
        string serverNum = jsonFilePath;
        string jsonData = File.ReadAllText(serverNum);
        languageState = JsonUtility.FromJson<LanguageState>(jsonData);
        return languageState;
    }

    public void ResetSelectedServer()
    {
        languageState.LanguageNum = 1;

        string jsonData = JsonUtility.ToJson(languageState, true);
        string path = jsonFilePath;
        File.WriteAllText(path, jsonData);
    }
    #endregion
}
[System.Serializable]
public class LanguageState{
    public int LanguageNum;
}
