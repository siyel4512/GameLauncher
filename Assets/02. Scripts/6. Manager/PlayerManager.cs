using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

using Debug = UnityEngine.Debug;

//public enum PlayerState
//{
//    Online,
//    Take_a_Break,
//    Other_Work,
//    Offline
//} 

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
    public TMP_Text stateName;
    public Image icon; // state icon
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
    public Button logoutButton;
    

    // Start is called before the first frame update
    void Start()
    {
        StateButton.onClick.AddListener(UsingSettingMenu);
        onlineButton.onClick.AddListener(() => SetPlayerState(1));
        takeABreakButton.onClick.AddListener(() => SetPlayerState(2));
        otherWorkButton.onClick.AddListener(() => SetPlayerState(3));
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
                    stateName.text = "자리 비움";
                    icon.color = Color.yellow;
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
                icon.color = Color.gray;
                break;
            // online
            case 1:
                stateName.text = "온라인";
                icon.color = Color.green;
                break;
            // take a break
            case 2:
                stateName.text = "자리 비움";
                icon.color = Color.yellow;
                break;
            // other work
            case 3:
                stateName.text = "다른 용무 중";
                icon.color = Color.red;
                break;
        }

        isStateSettings = false;
        settingMenu.SetActive(isStateSettings);

        RequestPlayerStateUpdate(i);
    }

    private async void RequestPlayerStateUpdate(int i)
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
        isStateSettings = false;
        settingMenu.SetActive(isStateSettings);
        //GameManager.instance.popupManager.popups[(int)PopupType.logout].SetActive(!isStateSettings);
        GameManager.instance.popupManager.ShowLogoutPage();
    }
}
