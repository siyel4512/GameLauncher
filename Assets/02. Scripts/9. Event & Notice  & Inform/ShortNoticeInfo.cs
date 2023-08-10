using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.UI;

public class ShortNoticeInfo : MonoBehaviour
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
    //public void SetContents(int _noticeNumm, string _title, string _content, string _linkURL)
    public void SetContents(int _noticeNumm)
    {
        switch (_noticeNumm)
        {
            // mian notice
            case 0:
                // set values
                title_text.text = title;
                content_text.text = content;

                // split date
                string[] temp_main_date1 = upDt.Split(" ");
                string[] temp_main_date2 = temp_main_date1[0].Split("-");

                Day_text.text = temp_main_date2[2];
                Year_Month_text.text = $"{temp_main_date2[0].Substring(2,2)}.{temp_main_date2[1]}";
                break;
            // sub notice 1
            case 1:
                // set values
                title_text.text = title;

                // split date
                string[] temp_sub1_date = upDt.Split(" ");
                Year_Month_Day_text.text = temp_sub1_date[0];
                break;
            // sub notice 2
            case 2:
                // set values
                title_text.text = title;

                // split date
                string[] temp_sub2_date = upDt.Split(" ");
                Year_Month_Day_text.text = temp_sub2_date[0];
                break;
        }
    }

    public void ResetInfo(int _noticeNumm)
    {
        switch (_noticeNumm)
        {
            // mian notice
            case 0:
                // save values
                title = "";
                content = "";
                linkURL = "";

                // set values
                title_text.text = title;
                content_text.text = content;
                Day_text.text = "";
                Year_Month_text.text = "";
                break;
            // sub notice 1
            case 1:
                // save values
                title = "";
                content = "";
                linkURL = "";

                // set values
                title_text.text = title;
                Year_Month_Day_text.text = "";
                break;
            // sub notice 2
            case 2:
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
        clickEvent = GetComponent<Button>();
        clickEvent.onClick.AddListener(BTN_OpenURL);
    }
}
