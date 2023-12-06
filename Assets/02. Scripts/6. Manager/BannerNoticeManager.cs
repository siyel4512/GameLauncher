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
            shortNotice.SetContents(shortNoticeCount);

            // Create Event News
            noticeUI.TryAddContents(eventNewsCount);

            // Set Guide URL
            // test server
            if (DEV.instance.isUsingTestServer)
            {
                //guideInfo[0].SetLinkURL("http://fgnowlvzhshz17884402.cdn.ntruss.com/onlinemuseum/curiverseUserGuide/launcherUserGuideBook.pdf");
                //guideInfo[1].SetLinkURL("http://fgnowlvzhshz17884402.cdn.ntruss.com/onlinemuseum/ugcAuthoringToolGuide/UGC_install_manual.pdf");
                guideInfo[0].SetLinkURL("https://ejrdejzsaflk20717940.cdn.ntruss.com/onlinemuseum/curiverseUserGuide/launcherUserGuideBook.pdf");
                guideInfo[1].SetLinkURL("https://ejrdejzsaflk20717940.cdn.ntruss.com/onlinemuseum/ugcAuthoringToolGuide/UGC_install_manual.pdf");
            }
            // live server
            else
            {
                //guideInfo[0].SetLinkURL("http://yhbdymjatqnq20869625.cdn.ntruss.com/onlinemuseum/curiverseUserGuide/launcherUserGuideBook.pdf");
                //guideInfo[1].SetLinkURL("http://yhbdymjatqnq20869625.cdn.ntruss.com/onlinemuseum/ugcAuthoringToolGuide/UGC_install_manual.pdf");
                guideInfo[0].SetLinkURL("http://alilgjwknwlm18374611.cdn.ntruss.com/onlinemuseum/curiverseUserGuide/launcherUserGuideBook.pdf");
                guideInfo[1].SetLinkURL("http://alilgjwknwlm18374611.cdn.ntruss.com/onlinemuseum/ugcAuthoringToolGuide/UGC_install_manual.pdf");
            }
        }
        else
        {
            // Create Event Banner
            GameManager.instance.api.Request_MainBoard(0).Forget();

            // Create Notice
            GameManager.instance.api.Request_MainBoard(1).Forget();

            // Create Curiverse Notice
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
        //guideInfo[0].SetLinkURL(GameManager.instance.jsonData.guide_List[0].lnchrImg);
        //guideInfo[1].SetLinkURL(GameManager.instance.jsonData.guide_List[1].lnchrImg);

        // test server
        if (DEV.instance.isUsingTestServer)
        {
            //guideInfo[0].SetLinkURL("http://fgnowlvzhshz17884402.cdn.ntruss.com/onlinemuseum/curiverseUserGuide/launcherUserGuideBook.pdf");
            //guideInfo[1].SetLinkURL("http://fgnowlvzhshz17884402.cdn.ntruss.com/onlinemuseum/ugcAuthoringToolGuide/UGC_install_manual.pdf");
            guideInfo[0].SetLinkURL("https://ejrdejzsaflk20717940.cdn.ntruss.com/onlinemuseum/curiverseUserGuide/launcherUserGuideBook.pdf");
            guideInfo[1].SetLinkURL("https://ejrdejzsaflk20717940.cdn.ntruss.com/onlinemuseum/ugcAuthoringToolGuide/UGC_install_manual.pdf");
        }
        // live server
        else
        {
            // guideInfo[0].SetLinkURL("https://yhbdymjatqnq20869625.cdn.ntruss.com/onlinemuseum/curiverseUserGuide/launcherUserGuideBook.pdf");
            //guideInfo[1].SetLinkURL("https://yhbdymjatqnq20869625.cdn.ntruss.com/onlinemuseum/ugcAuthoringToolGuide/UGC_install_manual.pdf");
            guideInfo[0].SetLinkURL("http://alilgjwknwlm18374611.cdn.ntruss.com/onlinemuseum/curiverseUserGuide/launcherUserGuideBook.pdf");
            guideInfo[1].SetLinkURL("http://alilgjwknwlm18374611.cdn.ntruss.com/onlinemuseum/ugcAuthoringToolGuide/UGC_install_manual.pdf");
        }
    }
}
