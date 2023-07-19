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

    public Button StateButton;
    public Button onlineButton;
    public Button takeABreakButton;
    public Button otherWorkButton;
    public Button logoutButton;
    

    // Start is called before the first frame update
    void Start()
    {
        StateButton.onClick.AddListener(UsingSettingMenu);
        onlineButton.onClick.AddListener(() => SetPlayerState(0));
        takeABreakButton.onClick.AddListener(() => SetPlayerState(1));
        otherWorkButton.onClick.AddListener(() => SetPlayerState(2));
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

                if (currentState != 1)
                {
                    currentState = 1;
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
        }
        else
        {
            isStateSettings = false;
            settingMenu.SetActive(isStateSettings);
        }
    }

    public void SetPlayerState(int i)
    {
        currentState = i;
        
        switch(i)
        {
            // online
            case 0:
                stateName.text = "온라인";
                icon.color = Color.green;
                break;
            // take a break
            case 1:
                stateName.text = "자리 비움";
                icon.color = Color.yellow;
                break;
            // other work
            case 2:
                stateName.text = "다른 용무 중";
                icon.color = Color.red;
                break;
            // logout
            case 3:
                stateName.text = "오프라인";
                icon.color = Color.gray;
                break;
        }

        isStateSettings = false;
        settingMenu.SetActive(isStateSettings);

        RequestPlayerStateUpdate(i);
    }

    private void RequestPlayerStateUpdate(int i)
    {
        // Todo : Request player state update
        Debug.Log("상태 변경 : " + i);
        GameManager.instance.api.Update_PlayerState().Forget();
    }

    private void ShowLogoutPopup()
    {
        isStateSettings = false;
        settingMenu.SetActive(isStateSettings);
        //GameManager.instance.popupManager.popups[(int)PopupType.logout].SetActive(!isStateSettings);
        GameManager.instance.popupManager.ShowLogoutPage();
    }
}
