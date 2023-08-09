using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class URL : LoadFile
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

    #region API
    // using server
    private string testServer = "http://101.101.218.135:5002/";
    private string liveServer = "http://49.50.162.141:5002/";

    // login
    private string requestKey_URL = "onlineScienceMuseumAPI/checkId.do";
    private string tryLogin_URL = "onlineScienceMuseumAPI/tryLogin.do";

    // player state
    private string playerStateUpdate_URL = "onlineScienceMuseumAPI/changeMyStatus.do";

    // event banner & notice
    private string mainBoard_URP = "onlineScienceMuseumAPI/mainBoard.do";

    // guide download

    // friend list
    private string friendList_URL = "onlineScienceMuseumAPI/frndInfo.do";
    private string addFriend_URL = "onlineScienceMuseumAPI/insertFrndInfo.do";
    private string requestAccept_URL = "onlineScienceMuseumAPI/updateFrndReqAccept.do";
    private string requestRefuseNDelete_URL = "onlineScienceMuseumAPI/deleteFrndReq.do";
    private string searchUserWithNickname_URL = "onlineScienceMuseumAPI/searchUserWithNickname.do";

    // file download
    private string fileDownload_URL = "onlineScienceMuseumAPI/downloadBuildFile.do";
    #endregion

    // set URL
    public void SetURL()
    {
        // test server
        if (DEV.instance.isTEST_Server)
        {
            // server
            baseServer = testServer;
            
            // login
            getKeyURL = testServer + requestKey_URL;
            tryLoginURL = tryLogin_URL;

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
        }
        // live server
        else
        {
            // server
            baseServer = liveServer;

            // login
            getKeyURL = liveServer + requestKey_URL;
            tryLoginURL = tryLogin_URL;

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
            fileDownloadURL = liveServer + fileDownload_URL;
            fileDownloadURL_Live = liveServer + fileDownload_URL;
        }
    }

    //======================== 삭제 예정 ========================//
    #region TEST API
    // login
    public string TEST_requesetKey = "http://101.101.218.135:5002/onlineScienceMuseumAPI/checkId.do";
    public string TEST_tryLogin = "http://101.101.218.135:5002/onlineScienceMuseumAPI/tryLogin.do";

    // friend list & request friend list
    public string TEST_friendList = "http://101.101.218.135:5002/onlineScienceMuseumAPI/frndInfo.do";
    public string TEST_addFriend = "http://101.101.218.135:5002/onlineScienceMuseumAPI/insertFrndInfo.do";
    public string TEST_requestAccept = "http://101.101.218.135:5002/onlineScienceMuseumAPI/updateFrndReqAccept.do";
    public string TEST_requestRefuseNDelete = "http://101.101.218.135:5002/onlineScienceMuseumAPI/deleteFrndReq.do";

    // event banner & notice

    // file download
    public string TEST_fileDownload = "http://101.101.218.135:5002/onlineScienceMuseumAPI/downloadBuildFile.do";
    #endregion

    #region LIVE API
    // login
    public string requesetKey;
    public string tryLogin;

    // friend list

    // request friend list

    // event banner & notice

    // file download
    public string fileDownload = "http://49.50.162.141:5002/onlineScienceMuseumAPI/downloadBuildFile.do";
    #endregion
    //=========================================================//
}
