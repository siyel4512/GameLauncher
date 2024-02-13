using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using UnityEditor.Build.Pipeline.Utilities;
using Unity.VisualScripting;
using System;

public class UrgentNoticeInfo : MonoBehaviour
{
    [Header("[ Info Values ]")]
    public int boardNum;
    public string writer;
    public string title;
    public string content;
    public string webImg;
    public string lnchrImg;
    public string boardType;
    public string openYn;
    public string exprPeriod;
    public string regDt;
    public string upDt;

    public string linkURL;

    private List<UrgentNoticeInfo> spawnedContents;

    [Space(10)]
    [Header("[ UI ]")]
    public UrgentNoticeUI noticeUI;
    public TMP_Text title_text;
    public TMP_Text content_text;
    public float tileFontSize = 30f;

    [Space(10)]
    [Header("[ Event ]")]
    public EventTrigger.Entry clickEvent;

    // Start is called before the first frame update
    void Start()
    {
        SetEvent();
    }

    // set contents
    public void SetContents()
    {
        content_text.text = $"<size={tileFontSize}><align=center>{title}</align></size>\n\n{content}";

        //// �ٹٲ� �±� �׽�Ʈ
        //string origin_text = $"���� �׽�Ʈ �Դϴ�_1<br />���� �׽�Ʈ �Դϴ�_2<br/>���� �׽�Ʈ �Դϴ�_3<br>���� �׽�Ʈ �Դϴ�_4</br>";
        //string temp_text1 = origin_text.Replace("<br>", "\n"); // ���� ������ �ʿ��ҽ� ����Ұ�
        //string temp_text2 = temp_text1.Replace("<br/>", "\n"); // ���� ������ �ʿ��ҽ� ����Ұ�
        //string temp_text3 = temp_text2.Replace("<br />", "\n"); // ���� ������ �ʿ��ҽ� ����Ұ�
        //string temp_text4 = temp_text3.Replace("</br>", "\n"); // ���� ������ �ʿ��ҽ� ����Ұ�
        //Debug.Log("�±� �׽�Ʈ : " + temp_text4);
        //content_text.text = temp_text4;
    }

    // open url
    public void BTN_OpenURL()
    {
        Application.OpenURL(linkURL);

        if (noticeUI.isTimer)
        {
            noticeUI.timer.Stop();
            noticeUI.timer.Start();
        }
        else
        {
            noticeUI.sw.Restart();
        }
    }

    // click event to eventtrigger
    public void SetEvent()
    {
        clickEvent = new EventTrigger.Entry();
        clickEvent.eventID = EventTriggerType.PointerClick;
        clickEvent.callback.AddListener((eventData) => BTN_OpenURL());

        GetComponent<EventTrigger>().triggers.Add(clickEvent);
    }
}
