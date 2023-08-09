using System.Collections.Generic;
using UnityEngine;

public class ShortNotice : MonoBehaviour
{
    public ShortNoticeInfo[] shortNoticeInfos;
    public GameObject warningText;

    // Start is called before the first frame update
    void Start()
    {
        warningText.SetActive(false);

        for (int i = 0; i < shortNoticeInfos.Length; i++)
        {
            shortNoticeInfos[i].gameObject.SetActive(false);
        }
    }

    public void SetContents(int noticeCount)
    {
        int count = 0;

        if (noticeCount == 0)
        {
            warningText.SetActive(true);
            return;
        }
        else
        {
            warningText.SetActive(false);
        }

        if (noticeCount > 3)
        {
            count = 3;
        }
        else
        {
            count = noticeCount;
        }

        Debug.Log(shortNoticeInfos[0]);

        Debug.Log("[SY] count : " + count);
        
        List<SaveData.mainBoard> shortNoticeValues = GameManager.instance.jsonData.shortNotice_List;

        for (int i = 0; i < count; i++)
        {
            Debug.Log("[SY] i : " + i);
            shortNoticeInfos[i].gameObject.SetActive(true);
            //shortNoticeInfos[i].SetContents(i, $"공지 사항 {i + 1}", "공지사항 세부 내용 일부 공지사항 세부 내용 일부공지사항 세부 내용 일부공지사항 세부 내용 일부공지사항 세부 내용 일부공지사항 세부 내용 일부공지사항 세부 내용 일부공지사항 세부 내용 일부공지사항 세부 내용 일부공지사항 세부 내용 일부공지사항 세부 내용 일부", "https://www.youtube.com/");

            // set data
            shortNoticeInfos[i].boardNum = shortNoticeValues[i].boardNum;
            shortNoticeInfos[i].writer = shortNoticeValues[i].writer;
            shortNoticeInfos[i].title = shortNoticeValues[i].title;
            shortNoticeInfos[i].content = shortNoticeValues[i].content;
            shortNoticeInfos[i].webImg = shortNoticeValues[i].webImg;
            shortNoticeInfos[i].lnchrImg = shortNoticeValues[i].lnchrImg;
            shortNoticeInfos[i].boardType = shortNoticeValues[i].boardType;
            shortNoticeInfos[i].openYn = shortNoticeValues[i].openYn;
            shortNoticeInfos[i].exprPeriod = shortNoticeValues[i].exprPeriod;
            shortNoticeInfos[i].regDt = shortNoticeValues[i].regDt;
            shortNoticeInfos[i].upDt = shortNoticeValues[i].upDt;
            shortNoticeInfos[i].linkURL = "https://www.youtube.com/";

            shortNoticeInfos[i].SetContents(i);
        }
    }

    public void ResetShortNoticeInfo()
    {
        warningText.SetActive(false);

        for (int i = 0;i < shortNoticeInfos.Length; i++)
        {
            shortNoticeInfos[i].ResetInfo(i);
        }
    }
}
