using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Net.Http;
using UnityEngine;
using static System.Net.WebRequestMethods;

public class API : URL
{
    public static API instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        SetURL();
    }

    //// Update is called once per frame
    //void Update()
    //{

    //}

    #region Friend List
    // friend list
    public async UniTaskVoid Request_FriendList()
    {
        await UniTask.SwitchToThreadPool();

        Debug.Log("Request_FriendList() start()");

        JsonData jsonData = GameManager.instance.jsonData;
        FriendListManager friendListManager = GameManager.instance.friendListManager;

        var param = new Dictionary<string, string>
        {
            { "token", Login.PID },
            { "ncnm", "" }
        };

        var content = new FormUrlEncodedContent(param);
        
        //HttpContent content = new StringContent("", System.Text.Encoding.UTF8);

        HttpClient client = new HttpClient();
        //var response = await client.PostAsync("http://101.101.218.135:5002/onlineScienceMuseumAPI/frndInfo.do", content);
        var response = await client.PostAsync(friendListURL, content);
        string requestResult = await response.Content.ReadAsStringAsync();

        if (response.IsSuccessStatusCode)
        {
            Debug.Log("응답 성공");
            Debug.Log("친구 리스트 결과 : " + requestResult);
            //jsonData.temp_friendListValue = JsonUtility.FromJson<SaveData>(requestResult).frndInfoList; // temp data save

            List<SaveData.friendList> tempSaveData = JsonUtility.FromJson<SaveData>(requestResult).frndInfoList;
            //jsonData.temp_friendListValue = null;
            jsonData.temp_friendListValue = new List<SaveData.friendList>();
            
            for (int i = 0; tempSaveData.Count > i; i++)
            {
                if (tempSaveData[i].frndRqstSttus == "1")
                {
                    jsonData.temp_friendListValue.Add(tempSaveData[i]);
                }
            }
        }
        else
        {
            Debug.Log("응답 실패 (친구 리스트 결과) : " + requestResult);
        }

        await UniTask.SwitchToMainThread();

        // set friend list count
        friendListManager.friendCount = jsonData.temp_friendListValue.Count;

        // frist time setting
        if (jsonData.friendListValues.Count == 0 || (jsonData.friendListValues.Count != jsonData.temp_friendListValue.Count))
        {
            //Debug.Log("값 없음");
            //jsonData.friendListValues = null;
            //jsonData.friendListValues = JsonUtility.FromJson<SaveData>(requestResult).frndInfoList;
            //jsonData.friendListValues = jsonData.temp_friendListValue;

            jsonData.friendListValues = new List<SaveData.friendList>();
            List<SaveData.friendList> tempSaveData = JsonUtility.FromJson<SaveData>(requestResult).frndInfoList;
            jsonData.friendListValues = new List<SaveData.friendList>();

            for (int i = 0; tempSaveData.Count > i; i++)
            {
                if (tempSaveData[i].frndRqstSttus == "1")
                {
                    jsonData.friendListValues.Add(tempSaveData[i]);
                }
            }

            //Debug.Log("[SY] "+jsonData.friendListValues.Count);

            // create data
            //StartCoroutine(friendListManager.CreateList());
            friendListManager.CreateList();
        }
        else
        {
            // compare to json data
            bool isCompareResult = jsonData.CompareToFriendList(jsonData.friendListValues, jsonData.temp_friendListValue);
            //Debug.Log("[SY] : " + isCompareResult);

            if (!isCompareResult)
            {
                // delete date
                friendListManager.DeleteList();

                // create data
                //StartCoroutine(friendListManager.CreateList());
                friendListManager.CreateList();
            }
        }
    }

    // search friend
    public async UniTask<bool> Request_SearchFriend(string _nickName)
    {
        bool isSuccessSearch = false;

        Debug.Log("Request_FriendList() start()");
        JsonData jsonData = GameManager.instance.jsonData;
        FriendListManager friendListManager = GameManager.instance.friendListManager;

        var param = new Dictionary<string, string>
        {
            { "token", Login.PID },
            { "ncnm", _nickName }
        };

        var content = new FormUrlEncodedContent(param);

        HttpClient client = new HttpClient();
        //var response = await client.PostAsync("http://101.101.218.135:5002/onlineScienceMuseumAPI/frndInfo.do", content);
        var response = await client.PostAsync(friendListURL, content);
        string requestResult = await response.Content.ReadAsStringAsync();

        if (response.IsSuccessStatusCode)
        {
            Debug.Log("응답 성공");
            Debug.Log("친구 검색 결과 : " + requestResult);
            
            if (JsonUtility.FromJson<SaveData>(requestResult).frndInfoList.Count <= 0)
            {
                Debug.Log("해당 유저 없음");
                isSuccessSearch = false;
            }
            else
            {
                Debug.Log("해당 유저 존재");
                jsonData.searchFriend = JsonUtility.FromJson<SaveData>(requestResult).frndInfoList[0];
                isSuccessSearch = true;
            }
        }
        else
        {
            Debug.Log("응답 실패 (친구 검색 결과) : " + requestResult);
            isSuccessSearch = false;
        }

        // set friend list count
        friendListManager.friendCount = jsonData.temp_friendListValue.Count;

        return isSuccessSearch;
    }

    // add friend
    public async UniTaskVoid Request_AddFriend(int frndMbrNo, int mbrNo)
    {
        await UniTask.SwitchToThreadPool();
        Debug.Log("Request_AddFriend() start()");
        JsonData jsonData = GameManager.instance.jsonData;
        FriendListManager friendListManager = GameManager.instance.friendListManager;

        var param = new Dictionary<string, string>
        {
            { "frndMbrNo", frndMbrNo.ToString() },
            { "mbrNo", mbrNo.ToString() }
        };

        var content = new FormUrlEncodedContent(param);

        //HttpContent content = new StringContent("", System.Text.Encoding.UTF8);

        HttpClient client = new HttpClient();
        //var response = await client.PostAsync("http://101.101.218.135:5002/onlineScienceMuseumAPI/insertFrndInfo.do", content);
        var response = await client.PostAsync(addFriendURL, content);
        string requestResult = await response.Content.ReadAsStringAsync();

        if (response.IsSuccessStatusCode)
        {
            Debug.Log("응답 성공");
            Debug.Log("친구 신청 결과 : " + requestResult);
        }
        else
        {
            Debug.Log("응답 실패 (친구 신청 결과) : " + requestResult);
        }
        await UniTask.SwitchToMainThread();
        
        GameManager.instance.friendListManager.ResetSearchUserNickName();
    }
    #endregion

    #region Request Friend List
    // request list
    public async UniTaskVoid Request_RequestFriendList()
    {
        await UniTask.SwitchToThreadPool();

        Debug.Log("Request_RequestFriendList() start()");

        JsonData jsonData = GameManager.instance.jsonData;
        RequestFriendManager requestFriendManager = GameManager.instance.requestFriendManager;
        
        var param = new Dictionary<string, string>
        {
            { "token", Login.PID },
            { "ncnm", "" }
        };

        var content = new FormUrlEncodedContent(param);

        //HttpContent content = new StringContent("", System.Text.Encoding.UTF8);

        HttpClient client = new HttpClient();
        //var response = await client.PostAsync("http://101.101.218.135:5002/onlineScienceMuseumAPI/frndInfo.do", content);
        var response = await client.PostAsync(friendListURL, content);
        string requestResult = await response.Content.ReadAsStringAsync();

        if (response.IsSuccessStatusCode)
        {
            Debug.Log("응답 성공");
            Debug.Log("친구 요청 리스트 결과 : " + requestResult);
            //jsonData.temp_requestFriendListValues = JsonUtility.FromJson<SaveData>(requestResult).frndInfoList; // temp data save
            
            List<SaveData.friendList> tempSaveData = JsonUtility.FromJson<SaveData>(requestResult).frndInfoList;
            //jsonData.temp_requestFriendListValues = null;
            jsonData.temp_requestFriendListValues = new List<SaveData.friendList>();

            for (int i = 0; tempSaveData.Count > i; i++)
            {
                if (tempSaveData[i].frndRqstSttus == "0")
                {
                    jsonData.temp_requestFriendListValues.Add(tempSaveData[i]);
                }
            }
        }
        else
        {
            Debug.Log("응답 실패 (친구 요청 리스트 결과) : " + requestResult);
        }

        await UniTask.SwitchToMainThread();

        // set request friend list count
        requestFriendManager.requestfriendCount = jsonData.temp_requestFriendListValues.Count;

        // if (jsonData.friendListValues.Count == 0 || (jsonData.friendListValues.Count != jsonData.temp_friendListValue.Count))
        // frist time setting
        if (jsonData.requestFriendListValues.Count == 0 || (jsonData.requestFriendListValues.Count != jsonData.temp_requestFriendListValues.Count))
        {
            //Debug.Log("값 없음");
            //jsonData.requestFriendListValues = null;
            //jsonData.requestFriendListValues = JsonUtility.FromJson<SaveData>(requestResult).frndInfoList;
            //jsonData.requestFriendListValues = jsonData.temp_requestFriendListValues;

            jsonData.requestFriendListValues = new List<SaveData.friendList>();

            List<SaveData.friendList> tempSaveData = JsonUtility.FromJson<SaveData>(requestResult).frndInfoList;
            jsonData.requestFriendListValues = new List<SaveData.friendList>();

            for (int i = 0; tempSaveData.Count > i; i++)
            {
                if (tempSaveData[i].frndRqstSttus == "0")
                {
                    jsonData.requestFriendListValues.Add(tempSaveData[i]);
                }
            }

            //Debug.Log($"[SY] {jsonData.requestFriendListValues.Count}");

            // create data
            StartCoroutine(requestFriendManager.CreateRequestList());
        }
        else
        {
            // compare to json data
            bool isCompareResult = jsonData.CompareToFriendList(jsonData.requestFriendListValues, jsonData.temp_requestFriendListValues);
            //Debug.Log("[SY] : " + isCompareResult);

            if (!isCompareResult)
            {
                // delete date
                requestFriendManager.DeleteRequestList();

                // create data
                StartCoroutine(requestFriendManager.CreateRequestList());
            }
        }
    }

    // request accept
    public async UniTask Request_Accept(int _mbrNo, int _frndMbrNo)
    {
        //await UniTask.SwitchToThreadPool();

        Debug.Log("Request_Accept() start()");

        //JsonData jsonData = GameManager.instance.jsonData;
        //RequestFriendManager requestFriendManager = GameManager.instance.requestFriendManager;

        var param = new Dictionary<string, string>
        {
            { "mbrNo", _mbrNo.ToString() },
            { "frndMbrNo", _frndMbrNo.ToString() }
        };

        var content = new FormUrlEncodedContent(param);

        HttpClient client = new HttpClient();
        //var response = await client.PostAsync("http://101.101.218.135:5002/onlineScienceMuseumAPI/updateFrndReqAccept.do", content);
        var response = await client.PostAsync(requestAcceptURL, content);
        string requestResult = await response.Content.ReadAsStringAsync();

        if (response.IsSuccessStatusCode)
        {
            Debug.Log("응답 성공");
            Debug.Log("친구 요청 승락 결과 : " + requestResult);
        }
        else
        {
            Debug.Log("응답 실패 (친구 요청 승락 결과 ) : " + requestResult);
        }

        //await UniTask.SwitchToMainThread();
    }

    // request refuse & delete
    public async UniTask Request_RefuseNDelete(int _mbrNo, int _frndMbrNo)
    {
        Debug.Log("Request_RefuseNDelete() start()");

        //JsonData jsonData = GameManager.instance.jsonData;
        //RequestFriendManager requestFriendManager = GameManager.instance.requestFriendManager;

        var param = new Dictionary<string, string>
        {
            { "mbrNo", _mbrNo.ToString() },
            { "frndMbrNo", _frndMbrNo.ToString() }
        };

        var content = new FormUrlEncodedContent(param);

        //HttpContent content = new StringContent("", System.Text.Encoding.UTF8);

        HttpClient client = new HttpClient();
        //var response = await client.PostAsync("http://101.101.218.135:5002/onlineScienceMuseumAPI/deleteFrndReq.do", content);
        var response = await client.PostAsync(requestRefuseNDeleteURL, content);
        string requestResult = await response.Content.ReadAsStringAsync();

        if (response.IsSuccessStatusCode)
        {
            Debug.Log("응답 성공");
            Debug.Log("거절 및 삭제 결과 : " + requestResult);
            //jsonData.temp_requestFriendListValues = JsonUtility.FromJson<SaveData>(requestResult).frndInfoList; // temp data save
        }
        else
        {
            Debug.Log("응답 실패 (거절 및 삭제 결과) : " + requestResult);
        }
    }
    #endregion

    #region player state
    // upudate player state
    //public async UniTaskVoid Update_PlayerState(int status, string token) 
    public async UniTask Update_PlayerState(int status, string token) 
    {
        //await UniTask.SwitchToThreadPool();
        Debug.Log("Update_PlayerState() start()");

        var param = new Dictionary<string, string>
        {
            { "myStatus", status.ToString() },
            { "token", token }
        };

        var content = new FormUrlEncodedContent(param);

        HttpClient client = new HttpClient();
        //var response = await client.PostAsync("http://101.101.218.135:5002/onlineScienceMuseumAPI/changeMyStatus.do", content);
        var response = await client.PostAsync(playerStateUpdateURL, content);
        string requestResult = await response.Content.ReadAsStringAsync();

        if (response.IsSuccessStatusCode)
        {
            Debug.Log("응답 성공");
            Debug.Log("플레이어 상태 변경 결과 : " + requestResult);
        }
        else
        {
            // error code : TL_104
            Debug.Log("응답 실패 (플레이어 상태 변경 결과) : " + requestResult);

            // pid값이 유효하지 않습니다.
            GameManager.instance.popupManager.popups[(int)PopupType.InvalidPID].SetActive(true);
        }

        //await UniTask.SwitchToMainThread();
        
        Debug.Log($"{status}번으로 상태 변경 요청 완료!!!");
    }
    #endregion

    #region donwload file
    // checksum file donwload
    public async UniTask Request_FileDownloadURL(ServerType _pathFlag, FileType _folderFlag)
    {
        Debug.Log("Request_FileDownloadURL() start()");
        
        JsonData jsonData = GameManager.instance.jsonData;

        var param = new Dictionary<string, string>
        {
            { "pathFlag", _pathFlag.ToString() },
            { "folderFlag", _folderFlag.ToString() }
        };

        var content = new FormUrlEncodedContent(param);

        HttpClient client = new HttpClient();
        //var response = await client.PostAsync("http://101.101.218.135:5002/onlineScienceMuseumAPI/downloadBuildFile.do", content);
        var response = await client.PostAsync(fileDownloadURL, content);
        string requestResult = await response.Content.ReadAsStringAsync();

        if (response.IsSuccessStatusCode)
        {
            Debug.Log("응답 성공");
            Debug.Log("다운로드 경로 결과 : " + requestResult);

            string zipPath = JsonUtility.FromJson<SaveData.downloadUrlList>(requestResult).zip_path;
            string jsonPath = JsonUtility.FromJson<SaveData.downloadUrlList>(requestResult).json_path;

            jsonData.temp_donwloadUrl.zip_path = zipPath; // temp data save
            jsonData.temp_donwloadUrl.json_path = jsonPath; // temp data save

            GameManager.instance.filePath.buildFileUrls[(int)_folderFlag] = zipPath;
            GameManager.instance.filePath.jsonFileUrls[(int)_folderFlag] = jsonPath;
        }
        else
        {
            Debug.Log("응답 실패 (다운로드 경로 결과) : " + requestResult);
        }
    }

    public async UniTask Request_FileDownloadURL_live(FileType _folderFlag)
    {
        Debug.Log("Request_FileDownloadURL_live() start()");
        
        JsonData jsonData = GameManager.instance.jsonData;
        
        var param = new Dictionary<string, string>
        {
            { "pathFlag", "dev" },
            { "folderFlag", _folderFlag.ToString() }
        };

        var content = new FormUrlEncodedContent(param);

        HttpClient client = new HttpClient();
        //var response = await client.PostAsync("http://49.50.162.141:5002/onlineScienceMuseumAPI/downloadBuildFile.do", content);
        var response = await client.PostAsync(fileDownloadURL_Live, content);
        string requestResult = await response.Content.ReadAsStringAsync();

        if (response.IsSuccessStatusCode)
        {
            Debug.Log("응답 성공");
            Debug.Log("다운로드 경로 결과 : " + requestResult);

            string zipPath = JsonUtility.FromJson<SaveData.downloadUrlList>(requestResult).zip_path;
            string jsonPath = JsonUtility.FromJson<SaveData.downloadUrlList>(requestResult).json_path;

            //jsonData.temp_donwloadUrl.zip_path = zipPath; // temp data save
            //jsonData.temp_donwloadUrl.json_path = jsonPath; // temp data save

            //GameManager.instance.filePath.buildFileUrls[(int)_folderFlag] = zipPath;
            //GameManager.instance.filePath.jsonFileUrls[(int)_folderFlag] = jsonPath;

            // Todo : 문자열 변환....
            string temp_zipPath = zipPath.Replace("\\", "/");
            string temp_jsonPath = jsonPath.Replace("\\", "/");

            jsonData.temp_donwloadUrl.zip_path = temp_zipPath; // temp data save
            jsonData.temp_donwloadUrl.json_path = temp_jsonPath; // temp data save

            GameManager.instance.filePath.buildFileUrls[(int)_folderFlag] = temp_zipPath;
            GameManager.instance.filePath.jsonFileUrls[(int)_folderFlag] = temp_jsonPath;
        }
        else
        {
            Debug.Log("응답 실패 (다운로드 경로 결과) : " + requestResult);
        }
    }
    #endregion

    #region event banner & notice
    // event banner
    public async UniTaskVoid Request_EventBanner()
    {
        await UniTask.SwitchToThreadPool();
        Debug.Log("Request_EventBanner() start()");
        await UniTask.SwitchToMainThread();

        BannerNoticeManager bannerNoticeManager = GameManager.instance.bannerNoticeManager;
        bannerNoticeManager.bannerUI.TryAddContents(bannerNoticeManager.eventBannerCount);
    }

    // notice
    public async UniTaskVoid Request_Notice()
    {
        await UniTask.SwitchToThreadPool();
        Debug.Log("Request_Notice() start()");
        await UniTask.SwitchToMainThread();

        BannerNoticeManager bannerNoticeManager = GameManager.instance.bannerNoticeManager;
        bannerNoticeManager.noticeUIs[0].TryAddContents(bannerNoticeManager.noticeCount);
    }

    // curiverse notice
    public async UniTaskVoid Request_CuriverseNotice()
    {
        await UniTask.SwitchToThreadPool();
        Debug.Log("Request_CuriverseNotice() start()");
        await UniTask.SwitchToMainThread();

        BannerNoticeManager bannerNoticeManager = GameManager.instance.bannerNoticeManager;
        bannerNoticeManager.noticeUIs[1].TryAddContents(bannerNoticeManager.curiverseNoticeCount);
    }

    public async UniTaskVoid Request_GuideDownload1()
    {
        await UniTask.SwitchToThreadPool();
        Debug.Log("Request_GuideDownload1() start()");
        await UniTask.SwitchToMainThread();

        GameManager.instance.bannerNoticeManager.guideInfo[0].SetLinkURL("https://launcherdownload1.s3.ap-northeast-2.amazonaws.com/Test+PDF.pdf");
    }

    public async UniTaskVoid Request_GuideDownload2()
    {
        await UniTask.SwitchToThreadPool();
        Debug.Log("Request_GuideDownload2() start()");
        await UniTask.SwitchToMainThread();

        GameManager.instance.bannerNoticeManager.guideInfo[1].SetLinkURL("https://launcherdownload1.s3.ap-northeast-2.amazonaws.com/Test+PDF.pdf");
    }
    #endregion

    #region turn off pc
    public async UniTaskVoid Request_turnOffPC()
    {
        await UniTask.SwitchToThreadPool();
        Debug.Log("Request_turnOffPC() start()");
        await UniTask.SwitchToMainThread();

        Debug.Log("PC off 요청 완료");
    }
    #endregion
}