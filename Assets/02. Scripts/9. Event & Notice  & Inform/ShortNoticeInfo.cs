using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.UI;

public class ShortNoticeInfo : MonoBehaviour
{
    [Header("[ Info Values ]")]
    public string title;
    public string content;
    public string linkURL;

    public string Day;
    public string Year_Month;

    public string Year_Month_Day;

    [Space(10)]
    [Header("[ UI ]")]
    public TMP_Text title_text;
    public TMP_Text content_text;

    public TMP_Text Day_text;
    public TMP_Text Year_Month_text;

    public TMP_Text Year_Month_Day_text;

    [Space(10)]
    [Header("[ Event ]")]
    //public EventTrigger.Entry clickEvent;
    public Button clickEvent;

    // Start is called before the first frame update
    void Start()
    {
        SetEvent();
    }

    // set contents
    public void SetContents(int _noticeNumm, string _title, string _content, string _linkURL)
    {
        switch (_noticeNumm)
        {
            case 0:
                // mian notice
                // save values
                title = _title;
                content = _content;
                linkURL = _linkURL;

                // set values
                title_text.text = title;
                content_text.text = content;
                Day_text.text = "20";
                Year_Month_text.text = "23.08";
                break;
            case 1:
                // sub notice 1
                // save values
                title = _title;
                content = _content;
                linkURL = _linkURL;

                // set values
                title_text.text = title;
                Year_Month_Day_text.text = "2023-08-10";
                break;
            case 2:
                // sub notice 2
                // save values
                title = _title;
                content = _content;
                linkURL = _linkURL;

                // set values
                title_text.text = title;
                Year_Month_Day_text.text = "2023-08-01";
                break;
        }
    }

    public void ResetInfo(int _noticeNumm)
    {
        switch (_noticeNumm)
        {
            case 0:
                // mian notice
                // save values
                title = "";
                content = "";
                linkURL = "";

                // set values
                title_text.text = title;
                Year_Month_Day_text.text = content;
                Day_text.text = "";
                Year_Month_text.text = "";
                break;
            case 1:
                // sub notice 1
                // save values
                title = "";
                content = "";
                linkURL = "";

                // set values
                title_text.text = title;
                Year_Month_Day_text.text = "";
                break;
            case 2:
                // sub notice 2
                // save values
                title = "";
                content = "";
                linkURL = "";

                // set values
                title_text.text = title;
                Year_Month_Day_text.text = "";
                break;
        }
    }

    // open url
    public void BTN_OpenURL()
    {
        Application.OpenURL(linkURL);
    }

    // click event to eventtrigger
    public void SetEvent()
    {
        //clickEvent = new EventTrigger.Entry();
        //clickEvent.eventID = EventTriggerType.PointerClick;
        //clickEvent.callback.AddListener((eventData) => BTN_OpenURL());

        //GetComponent<EventTrigger>().triggers.Add(clickEvent);

        clickEvent = GetComponent<Button>();
        clickEvent.onClick.AddListener(BTN_OpenURL);
    }
}
