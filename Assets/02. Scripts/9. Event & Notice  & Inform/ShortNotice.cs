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

        for (int i = 0; i < count; i++)
        {
            Debug.Log("[SY] i : " + i);
            shortNoticeInfos[i].gameObject.SetActive(true);
            shortNoticeInfos[i].SetContents(i, $"공지 사항 {i + 1}", "공지사항 세부 내용 일부 공지사항 세부 내용 일부공지사항 세부 내용 일부공지사항 세부 내용 일부공지사항 세부 내용 일부공지사항 세부 내용 일부공지사항 세부 내용 일부공지사항 세부 내용 일부공지사항 세부 내용 일부공지사항 세부 내용 일부공지사항 세부 내용 일부", "https://www.youtube.com/");
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
