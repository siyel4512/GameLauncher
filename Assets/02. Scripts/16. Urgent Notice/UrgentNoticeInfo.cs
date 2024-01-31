using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

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
        title_text.text = title;
        content_text.text = content;
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
