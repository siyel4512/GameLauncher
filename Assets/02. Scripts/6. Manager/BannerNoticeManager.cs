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

    [Space(10)]
    public string[] launcherUserGuideLinks;
    public string[] UGCInstallMenualLinks;

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
                // korean
                if (GameManager.instance.languageManager.currentLanguageNum == 1)
                {
                    guideInfo[0].SetLinkURL(DEV.instance.CDN_Test + launcherUserGuideLinks[0]);
                    guideInfo[1].SetLinkURL(DEV.instance.CDN_Test + UGCInstallMenualLinks[0]);
                }
                // english
                else
                {
                    guideInfo[0].SetLinkURL(DEV.instance.CDN_Test + launcherUserGuideLinks[1]);
                    guideInfo[1].SetLinkURL(DEV.instance.CDN_Test + UGCInstallMenualLinks[1]);
                }
            }
            // live server
            else
            {
                // korean
                if (GameManager.instance.languageManager.currentLanguageNum == 1)
                {
                    guideInfo[0].SetLinkURL(DEV.instance.CDN_Live + launcherUserGuideLinks[0]);
                    guideInfo[1].SetLinkURL(DEV.instance.CDN_Live + UGCInstallMenualLinks[0]);
                }
                // english
                else
                {
                    guideInfo[0].SetLinkURL(DEV.instance.CDN_Live + launcherUserGuideLinks[1]);
                    guideInfo[1].SetLinkURL(DEV.instance.CDN_Live + UGCInstallMenualLinks[1]);
                }
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
            // korean
            if (GameManager.instance.languageManager.currentLanguageNum == 1)
            {
                guideInfo[0].SetLinkURL(DEV.instance.CDN_Test + launcherUserGuideLinks[0]);
                guideInfo[1].SetLinkURL(DEV.instance.CDN_Test + UGCInstallMenualLinks[0]);
            }
            // english
            else
            {
                guideInfo[0].SetLinkURL(DEV.instance.CDN_Test + launcherUserGuideLinks[1]);
                guideInfo[1].SetLinkURL(DEV.instance.CDN_Test + UGCInstallMenualLinks[1]);
            }
        }
        // live server
        else
        {
            // korean
            if (GameManager.instance.languageManager.currentLanguageNum == 1)
            {
                guideInfo[0].SetLinkURL(DEV.instance.CDN_Live + launcherUserGuideLinks[0]);
                guideInfo[1].SetLinkURL(DEV.instance.CDN_Live + UGCInstallMenualLinks[0]);
            }
            // english
            else
            {
                guideInfo[0].SetLinkURL(DEV.instance.CDN_Live + launcherUserGuideLinks[1]);
                guideInfo[1].SetLinkURL(DEV.instance.CDN_Live + UGCInstallMenualLinks[1]);
            }
        }
    }
}
