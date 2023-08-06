using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BannerNoticeManager : MonoBehaviour
{
    [Header("[ UI ]")]
    public BannerUI bannerUI;
    public NoticeUI noticeUIs;
    public ShortNotice shortNotice;
    public GuideInfo[] guideInfo;

    [Space(10)]
    [Header("[ Contents Count ]")]
    public int eventBannerCount;
    public int shortNoticeCount;
    public int eventNewsCount;

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
        if (DEV.instance.isTEST_Contents)
        {
            // Create Event Banner
            bannerUI.TryAddContents(eventBannerCount);

            // Create Notice
            //noticeUIs[0].TryAddContents(noticeCount);
            shortNotice.SetContents(shortNoticeCount);

            // Create Event News
            noticeUIs.TryAddContents(eventNewsCount);
        }
        else
        {
            // Create Event Banner
            GameManager.instance.api.Request_EventBanner().Forget();

            // Create Notice
            GameManager.instance.api.Request_Notice().Forget();

            // Create Curiverse Notice
            GameManager.instance.api.Request_EventNews().Forget();
        }
    }

    public void SetGuideDownloadLink()
    {
        if (DEV.instance.isTEST_Contents)
        {
            // curiverse using guide download
            //guideInfo[0].SetLinkURL("https://www.google.com/");
            guideInfo[0].SetLinkURL("https://launcherdownload1.s3.ap-northeast-2.amazonaws.com/Test+PDF.pdf");

            // make/batch editor guide download
            //guideInfo[1].SetLinkURL("https://www.google.com/");
            guideInfo[1].SetLinkURL("https://launcherdownload1.s3.ap-northeast-2.amazonaws.com/Test+PDF.pdf");
        }
        else
        {
            // curiverse using guide download
            GameManager.instance.api.Request_GuideDownload1().Forget();

            // make/batch editor guide download
            GameManager.instance.api.Request_GuideDownload2().Forget();
        }
    }
}
