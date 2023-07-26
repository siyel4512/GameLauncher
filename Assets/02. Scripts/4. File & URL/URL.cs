using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class URL : LoadFile
{
    public static URL Instance;

    // login
    private string getKeyUrl;
    private string loginUrl;
    private string key_id;
    private string key_password;

    public string GetKeyUrl => getKeyUrl;
    public string LoginUrl => loginUrl;
    public string Key_id => key_id;
    public string Key_password => key_password;

    // friend list
    public string friendList;
    public string requestFriendList;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        SetSettingValues();
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // read setting file content
    public void SetSettingValues()
    {
        string[] parsingData = ParsingData();

        key_id = parsingData[0];
        key_password = parsingData[1];

        getKeyUrl = parsingData[3];
        loginUrl = parsingData[4];
    }

    #region TEST API
    // login
    public string TEST_requesetKey = "http://101.101.218.135:5002/onlineScienceMuseumAPI/checkId.do";
    public string TEST_tryLogin = "http://101.101.218.135:5002/onlineScienceMuseumAPI/tryLogin.do";

    // friend list
    public string TEST_friendList = "http://101.101.218.135:5002/onlineScienceMuseumAPI/frndInfo.do";
    public string TEST_addFriend = "http://101.101.218.135:5002/onlineScienceMuseumAPI/insertFrndInfo.do";

    // request friend list
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
}
