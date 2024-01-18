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

    [Space(10)]
    [Header("[ Main Board Scroll ]")]
    public RectTransform mainBoardScrollPos;

    [Space(10)]
    [Header("[ Guide Link ]")]
    public string[] launcherUserGuideLinks; // 0:korean, 1:english
    public string[] UGCInstallMenualLinks; // 0:korean, 1:english

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

            //// Todo : link 가져오기 테스트 완료 후 삭제 예정
            //if (!DEV.instance.isLinkLoadTEST)
            //{
            //    // Set Guide URL
            //    // test server
            //    if (DEV.instance.isUsingTestServer)
            //    {
            //        // korean
            //        guideInfo[0].SetLinkURL(DEV.instance.CDN_Test + launcherUserGuideLinks[0]);
            //        guideInfo[1].SetLinkURL(DEV.instance.CDN_Test + UGCInstallMenualLinks[0]);

            //        // english
            //        guideInfo[2].SetLinkURL(DEV.instance.CDN_Test + launcherUserGuideLinks[1]);
            //        guideInfo[3].SetLinkURL(DEV.instance.CDN_Test + UGCInstallMenualLinks[1]);
            //    }
            //    // live server
            //    else
            //    {
            //        // korean
            //        guideInfo[0].SetLinkURL(DEV.instance.CDN_Live + launcherUserGuideLinks[0]);
            //        guideInfo[1].SetLinkURL(DEV.instance.CDN_Live + UGCInstallMenualLinks[0]);

            //        // english
            //        guideInfo[2].SetLinkURL(DEV.instance.CDN_Live + launcherUserGuideLinks[1]);
            //        guideInfo[3].SetLinkURL(DEV.instance.CDN_Live + UGCInstallMenualLinks[1]);
            //    }
            //}
            //else
            //{
            //    // Set Guide URL
            //    // korean
            //    guideInfo[0].SetLinkURL(await API.instance.Request_LauncherUserGuideLink("ko"));
            //    guideInfo[1].SetLinkURL(await API.instance.Request_UGCInstallMenualLink("ko"));

            //    // english
            //    guideInfo[2].SetLinkURL(await API.instance.Request_LauncherUserGuideLink("en"));
            //    guideInfo[3].SetLinkURL(await API.instance.Request_UGCInstallMenualLink("en"));
            //}
        }
        else
        {
            // Create Event Banner
            GameManager.instance.api.Request_MainBoard(0).Forget();

            // Create Notice
            GameManager.instance.api.Request_MainBoard(1).Forget();

            // Create Curiverse Notice
            GameManager.instance.api.Request_MainBoard(2).Forget();

            //// Set Guide ULR
            //GameManager.instance.api.Request_MainBoard(3).Forget();
        }
    }

    public async void LoadGuideLink()
    {
        if (DEV.instance.isTEST_Contents)
        {
            // Todo : link 가져오기 테스트 완료 후 삭제 예정
            if (!DEV.instance.isLinkLoadTEST)
            {
                // Set Guide URL
                // test server
                if (DEV.instance.isUsingTestServer)
                {
                    // korean
                    guideInfo[0].SetLinkURL(DEV.instance.CDN_Test + launcherUserGuideLinks[0]);
                    guideInfo[1].SetLinkURL(DEV.instance.CDN_Test + UGCInstallMenualLinks[0]);

                    // english
                    guideInfo[2].SetLinkURL(DEV.instance.CDN_Test + launcherUserGuideLinks[1]);
                    guideInfo[3].SetLinkURL(DEV.instance.CDN_Test + UGCInstallMenualLinks[1]);
                }
                // live server
                else
                {
                    // korean
                    guideInfo[0].SetLinkURL(DEV.instance.CDN_Live + launcherUserGuideLinks[0]);
                    guideInfo[1].SetLinkURL(DEV.instance.CDN_Live + UGCInstallMenualLinks[0]);

                    // english
                    guideInfo[2].SetLinkURL(DEV.instance.CDN_Live + launcherUserGuideLinks[1]);
                    guideInfo[3].SetLinkURL(DEV.instance.CDN_Live + UGCInstallMenualLinks[1]);
                }
            }
            else
            {
                // Set Guide URL
                // korean
                guideInfo[0].SetLinkURL(await API.instance.Request_LauncherUserGuideLink("ko"));
                guideInfo[1].SetLinkURL(await API.instance.Request_UGCInstallMenualLink("ko"));

                // english
                guideInfo[2].SetLinkURL(await API.instance.Request_LauncherUserGuideLink("en"));
                guideInfo[3].SetLinkURL(await API.instance.Request_UGCInstallMenualLink("en"));
            }
        }
        else
        {
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

    public async void SetGuideDowloadLink()
    {
        // Todo : link 가져오기 테스트 완료 후 삭제 예정
        if (!DEV.instance.isLinkLoadTEST)
        {
            // test server
            if (DEV.instance.isUsingTestServer)
            {
                // korean
                guideInfo[0].SetLinkURL(DEV.instance.CDN_Test + launcherUserGuideLinks[0]);
                guideInfo[1].SetLinkURL(DEV.instance.CDN_Test + UGCInstallMenualLinks[0]);

                // english
                guideInfo[2].SetLinkURL(DEV.instance.CDN_Test + launcherUserGuideLinks[1]);
                guideInfo[3].SetLinkURL(DEV.instance.CDN_Test + UGCInstallMenualLinks[1]);
            }
            // live server
            else
            {
                // korean
                guideInfo[0].SetLinkURL(DEV.instance.CDN_Live + launcherUserGuideLinks[0]);
                guideInfo[1].SetLinkURL(DEV.instance.CDN_Live + UGCInstallMenualLinks[0]);

                // english
                guideInfo[2].SetLinkURL(DEV.instance.CDN_Live + launcherUserGuideLinks[1]);
                guideInfo[3].SetLinkURL(DEV.instance.CDN_Live + UGCInstallMenualLinks[1]);
            }
        }
        else
        {
            Debug.Log("가이드 링크 호출");

            // korean
            guideInfo[0].SetLinkURL(await API.instance.Request_LauncherUserGuideLink("ko"));
            guideInfo[1].SetLinkURL(await API.instance.Request_UGCInstallMenualLink("ko"));

            // english
            guideInfo[2].SetLinkURL(await API.instance.Request_LauncherUserGuideLink("en"));
            guideInfo[3].SetLinkURL(await API.instance.Request_UGCInstallMenualLink("en"));
        }
    }
}
