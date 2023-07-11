using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class BannerInfo : MonoBehaviour
{
    public string title;
    public string linkURL;

    public BannerUI bannerUI;

    public TMP_Text title_text;

    public EventTrigger.Entry clickEvent;

    // Start is called before the first frame update
    void Start()
    {
        SetEvent();
    }

    // set contents
    public void SetContents(string _title, string _linkURL)
    {
        title = _title;
        linkURL = _linkURL;

        title_text.text = title;
    }

    // open url
    public void BTN_OpenURL()
    {
        Application.OpenURL(linkURL);

        if (bannerUI.isTimer)
        {
            bannerUI.timer.Stop();
            bannerUI.timer.Start();
        }
        else
        {
            bannerUI.sw.Restart();
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
