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
    #region Default Setting
    [Header("[ Default Component ]")]
    public UrgentNoticeUI urgentNoticeUI;
    public UrgentNoticeCheckData checkData;

    [Space(10)]
    [Header("[ Popup UI ]")]
    public GameObject urgentNoticePopup;
    public Toggle viewAgainToggle;
    public Button closeButton;

    public void BTN_Close()
    {
        // �����Ϸ� �Ⱥ�
        if (viewAgainToggle.isOn)
        {
            SaveCheckData();
        }
        
        urgentNoticePopup.SetActive(false); // ��ް��� �˾�â �ݱ�
        viewAgainToggle.isOn = false; // üũ ����
    }
    #endregion

    #region Json file Read and Write
    public bool CheckData()
    {
        bool isShowPopup;
        viewAgainToggle.isOn = false; // �ٽú����ʱ� üũ ����
        UrgentNoticeCheckData loadData = LoadCheckData(); // ��ó�� �����ϰ� �ִ� ��¥ ������ �ε�
        DateTime currentTime = DateTime.Now; // ���� ��¥ ������ ��������

        // hide urgent notice popup
        if (loadData.year == currentTime.Year.ToString()
            && loadData.month == currentTime.Month.ToString()
            && loadData.day == currentTime.DayOfYear.ToString())
        {
            isShowPopup = false;
        }
        // show urgent notice popup
        else
        {
            isShowPopup = true;
        }

        return isShowPopup;
    }

    // �˾�â ǥ�� ���� üũ�� ��¥ ����
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

    // �˾�â ǥ�� ���� üũ�� ��¥ �ε�
    public UrgentNoticeCheckData LoadCheckData()
    {
        string path = Path.Combine(Application.streamingAssetsPath + "/UrgentNotice", "UrgentNotice.json");
        string jsonData = File.ReadAllText(path);
        checkData = JsonUtility.FromJson<UrgentNoticeCheckData>(jsonData);

        return checkData;
    }

    // �˾�â ǥ�� ���� üũ�� ��¥ ����
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

    #region Notice Contents
    [Space(10)]
    [Header("[ Notice UI ]")]
    public UrgentNoticeUI noticeUI;

    [Space(10)]
    [Header("[ Contents Count ]")]
    public int urgentNoticeCount;

    [Space(10)]
    [Header("[ Main Board Scroll ]")]
    public RectTransform mainBoardScrollPos;

    public void CreateAllContents()
    {
        // ��ް��� ������ ��û
        API.instance.Request_UrgentNotice().Forget();
    }

    public void CreateUrgentNotice()
    {
        // Create Event News
        noticeUI.TryAddContents(urgentNoticeCount);
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
