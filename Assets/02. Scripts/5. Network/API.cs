using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.CompilerServices;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

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

    #region Friend List
    // friend list
    public async UniTaskVoid Request_FriendList(bool isUsingPlayerUpdate = false)
    {
        if (isUsingPlayerUpdate)
        {
            await GameManager.instance.api.Update_PlayerState(GameManager.instance.playerManager.currentState, Login.PID);
        }

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
            Debug.Log("응답 성공 (친구 리스트 결과) : " + requestResult);
            //jsonData.temp_friendListValue = JsonUtility.FromJson<SaveData>(requestResult).frndInfoList; // temp data save

            List<SaveData.friendList> tempSaveData = JsonUtility.FromJson<SaveData>(requestResult).frndInfoList;
            //jsonData.temp_friendListValue = null;
            jsonData.temp_friendList_List = new List<SaveData.friendList>();
            
            for (int i = 0; tempSaveData.Count > i; i++)
            {
                if (tempSaveData[i].frndRqstSttus == "1")
                {
                    jsonData.temp_friendList_List.Add(tempSaveData[i]);
                }
            }
        }
        else
        {
            Debug.Log("응답 실패 (친구 리스트 결과) : " + requestResult);
        }

        await UniTask.SwitchToMainThread();

        // set friend list count
        friendListManager.friendCount = jsonData.temp_friendList_List.Count;

        // frist time setting
        if (jsonData.friendList_List.Count == 0 || (jsonData.friendList_List.Count != jsonData.temp_friendList_List.Count))
        {
            //Debug.Log("값 없음");
            //jsonData.friendList_List = new List<SaveData.friendList>();

            if (jsonData.friendList_List.Count != 0)
            {
                // delete data
                friendListManager.DeleteList();
            }

            List<SaveData.friendList> tempSaveData = JsonUtility.FromJson<SaveData>(requestResult).frndInfoList;
            jsonData.friendList_List = new List<SaveData.friendList>();

            for (int i = 0; tempSaveData.Count > i; i++)
            {
                if (tempSaveData[i].frndRqstSttus == "1")
                {
                    jsonData.friendList_List.Add(tempSaveData[i]);
                }
            }

            // create data
            //StartCoroutine(friendListManager.CreateList());
            friendListManager.CreateList();
        }
        else
        {
            // compare to json data
            bool isCompareResult = jsonData.CompareToFriendList(jsonData.friendList_List, jsonData.temp_friendList_List);
            //Debug.Log("[SY] : " + isCompareResult);

            if (!isCompareResult)
            {
                // delete data
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

        Debug.Log("Request_SearchFriend() start()");
        JsonData jsonData = GameManager.instance.jsonData;
        FriendListManager friendListManager = GameManager.instance.friendListManager;

        var param = new Dictionary<string, string>
        {
            { "nickname", _nickName }
        };

        var content = new FormUrlEncodedContent(param);

        HttpClient client = new HttpClient();
        //var response = await client.PostAsync("http://101.101.218.135:5002/onlineScienceMuseumAPI/frndInfo.do", content);
        var response = await client.PostAsync(searchUserWithNicknameURL, content);
        string requestResult = await response.Content.ReadAsStringAsync();

        if (response.IsSuccessStatusCode)
        {
            Debug.Log("응답 성공 (친구 검색 결과) : " + requestResult);
            
            if (requestResult == "no nick")
            {
                Debug.Log("[친구 검색] 해당 유저 없음");
                jsonData.searchFriendNum = "";
                isSuccessSearch = false;
            }
            else
            {
                Debug.Log("[친구 검색] 해당 유저 존재 : " + requestResult);
                jsonData.searchFriendNum = requestResult;
                isSuccessSearch = true;
            }
        }
        else
        {
            Debug.Log("응답 실패 (친구 검색 결과) : " + requestResult);
            isSuccessSearch = false;
        }

        //// set friend list count
        //friendListManager.friendCount = jsonData.temp_friendListValue.Count;

        return isSuccessSearch;
    }

    // add friend
    public async UniTaskVoid Temp_Request_AddFriend(string myNo, string mbrNo)
    {
        await UniTask.SwitchToThreadPool();
        Debug.Log("Request_AddFriend() start()");
        JsonData jsonData = GameManager.instance.jsonData;
        FriendListManager friendListManager = GameManager.instance.friendListManager;

        var param = new Dictionary<string, string>
        {
            //{ "mbrNo", myNo },
            //{ "frndMbrNo", mbrNo }
            { "mbrNo", mbrNo },
            { "frndMbrNo", myNo }
        };

        var content = new FormUrlEncodedContent(param);

        //HttpContent content = new StringContent("", System.Text.Encoding.UTF8);

        HttpClient client = new HttpClient();
        //var response = await client.PostAsync("http://101.101.218.135:5002/onlineScienceMuseumAPI/insertFrndInfo.do", content);
        var response = await client.PostAsync(addFriendURL, content);
        string requestResult = await response.Content.ReadAsStringAsync();

        if (response.IsSuccessStatusCode)
        {
            Debug.Log("응답 성공 (친구 신청 결과) : " + requestResult);
        }
        else
        {
            Debug.Log("응답 실패 (친구 신청 결과) : " + requestResult);
        }
        await UniTask.SwitchToMainThread();
        
        GameManager.instance.friendListManager.ResetSearchUserNickName();
    }

    public async UniTaskVoid Request_AddFriend(string token, string mbrNo)
    {
        await UniTask.SwitchToThreadPool();
        Debug.Log("Request_AddFriend() start()");
        JsonData jsonData = GameManager.instance.jsonData;
        FriendListManager friendListManager = GameManager.instance.friendListManager;

        var param = new Dictionary<string, string>
        {
            { "token", token }, // Todo : 추후 변경 예정
            { "frndMbrNo", mbrNo }
        };

        var content = new FormUrlEncodedContent(param);

        //HttpContent content = new StringContent("", System.Text.Encoding.UTF8);

        HttpClient client = new HttpClient();
        //var response = await client.PostAsync("http://101.101.218.135:5002/onlineScienceMuseumAPI/insertFrndInfo.do", content);
        var response = await client.PostAsync(addFriendURL, content);
        string requestResult = await response.Content.ReadAsStringAsync();

        if (response.IsSuccessStatusCode)
        {
            Debug.Log("응답 성공 (친구 신청 결과) : " + requestResult);
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
            Debug.Log("응답 성공 (친구 요청 리스트 결과) : " + requestResult);
            
            List<SaveData.friendList> tempSaveData = JsonUtility.FromJson<SaveData>(requestResult).frndInfoList;
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
            //jsonData.requestFriendListValues = new List<SaveData.friendList>();

            if (jsonData.requestFriendListValues.Count != 0)
            {
                // delete data
                requestFriendManager.DeleteRequestList();
            }

            List<SaveData.friendList> tempSaveData = JsonUtility.FromJson<SaveData>(requestResult).frndInfoList;
            jsonData.requestFriendListValues = new List<SaveData.friendList>();

            for (int i = 0; tempSaveData.Count > i; i++)
            {
                if (tempSaveData[i].frndRqstSttus == "0")
                {
                    jsonData.requestFriendListValues.Add(tempSaveData[i]);
                }
            }

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
                // delete data
                requestFriendManager.DeleteRequestList();

                // create data
                StartCoroutine(requestFriendManager.CreateRequestList());
            }
        }
    }

    // request accept
    public async UniTask Request_Accept(int _mbrNo, int _frndMbrNo)
    {
        Debug.Log("Request_Accept() start()");

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
            Debug.Log("응답 성공 (친구 요청 승락 결과) : " + requestResult);
        }
        else
        {
            Debug.Log("응답 실패 (친구 요청 승락 결과) : " + requestResult);
        }
    }

    // request refuse & delete
    public async UniTask Request_RefuseNDelete(int _mbrNo, int _frndMbrNo)
    {
        Debug.Log("Request_RefuseNDelete() start()");

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
            Debug.Log("응답 성공 (거절 및 삭제 결과) : " + requestResult);
        }
        else
        {
            Debug.Log("응답 실패 (거절 및 삭제 결과) : " + requestResult);
        }
    }
    #endregion

    #region player state
    // upudate player state
    public async UniTask Update_PlayerState(int status, string token) 
    {
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

        if (requestResult == "TL_104")
        {
            // error code : TL_104
            Debug.Log("응답 실패 (플레이어 상태 변경 결과) : " + requestResult);

            // pid값이 유효하지 않습니다.
            GameManager.instance.popupManager.popups[(int)PopupType.PlayerStateUpdateFailed].SetActive(true);
        }
        else if (requestResult == "no exist token")
        {
            Debug.Log("응답 실패 (플레이어 상태 변경 결과) : " + requestResult);

            // pid값이 유효하지 않습니다.
            GameManager.instance.popupManager.popups[(int)PopupType.InvalidPID].SetActive(true);
        }
        else
        {
            Debug.Log("응답 성공 (플레이어 상태 변경 결과) : " + requestResult);
            Debug.Log($"{status}번으로 상태 변경 요청 완료!!!");
        }
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
            Debug.Log("응답 성공 (다운로드 경로 결과) : " + requestResult);

            string zipPath = JsonUtility.FromJson<SaveData.downloadUrlList>(requestResult).zip_path;
            string jsonPath = JsonUtility.FromJson<SaveData.downloadUrlList>(requestResult).json_path;

            jsonData.temp_donwloadUrl.zip_path = zipPath; // temp data save
            jsonData.temp_donwloadUrl.json_path = jsonPath; // temp data save

            Debug.Log("[SY] 인덱스값 " + (int)_folderFlag);
            jsonData.temp_donwloadUrlList[(int)_folderFlag].zip_path = zipPath;
            jsonData.temp_donwloadUrlList[(int)_folderFlag].json_path = jsonPath;

            //GameManager.instance.filePath.buildFileUrls[(int)_folderFlag] = zipPath;
            //GameManager.instance.filePath.jsonFileUrls[(int)_folderFlag] = jsonPath;
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
            Debug.Log("응답 성공 (다운로드 경로 결과) : " + requestResult);

            string zipPath = JsonUtility.FromJson<SaveData.downloadUrlList>(requestResult).zip_path;
            string jsonPath = JsonUtility.FromJson<SaveData.downloadUrlList>(requestResult).json_path;

            jsonData.temp_donwloadUrl.zip_path = zipPath; // temp data save
            jsonData.temp_donwloadUrl.json_path = jsonPath; // temp data save

            jsonData.temp_donwloadUrlList[(int)_folderFlag].zip_path = zipPath;
            jsonData.temp_donwloadUrlList[(int)_folderFlag].json_path = jsonPath;
            
            //GameManager.instance.filePath.buildFileUrls[(int)_folderFlag] = temp_zipPath;
            //GameManager.instance.filePath.jsonFileUrls[(int)_folderFlag] = temp_jsonPath;
        }
        else
        {
            Debug.Log("응답 실패 (다운로드 경로 결과) : " + requestResult);
        }
    }
    #endregion

    #region event banner & notice
    public async UniTaskVoid Request_MainBoard(int boardType)
    {
        await UniTask.SwitchToThreadPool();

        Debug.Log("[Request_MainBoard] Request_MainBoard() start()");

        JsonData jsonData = GameManager.instance.jsonData;
        BannerNoticeManager bannerNoticeManager = GameManager.instance.bannerNoticeManager;

        int _boardType;

        if (boardType == 2)
        {
            _boardType = 0;
        }
        else
        {
            _boardType = boardType;
        }

        var param = new Dictionary<string, string>
        {
            { "boardType", _boardType.ToString() }
        };

        var content = new FormUrlEncodedContent(param);

        HttpClient client = new HttpClient();
        var response = await client.PostAsync(mainBoardURP, content);
        string requestResult = await response.Content.ReadAsStringAsync();

        if (response.IsSuccessStatusCode)
        {
            Debug.Log("응답 성공 (게시글 요청 결과) : " + requestResult);

            List<SaveData.mainBoard> tempSaveData = JsonUtility.FromJson<SaveData>(requestResult).mainboardlist;

            switch (boardType)
            {
                case 0:
                    // 이벤트
                    jsonData.temp_event_List = new List<SaveData.mainBoard>();
                    jsonData.temp_event_List = tempSaveData;
                    //jsonData.temp_event_List = OrderSort(tempSaveData, false);
                    break;
                case 1:
                    // 공지사항
                    jsonData.temp_shortNotice_List = new List<SaveData.mainBoard>();
                    jsonData.temp_shortNotice_List = tempSaveData;
                    //jsonData.temp_shortNotice_List = OrderSort(tempSaveData, false);
                    break;
                case 2:
                    // 소식
                    jsonData.temp_news_List = new List<SaveData.mainBoard>();
                    jsonData.temp_news_List = tempSaveData;
                    //jsonData.temp_news_List = OrderSort(tempSaveData, false);
                    break;
                case 3:
                    // 가이드
                    Debug.Log("가이드 : " + requestResult);
                    jsonData.temp_guide_List = new List<SaveData.mainBoard>();
                    jsonData.temp_guide_List = tempSaveData;
                    //jsonData.temp_guide_List = OrderSort(tempSaveData, true);
                    break;
            }
        }
        else
        {
            Debug.Log("응답 실패 (게시글 요청 결과) : " + requestResult);
        }

        await UniTask.SwitchToMainThread();

        // set conents list count
        switch (boardType)
        {
            // event banner
            case 0:
                // set event count
                bannerNoticeManager.eventBannerCount = jsonData.temp_event_List.Count;

                // frist time setting
                if (jsonData.event_List.Count == 0 || (jsonData.event_List.Count != jsonData.temp_event_List.Count))
                {
                    if (jsonData.event_List.Count != 0)
                    {
                        // delete data
                        bannerNoticeManager.bannerUI.DeleteContents();
                    }

                    jsonData.event_List = new List<SaveData.mainBoard>();
                    List<SaveData.mainBoard> tempSaveData = JsonUtility.FromJson<SaveData>(requestResult).mainboardlist;
                    jsonData.event_List = tempSaveData;
                    //jsonData.event_List = OrderSort(tempSaveData, false);

                    // create data
                    bannerNoticeManager.CreateEventBanner();
                }
                else
                {
                    // compare to json data
                    bool isCompareResult = jsonData.CompareToMainBoard(jsonData.event_List, jsonData.temp_event_List);

                    if (!isCompareResult)
                    {
                        // delete data
                        bannerNoticeManager.bannerUI.DeleteContents();

                        // create data
                        bannerNoticeManager.CreateEventBanner();
                    }
                }
                break;
            // short notice
            case 1:
                // set short notice count
                bannerNoticeManager.shortNoticeCount = jsonData.temp_shortNotice_List.Count;

                // frist time setting
                if (jsonData.shortNotice_List.Count == 0 || (jsonData.shortNotice_List.Count != jsonData.temp_shortNotice_List.Count))
                {
                    if (jsonData.shortNotice_List.Count != 0)
                    {
                        // delete data
                        bannerNoticeManager.shortNotice.ResetShortNoticeInfo();
                    }

                    jsonData.shortNotice_List = new List<SaveData.mainBoard>();
                    List<SaveData.mainBoard> tempSaveData = JsonUtility.FromJson<SaveData>(requestResult).mainboardlist;
                    jsonData.shortNotice_List = tempSaveData;
                    //jsonData.shortNotice_List = OrderSort(tempSaveData, false);

                    // create data
                    bannerNoticeManager.SetNotice();
                }
                else
                {
                    // compare to json data
                    bool isCompareResult = jsonData.CompareToMainBoard(jsonData.shortNotice_List, jsonData.temp_shortNotice_List);

                    if (!isCompareResult)
                    {
                        // delete data
                        bannerNoticeManager.shortNotice.ResetShortNoticeInfo();

                        // create data
                        bannerNoticeManager.SetNotice();
                    }
                }
                break;
            // event news
            case 2:
                // set event news count
                bannerNoticeManager.eventNewsCount = jsonData.temp_news_List.Count;

                // frist time setting
                if (jsonData.news_List.Count == 0 || (jsonData.news_List.Count != jsonData.temp_news_List.Count))
                {
                    if (jsonData.news_List.Count != 0)
                    {
                        // delete data
                        bannerNoticeManager.noticeUI.DeleteContents();
                    }

                    jsonData.news_List = new List<SaveData.mainBoard>();
                    List<SaveData.mainBoard> tempSaveData = JsonUtility.FromJson<SaveData>(requestResult).mainboardlist;
                    jsonData.news_List = tempSaveData;
                    //jsonData.news_List = OrderSort(tempSaveData, false);

                    // create data
                    bannerNoticeManager.CreateNews();
                }
                else
                {
                    // compare to json data
                    bool isCompareResult = jsonData.CompareToMainBoard(jsonData.news_List, jsonData.temp_news_List);

                    if (!isCompareResult)
                    {
                        // delete data
                        bannerNoticeManager.noticeUI.DeleteContents();

                        // create data
                        bannerNoticeManager.CreateNews();
                    }
                }
                break;
            // download guide
            case 3:
                // set download guide count
                bannerNoticeManager.guideCount = jsonData.temp_guide_List.Count;

                // frist time setting
                if (jsonData.guide_List.Count == 0 || (jsonData.guide_List.Count != jsonData.temp_guide_List.Count))
                {
                    if (jsonData.guide_List.Count != 0)
                    {
                        // delete data
                        bannerNoticeManager.guideInfo[0].ResetLinkURL();
                        bannerNoticeManager.guideInfo[1].ResetLinkURL();
                    }

                    jsonData.guide_List = new List<SaveData.mainBoard>();
                    List<SaveData.mainBoard> tempSaveData = JsonUtility.FromJson<SaveData>(requestResult).mainboardlist;
                    jsonData.guide_List = tempSaveData;
                    //jsonData.guide_List = OrderSort(tempSaveData, true);

                    // create data
                    bannerNoticeManager.SetGuideDowloadLink();
                }
                else
                {
                    // compare to json data
                    bool isCompareResult = jsonData.CompareToMainBoard(jsonData.guide_List, jsonData.temp_guide_List);

                    if (!isCompareResult)
                    {
                        // delete data
                        bannerNoticeManager.guideInfo[0].ResetLinkURL();
                        bannerNoticeManager.guideInfo[1].ResetLinkURL();

                        // create data
                        bannerNoticeManager.SetGuideDowloadLink();
                    }
                }
                break;
        }
    }

    // Todo : 게시판 데이터 정렬함수 삭제 예정
    private List<SaveData.mainBoard> OrderSort(List<SaveData.mainBoard> _target, bool isAscending)
    {
        List<SaveData.mainBoard> target = new List<SaveData.mainBoard>();

        if (isAscending)
        {
            target = _target.OrderBy(x => x.boardNum).ToList();
        }
        else
        {
            target = _target.OrderByDescending(x => x.boardNum).ToList();
        }

        //for (int i = 0; i < target.Count; i++)
        //{
        //    Debug.Log("[정렬] : " + target[i].boardNum);
        //}

        return target;
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

    public async UniTaskVoid Request_GuideDownload(int guideType)
    {
        Debug.Log("Request_GuideDownload() start()");
        await UniTask.SwitchToThreadPool();

        var param = new Dictionary<string, string>
        {
            { "boardType", guideType.ToString() }
        };

        var content = new FormUrlEncodedContent(param);

        HttpClient client = new HttpClient();
        var response = await client.PostAsync(mainBoardURP, content);
        string requestResult = await response.Content.ReadAsStringAsync();

        if (response.IsSuccessStatusCode)
        {
            Debug.Log("응답 성공 (가이드 로드 경로 요청 결과) : " + requestResult);
        }
        else
        {
            Debug.Log("응답 실패 (가이드 로드 경로 요청 결과) : " + requestResult);
        }

        await UniTask.SwitchToMainThread();
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