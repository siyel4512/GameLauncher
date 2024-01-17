using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class URL : MonoBehaviour
{
    // server
    private string baseServer;
    public string BaseServer => baseServer;

    // login
    private string getKeyURL;
    private string tryLoginURL;
    public string GetKeyURL => getKeyURL;
    public string TryLoginURL => tryLoginURL;

    // event banner & notice
    protected string playerStateUpdateURL;

    // friend list
    protected string friendListURL;
    protected string addFriendURL;
    protected string requestAcceptURL;
    protected string requestRefuseNDeleteURL;
    protected string searchUserWithNicknameURL;

    protected string mainBoardURP;

    // file download
    protected string fileDownloadURL;
    protected string fileDownloadURL_Live;

    // abnormal shutdown
    protected string abnormalShutdownURL;

    // asset bundle download
    public string myBundleListURL;
    public string downloadBundleURL;

    // launcher version check
    protected string launcherVersionCheckURL;

    // launcher download
    public string launcherDownloadURL;

    // user guide link
    protected string launcherUserGuideURL;
    protected string ugcInstallMenualURL;

    // english video link
    protected string videoEnURL;

    #region API
    // using server
    private string testServer = "https://metaplytest.co.kr/";
    private string liveServer = "https://metaply.go.kr/";

    // login
    private string requestKey_URL = "onlineScienceMuseumAPI/checkId.do";
    private string tryLogin_URL = "onlineScienceMuseumAPI/tryLogin.do";

    // player state
    private string playerStateUpdate_URL = "onlineScienceMuseumAPI/changeMyStatus.do";

    // event banner & notice
    private string mainBoard_URP = "onlineScienceMuseumAPI/mainBoard.do";

    // friend list
    private string friendList_URL = "onlineScienceMuseumAPI/frndInfo.do";
    private string addFriend_URL = "onlineScienceMuseumAPI/insertFrndInfo.do";
    private string requestAccept_URL = "onlineScienceMuseumAPI/updateFrndReqAccept.do";
    private string requestRefuseNDelete_URL = "onlineScienceMuseumAPI/deleteFrndReq.do";
    private string searchUserWithNickname_URL = "onlineScienceMuseumAPI/searchUserWithId.do";

    // file download
    private string fileDownload_URL = "onlineScienceMuseumAPI/downloadBuildFile.do";

    // asset bundle download
    private string myBundleList_URL = "onlineScienceMuseumAPI/callDownloadAssetList.do";
    private string downloadBundle_URL = "onlineScienceMuseumAPI/downloadAssetBundleFile.do";

    // abnormal shutdown
    private string abnormalShutdown_URL = "onlineScienceMuseumAPI/insertErrLog.do";

    // launcher version check
    //https://metaplytest.co.kr/onlineScienceMuseumAPI/getCurrentVersionOfLauncher.do
    private string launcherVersionCheck_URL = "onlineScienceMuseumAPI/getCurrentVersionOfLauncher.do";

    //launcher download
    private string launcherDownload_URL = "onlineScienceMuseumAPI/downloadInstallFile.do";

    // user guide link
    private string launcherUserGuide_URL = "onlineScienceMuseumAPI/getLauncherGuide.do";
    private string ugcInstallMenual_URL = "onlineScienceMuseumAPI/getToolGuide.do";

    // english video link
    private string video_En_URL = "onlineScienceMuseumAPI/getVideo.do";

    #endregion

    // set URL
    public void SetURL()
    {
        // test server
        if (DEV.instance.isUsingTestServer)
        {
            // server
            baseServer = testServer;
            
            // login
            if (DEV.instance.isLoginToTestServer)
            {
                // test server
                getKeyURL = testServer + requestKey_URL;
                tryLoginURL = testServer + tryLogin_URL;
            }
            else
            {
                // live server
                getKeyURL = liveServer + requestKey_URL;
                tryLoginURL = liveServer + tryLogin_URL;
            }

            // player state
            playerStateUpdateURL = testServer + playerStateUpdate_URL;

            // friend list
            friendListURL = testServer + friendList_URL;
            addFriendURL = testServer + addFriend_URL;
            requestAcceptURL = testServer + requestAccept_URL;
            requestRefuseNDeleteURL = testServer + requestRefuseNDelete_URL;
            searchUserWithNicknameURL = testServer + searchUserWithNickname_URL;

            // main board
            mainBoardURP = testServer + mainBoard_URP;

            // file download
            fileDownloadURL = testServer + fileDownload_URL;
            fileDownloadURL_Live = liveServer + fileDownload_URL;

            // asset bundle download
            myBundleListURL = testServer + myBundleList_URL;
            downloadBundleURL = testServer + downloadBundle_URL;

            // abnormal shutdown
            abnormalShutdownURL = testServer + abnormalShutdown_URL;

            // launcher version check
            launcherVersionCheckURL = testServer + launcherVersionCheck_URL;

            // launcher download
            launcherDownloadURL = testServer + launcherDownload_URL;

            // user guide link
            launcherUserGuideURL = testServer + launcherUserGuide_URL;
            ugcInstallMenualURL = testServer + ugcInstallMenual_URL;

            // english video link
            videoEnURL = testServer + video_En_URL;
        }
        // live server
        else
        {
            // server
            baseServer = liveServer;

            // login
            if (DEV.instance.isLoginToTestServer)
            {
                // test server
                getKeyURL = testServer + requestKey_URL;
                tryLoginURL = testServer + tryLogin_URL;
            }
            else
            {
                // live server
                getKeyURL = liveServer + requestKey_URL;
                tryLoginURL = liveServer + tryLogin_URL;
            }

            // player state
            playerStateUpdateURL = liveServer + playerStateUpdate_URL;

            // friend list
            friendListURL = liveServer + friendList_URL;
            addFriendURL = liveServer + addFriend_URL;
            requestAcceptURL = liveServer + requestAccept_URL;
            requestRefuseNDeleteURL = liveServer + requestRefuseNDelete_URL;
            searchUserWithNicknameURL = liveServer + searchUserWithNickname_URL;

            // main board
            mainBoardURP = liveServer + mainBoard_URP;

            // file download
            fileDownloadURL = testServer + fileDownload_URL;
            fileDownloadURL_Live = liveServer + fileDownload_URL;

            // asset bundle download
            myBundleListURL = liveServer + myBundleList_URL;
            downloadBundleURL = liveServer + downloadBundle_URL;

            // abnormal shutdown
            //abnormalShutdownURL = liveServer + abnormalShutdown_URL;
            abnormalShutdownURL = testServer + abnormalShutdown_URL;

            // launcher version check
            launcherVersionCheckURL = liveServer + launcherVersionCheck_URL;

            // launcher download
            launcherDownloadURL = liveServer + launcherDownload_URL;

            // user guide link
            launcherUserGuideURL = liveServer + launcherUserGuide_URL;
            ugcInstallMenualURL = liveServer + ugcInstallMenual_URL;

            // english video link
            videoEnURL = liveServer + video_En_URL;
        }
    }
}
