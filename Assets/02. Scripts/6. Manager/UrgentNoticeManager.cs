using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using Application = UnityEngine.Application;
using Debug = UnityEngine.Debug;
using UnityEngine.UI;

public class UrgentNoticeManager : MonoBehaviour
{
    public UrgentNoticeCheckData checkData;
    public GameObject urgentNoticePopup;
    public Toggle viewAgainToggle;
    public Button closeButton;

    #region Default Setting
    //public void SetupNotice()
    //{
    //    urgentNoticePopup.SetActive(CheckData()); // Todo : ��ް��� ǥ�� ���� üũ (�ʿ��� ��ġ�� �߰��Ұ�)
    //}

    public void BTN_Close()
    {
        // �����Ϸ� �Ⱥ�
        if (viewAgainToggle.isOn)
        {
            SaveCheckData();
        }

        // popupâ �ݱ�
        urgentNoticePopup.SetActive(false);
        viewAgainToggle.isOn = false;
        Debug.Log($"�ݱ� ��ư ���� / {viewAgainToggle.isOn}");
    }
    #endregion

    #region Json file Read and Write
    public bool CheckData()
    {
        bool isShowPopup;
        viewAgainToggle.isOn = false;
        UrgentNoticeCheckData loadData = LoadCheckData();
        DateTime currentTime = DateTime.Now;

        //Debug.Log($"{currentTime.Date} / {currentTime}");
        //Debug.Log($"{currentTime.Year} / {currentTime.Month} / {currentTime.DayOfYear}");

        // hide urgent notice popup
        if (loadData.year == currentTime.Year.ToString()
            && loadData.month == currentTime.Month.ToString()
            && loadData.day == currentTime.DayOfYear.ToString())
        {
            Debug.Log("[urgent notice] : hide popup");
            isShowPopup = false;
        }
        // show urgent notice popup
        else
        {
            Debug.Log("[urgent notice] : show popup");
            isShowPopup = true;
        }

        return isShowPopup;
    }

    public void SaveCheckData()
    {
        DateTime currentTime = DateTime.Now;

        checkData.year = currentTime.Year.ToString();
        checkData.month = currentTime.Month.ToString();
        checkData.day = currentTime.DayOfYear.ToString();

        string jsonData = JsonUtility.ToJson(checkData, true);
        string path = Path.Combine(Application.streamingAssetsPath + "/UrgentNotice", "UrgentNotice.json");
        File.WriteAllText(path, jsonData);
    }

    public UrgentNoticeCheckData LoadCheckData()
    {
        string path = Path.Combine(Application.streamingAssetsPath + "/UrgentNotice", "UrgentNotice.json");
        string jsonData = File.ReadAllText(path);
        checkData = JsonUtility.FromJson<UrgentNoticeCheckData>(jsonData);

        return checkData;
    }

    public void ResetCheckData()
    {
        checkData.year = "";
        checkData.month = "";
        checkData.day = "";

        string jsonData = JsonUtility.ToJson(checkData, true);
        string path = Path.Combine(Application.streamingAssetsPath + "/UrgentNotice", "UrgentNotice.json");
        File.WriteAllText(path, jsonData);
    }
    #endregion

    #region Test Urgent Notice
    [Header("[ UI ]")]
    public UrgentNoticeUI noticeUI;

    [Space(10)]
    [Header("[ Contents Count ]")]
    public int eventNewsCount;

    [Space(10)]
    [Header("[ Main Board Scroll ]")]
    public RectTransform mainBoardScrollPos;

    public void CreateAllContents()
    {
        API.instance.Request_UrgentNotice().Forget();
    }

    public void CreateNews()
    {
        // Create Event News
        noticeUI.TryAddContents(eventNewsCount);
    }
    #endregion
}

[System.Serializable]
public class UrgentNoticeCheckData
{
    public string year;
    public string month;
    public string day;
}
