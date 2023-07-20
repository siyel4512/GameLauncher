using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BannerNoticeManager : MonoBehaviour
{
    [Header("[ UI ]")]
    public BannerUI bannerUI;
    public NoticeUI[] noticeUIs;
    public GuideInfo[] guideInfo;

    [Space(10)]
    [Header("[ Contents Count ]")]
    public int eventBannerCount;
    public int noticeCount;
    public int curiverseNoticeCount;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void CreateAllContents()
    {
        // Create Event Banner
        bannerUI.TryAddContents(eventBannerCount);
        GameManager.instance.api.Request_EventBanner().Forget();

        // Create Notice
        noticeUIs[0].TryAddContents(noticeCount);
        GameManager.instance.api.Request_Notice().Forget();

        // Create Curiverse Notice
        noticeUIs[1].TryAddContents(curiverseNoticeCount);
        GameManager.instance.api.Request_CuriverseNotice().Forget();
    }

    public void SetGuideDownloadLink()
    {
        // curiverse using guide download
        guideInfo[0].SetLinkURL("https://www.google.com/");
        GameManager.instance.api.Request_GuideDownload1().Forget();

        // make/batch editor guide download
        guideInfo[1].SetLinkURL("https://www.google.com/");
        GameManager.instance.api.Request_GuideDownload1().Forget();
    }
}
