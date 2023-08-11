using UnityEngine;

public class BannerNoticeManager : MonoBehaviour
{
    [Header("[ UI ]")]
    public BannerUI bannerUI;
    public NoticeUI noticeUI;
    public ShortNotice shortNotice;
    public GuideInfo[] guideInfo;

    [Space(10)]
    [Header("[ Contents Count ]")]
    public int eventBannerCount;
    public int shortNoticeCount;
    public int eventNewsCount;
    public int guideCount;

    public RectTransform mainBoardScrollPos;

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
            noticeUI.TryAddContents(eventNewsCount);

            // Set Guide URL
            guideInfo[0].SetLinkURL("https://launcherdownload1.s3.ap-northeast-2.amazonaws.com/2020+%EB%8C%80%EC%A0%84%EB%A7%88%EC%BC%80%ED%8C%85%EA%B3%B5%EC%82%AC+%ED%99%8D%EB%B3%B4%EC%9E%90%EB%A3%8C.pdf");
            guideInfo[1].SetLinkURL("https://launcherdownload1.s3.ap-northeast-2.amazonaws.com/2021%EB%85%84+%EB%8C%80%EC%A0%84%EB%A7%88%EC%BC%80%ED%8C%85%EA%B3%B5%EC%82%AC.pdf");
        }
        else
        {
            //// Create Event Banner
            //GameManager.instance.api.Request_EventBanner().Forget();
            GameManager.instance.api.Request_MainBoard(0).Forget();

            //// Create Notice
            //GameManager.instance.api.Request_Notice().Forget();
            GameManager.instance.api.Request_MainBoard(1).Forget();

            //// Create Curiverse Notice
            //GameManager.instance.api.Request_EventNews().Forget();
            GameManager.instance.api.Request_MainBoard(2).Forget();

            // Set Guide ULR
            GameManager.instance.api.Request_MainBoard(3).Forget();
        }
    }

    public void CreateEventBanner()
    {
        // Create Event Banner
        bannerUI.TryAddContents(eventBannerCount);
    }

    public void SetNotice()
    {
        // Create Notice
        shortNotice.SetContents(shortNoticeCount);
    }

    public void CreateNews()
    {
        // Create Event News
        noticeUI.TryAddContents(eventNewsCount);
    }

    public void SetGuideDowloadLink()
    {
        guideInfo[0].SetLinkURL(GameManager.instance.jsonData.guide_List[0].lnchrImg);
        guideInfo[1].SetLinkURL(GameManager.instance.jsonData.guide_List[1].lnchrImg);
    }

    //public void Test_SetGuideDownloadLink()
    //{
    //    if (DEV.instance.isTEST_Contents)
    //    {
    //        // curiverse using guide download
    //        //guideInfo[0].SetLinkURL("https://www.google.com/");
    //        guideInfo[0].SetLinkURL("https://launcherdownload1.s3.ap-northeast-2.amazonaws.com/Test+PDF.pdf");

    //        // make/batch editor guide download
    //        //guideInfo[1].SetLinkURL("https://www.google.com/");
    //        guideInfo[1].SetLinkURL("https://launcherdownload1.s3.ap-northeast-2.amazonaws.com/Test+PDF.pdf");
    //    }
    //    else
    //    {
    //        // curiverse using guide download
    //        GameManager.instance.api.Request_GuideDownload1().Forget();

    //        // make/batch editor guide download
    //        GameManager.instance.api.Request_GuideDownload2().Forget();
    //    }
    //}
}
