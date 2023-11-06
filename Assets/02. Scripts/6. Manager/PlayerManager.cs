using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

using Debug = UnityEngine.Debug;
using UnityEngine.Localization.Settings;

public class PlayerManager : MonoBehaviour
{
    // timer
    [Header("[ Timer Settings ]")]
    public int limitTime;
    public int currentTimeCount;
    private bool isStartTimer;
    private Stopwatch sw;

    // player nickname & state
    [Space(10)]
    [Header("[ Player State Settings ]")]
    public TMP_Text nickname;
    public Text nickname_legacy;
    public TMP_Text stateName;
    public Image icon; // state icon
    public GameObject[] stateIcons;
    public int currentState;

    // setting menu buttons
    [Space(10)]
    [Header("[ Setting Menu ]")]
    public GameObject settingMenu;
    public bool isStateSettings;
    public GameObject[] menuStateIcons;

    public Button StateButton;
    public Button onlineButton;
    public Button takeABreakButton;
    public Button otherWorkButton;
    public Button downloadButton;
    public Button logoutButton;

    public TMP_Text[] stateNames = new TMP_Text[3];

    // Start is called before the first frame update
    void Start()
    {
        StateButton.onClick.AddListener(UsingSettingMenu);
        onlineButton.onClick.AddListener(() => SetPlayerState(1));
        takeABreakButton.onClick.AddListener(() => SetPlayerState(2));
        otherWorkButton.onClick.AddListener(() => SetPlayerState(3));
        downloadButton.onClick.AddListener(ShowDownloadSettingPopup);
        logoutButton.onClick.AddListener(ShowLogoutPopup);

        sw = new Stopwatch();
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.instance.isLogin)
        {
            // start timer
            if (!isStartTimer)
            {
                isStartTimer = true;
                sw.Start();
            }

            // detect using
            if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1) || Input.GetMouseButtonDown(2))
            {
                currentTimeCount = limitTime;
                sw.Restart();
            }

            // calculate current time
            currentTimeCount = limitTime - (int)(sw.ElapsedMilliseconds / 1000f);
            
            // time out
            if (currentTimeCount < 0)
            {
                currentTimeCount = limitTime;
                sw.Restart();

                if (currentState != 2)
                {
                    currentState = 2;
                    //stateName.text = "자리 비움";
                    stateName.text = stateNames[1].text;
                    //icon.color = Color.yellow;
                    SelectStateIcon(1);
                    RequestPlayerStateUpdate(currentState);
                }
            }
        }
    }

    public void StopTimer()
    {
        sw.Stop();
        isStartTimer = false;
    }

    public void UsingSettingMenu()
    {
        if (!isStateSettings)
        {
            isStateSettings = true;
            settingMenu.SetActive(isStateSettings);
            menuStateIcons[0].SetActive(false);
            menuStateIcons[1].SetActive(true);
        }
        else
        {
            isStateSettings = false;
            settingMenu.SetActive(isStateSettings);
            menuStateIcons[0].SetActive(true);
            menuStateIcons[1].SetActive(false);
        }
    }

    public void SetPlayerState(int i)
    {
        currentState = i;

        switch (i)
        {
            // offline
            case 0:
                stateName.text = "오프라인";
                //icon.color = Color.gray;
                SelectStateIcon(3);
                break;
            // online
            case 1:
                //stateName.text = "온라인";
                //stateName.text = stateNames[0].text;
                stateName.text = LocalizationSettings.StringDatabase.GetLocalizedString("Player State Table", "online button");
                //icon.color = Color.green;
                SelectStateIcon(0);
                break;
            // take a break
            case 2:
                //stateName.text = "자리 비움";
                //stateName.text = stateNames[1].text;
                stateName.text = LocalizationSettings.StringDatabase.GetLocalizedString("Player State Table", "take a break button");
                //icon.color = Color.yellow;
                SelectStateIcon(1);
                break;
            // other work
            case 3:
                //stateName.text = "다른 용무 중";
                //stateName.text = stateNames[2].text;
                stateName.text = LocalizationSettings.StringDatabase.GetLocalizedString("Player State Table", "other work button");
                //icon.color = Color.red;
                SelectStateIcon(2);
                break;
        }

        isStateSettings = false;
        settingMenu.SetActive(isStateSettings);

        RequestPlayerStateUpdate(i);
    }

    public async void RequestPlayerStateUpdate(int i)
    {
        // Todo : Request player state update
        //GameManager.instance.api.Update_PlayerState(i, Login.PID).Forget();
        if (!DEV.instance.isTEST_UpdatePlayerState)
        {
            await GameManager.instance.api.Update_PlayerState(i, Login.PID);
        }
        else
        {
            Debug.Log($"{i}번으로 상태 변경 요청 완료!!!");
        }
    }

    private void ShowLogoutPopup()
    {
        if (!DEV.instance.isFileDownload)
        {
            isStateSettings = false;
            settingMenu.SetActive(isStateSettings);
            //GameManager.instance.popupManager.popups[(int)PopupType.logout].SetActive(!isStateSettings);
            GameManager.instance.popupManager.ShowLogoutPage();
        }
        else
        {
            GameManager.instance.popupManager.popups[(int)PopupType.logoutFailed].SetActive(true);
        }
    }

    public void ShowDownloadSettingPopup()
    {
        isStateSettings = false;
        settingMenu.SetActive(isStateSettings);
        GameManager.instance.popupManager.popups[(int)PopupType.DownloadSetting].SetActive(!isStateSettings);
    }

    private void SelectStateIcon(int iconNum)
    {
        for (int i = 0; i < stateIcons.Length; i++)
        {
            stateIcons[i].SetActive(false);
        }

        stateIcons[iconNum].SetActive(true);
    }

    private void OnApplicationQuit()
    {
        Debug.Log("종료 시작");

        Debug.Log("Login.PID 값 : " + Login.PID);

        GameManager.instance.isQuit = true;

        if (Login.PID != "")
        {
            RequestPlayerStateUpdate(0);
        }
    }
}
