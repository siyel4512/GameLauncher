using UnityEngine;
using TMPro;
using System.Diagnostics;

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
    public TMP_Dropdown dropdown;
    public int currentState;

    private Stopwatch sw;
    public int limitTime;
    public int currentTimeCount;

    // Start is called before the first frame update
    void Start()
    {
        // Todo : set dropdown & set stopwatch
        currentState = dropdown.value;

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
                dropdown.value = 1;
                sw.Restart();
            }
        }

        //if(Input.GetKeyDown(KeyCode.Space))
        //{
        //    dropdown.value = 100;
        //}
    }

    public void OnChengedValue()
    {
        currentState = dropdown.value;
        Debug.Log("update player state : " + currentState);
    }

    public bool isTryLogout;
    public GameObject logoutButton;

    public void TryLogout()
    {
        if (!isTryLogout)
        {
            isTryLogout = true;
            logoutButton.SetActive(isTryLogout);
        }
        else
        {
            isTryLogout = false;
            logoutButton.SetActive(isTryLogout);
        }
    }

    public void ShowLogoutPopup()
    {
        isTryLogout = false;
        logoutButton.SetActive(isTryLogout);
        GameManager.instance.popupManager.popups[1].SetActive(!isTryLogout);
    }
}
