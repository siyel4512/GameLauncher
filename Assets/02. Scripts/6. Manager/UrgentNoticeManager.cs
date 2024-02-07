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
        // 오늘하루 안봄
        if (viewAgainToggle.isOn)
        {
            SaveCheckData();
        }
        
        urgentNoticePopup.SetActive(false); // 긴급공지 팝업창 닫기
        viewAgainToggle.isOn = false; // 체크 해제
    }
    #endregion

    #region Json file Read and Write
    public bool CheckData()
    {
        bool isShowPopup;
        viewAgainToggle.isOn = false; // 다시보지않기 체크 해제
        UrgentNoticeCheckData loadData = LoadCheckData(); // 런처가 저장하고 있는 날짜 데이터 로드
        DateTime currentTime = DateTime.Now; // 현재 날짜 데이터 가져오기

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

    // 팝업창 표시 유무 체크용 날짜 저장
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

    // 팝업창 표시 유무 체크용 날짜 로드
    public UrgentNoticeCheckData LoadCheckData()
    {
        string path = Path.Combine(Application.streamingAssetsPath + "/UrgentNotice", "UrgentNotice.json");
        string jsonData = File.ReadAllText(path);
        checkData = JsonUtility.FromJson<UrgentNoticeCheckData>(jsonData);

        return checkData;
    }

    // 팝업창 표시 유무 체크용 날짜 리셋
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
        // 긴급공지 테이터 요청
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
