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
    //public TMP_Dropdown dropdown;
    public int currentState;

    // timer
    private Stopwatch sw;
    public int limitTime;
    public int currentTimeCount;

    // player nickname & state
    public TMP_Text nickname;
    public TMP_Text stateName;
    public Image icon;

    // setting menu buttons
    public Button StateButton;
    public Button onlineButton;
    public Button takeABreakButton;
    public Button otherWorkButton;
    public Button logoutButton;

    public bool isStateSettings;
    public GameObject settingMenu;

    // Start is called before the first frame update
    void Start()
    {
        StateButton.onClick.AddListener(UsingSettingMenu);
        onlineButton.onClick.AddListener(() => SetPlayerState(0));
        takeABreakButton.onClick.AddListener(() => SetPlayerState(1));
        otherWorkButton.onClick.AddListener(() => SetPlayerState(2));
        logoutButton.onClick.AddListener(ShowLogoutPopup);

        // Todo : set dropdown & set stopwatch
        //currentState = dropdown.value;

        sw = new Stopwatch();
        sw.Start();
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.instance.isLogin)
        {
            if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1) || Input.GetMouseButtonDown(2))
            {
                Debug.Log("using detect");
                currentTimeCount = limitTime;
                sw.Restart();
            }

            currentTimeCount = limitTime - (int)(sw.ElapsedMilliseconds / 1000f);

            if (currentTimeCount < 0)
            {
                Debug.Log("time out");
                currentTimeCount = limitTime;
                //dropdown.value = 1;
                sw.Restart();
            }
        }

        //if(Input.GetKeyDown(KeyCode.Space))
        //{
        //    dropdown.value = 100;
        //}
    }

    //public void OnChengedValue()
    //{
    //    currentState = dropdown.value;
    //    Debug.Log("update player state : " + currentState);
    //}

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
    }

    private void ShowLogoutPopup()
    {
        isStateSettings = false;
        settingMenu.SetActive(isStateSettings);
        //GameManager.instance.popupManager.popups[(int)PopupType.logout].SetActive(!isStateSettings);
        GameManager.instance.popupManager.ShowLogoutPage();
    }
}
