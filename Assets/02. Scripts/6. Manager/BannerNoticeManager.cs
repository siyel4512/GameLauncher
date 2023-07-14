using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BannerNoticeManager : MonoBehaviour
{
    public BannerUI bannerUI;
    public NoticeUI[] noticeUIs;

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

        // Create Notice
        noticeUIs[0].TryAddContents(noticeCount);

        // Create Curiverse Notice
        noticeUIs[1].TryAddContents(curiverseNoticeCount);
    }
}
