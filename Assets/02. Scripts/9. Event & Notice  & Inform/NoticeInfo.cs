using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class NoticeInfo : MonoBehaviour
{
    [Header("[ Info Values ]")]
    public string title;
    public string content;
    public string linkURL;
    private List<NoticeInfo> spawnedContents;

    [Space(10)]
    [Header("[ UI ]")]
    public NoticeUI noticeUI;
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
    public void SetContents(string _title, string _content, string _linkURL)
    {
        title = _title;
        content = _content;
        linkURL = _linkURL;

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
