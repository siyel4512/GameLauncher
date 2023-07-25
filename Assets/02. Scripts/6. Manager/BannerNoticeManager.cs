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
        if (DEV.instance.isTEST_CONTENTS)
        {
            // Create Event Banner
            bannerUI.TryAddContents(eventBannerCount);

            // Create Notice
            noticeUIs[0].TryAddContents(noticeCount);

            // Create Curiverse Notice
            noticeUIs[1].TryAddContents(curiverseNoticeCount);
        }
        else
        {
            // Create Event Banner
            GameManager.instance.api.Request_EventBanner().Forget();

            // Create Notice
            GameManager.instance.api.Request_Notice().Forget();

            // Create Curiverse Notice
            GameManager.instance.api.Request_CuriverseNotice().Forget();
        }
    }

    public void SetGuideDownloadLink()
    {
        if (DEV.instance.isTEST_CONTENTS)
        {
            // curiverse using guide download
            guideInfo[0].SetLinkURL("https://www.google.com/");

            // make/batch editor guide download
            guideInfo[1].SetLinkURL("https://www.google.com/");
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
