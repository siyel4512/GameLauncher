using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using UnityEngine;
using UnityEngine.Networking;
using static System.Windows.Forms.LinkLabel;
using static Unity.VisualScripting.Icons;

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
        await UniTask.SwitchToMainThread();

        Debug.Log("Request_FriendList() start()");

        JsonData jsonData = GameManager.instance.jsonData;
        FriendListManager friendListManager = GameManager.instance.friendListManager;

        var content = new WWWForm();
        content.AddField("token", Login.PID);
        content.AddField("ncnm", "");

        string temp_requestResult = "";

        using (UnityWebRequest www = UnityWebRequest.Post(friendListURL, content))
        {
            try
            {
                await www.SendWebRequest();

                if (www.result == UnityWebRequest.Result.Success)
                {
                    string requestResult = www.downloadHandler.text;

                    Debug.Log("응답 성공 (친구 리스트 결과) : " + requestResult);
                    temp_requestResult = requestResult;
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
            }
            catch (UnityWebRequestException ex)
            {
                string requestResult = www.downloadHandler.text;
                Debug.Log("응답 실패 (친구 리스트 결과) : " + requestResult);
                temp_requestResult = requestResult;

                if (DEV.instance.isSendingErrorLog)
                {
                    // send error log
                    Send_AbnormalShutdown($"request friend list response failed : {www.error}").Forget();
                }
            }
        }

        //await UniTask.SwitchToMainThread();

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

            List<SaveData.friendList> tempSaveData = JsonUtility.FromJson<SaveData>(temp_requestResult).frndInfoList;
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
        
        var content = new WWWForm();
        content.AddField("id", _nickName);

        using (UnityWebRequest www = UnityWebRequest.Post(searchUserWithNicknameURL, content))
        {
            try
            {
                await www.SendWebRequest();

                if (www.result == UnityWebRequest.Result.Success)
                {
                    string requestResult = www.downloadHandler.text;

                    Debug.Log("응답 성공 (친구 검색 결과) : " + requestResult);

                    if (requestResult == "no id")
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
            }
            catch (UnityWebRequestException ex)
            {
                string requestResult = www.downloadHandler.text;

                Debug.Log("응답 실패 (친구 검색 결과) : " + requestResult);
                isSuccessSearch = false;

                if (DEV.instance.isSendingErrorLog)
                {
                    // send error log
                    Send_AbnormalShutdown($"request search friend response failed : {www.error}").Forget();
                }
            }
        }

        return isSuccessSearch;
    }

    // add friend
    //public async UniTask Temp_Request_AddFriend(string myNo, string mbrNo)
    public async UniTaskVoid Temp_Request_AddFriend(string myNo, string mbrNo)
    {
        Debug.Log("Request_AddFriend() start()");
        JsonData jsonData = GameManager.instance.jsonData;
        FriendListManager friendListManager = GameManager.instance.friendListManager;

        var content = new WWWForm();
        //content.AddField("mbrNo", myNo);
        //content.AddField("frndMbrNo", mbrNo);
        content.AddField("mbrNo", mbrNo);
        content.AddField("frndMbrNo", myNo);

        using (UnityWebRequest www = UnityWebRequest.Post(addFriendURL, content))
        {
            try
            {
                await www.SendWebRequest();

                if (www.result == UnityWebRequest.Result.Success)
                {
                    string requestResult = www.downloadHandler.text;
                    Debug.Log("응답 성공 (친구 신청 결과) : " + requestResult);
                }
            }
            catch (UnityWebRequestException ex)
            {
                string requestResult = www.downloadHandler.text;
                Debug.Log("응답 실패 (친구 신청 결과) : " + requestResult);

                if (DEV.instance.isSendingErrorLog)
                {
                    // send error log
                    Send_AbnormalShutdown($"temp request add friend response failed : {www.error}").Forget();
                }
            }
        }

        GameManager.instance.friendListManager.ResetSearchUserNickName();

        await GameManager.instance.api.Update_PlayerState(GameManager.instance.playerManager.currentState, Login.PID);
    }

    public async UniTaskVoid Request_AddFriend(string token, string mbrNo)
    {
        await UniTask.SwitchToThreadPool();
        Debug.Log("Request_AddFriend() start()");
        JsonData jsonData = GameManager.instance.jsonData;
        FriendListManager friendListManager = GameManager.instance.friendListManager;

        var content = new WWWForm();
        content.AddField("token", token); // Todo : 추후 변경 예정
        content.AddField("frndMbrNo", mbrNo);

        using (UnityWebRequest www = UnityWebRequest.Post(addFriendURL, content))
        {
            try
            {
                await www.SendWebRequest();

                if (www.result == UnityWebRequest.Result.Success)
                {
                    string requestResult = www.downloadHandler.text;

                    Debug.Log("응답 성공 (친구 신청 결과) : " + requestResult);
                }
            }
            catch (UnityWebRequestException ex)
            {
                string requestResult = www.downloadHandler.text;
                Debug.Log("응답 실패 (친구 신청 결과) : " + requestResult + " // " + ex);

                if (DEV.instance.isSendingErrorLog)
                {
                    // send error log
                    Send_AbnormalShutdown($"requeset add friend response failed : {www.error}").Forget();
                }
            }

            GameManager.instance.friendListManager.ResetSearchUserNickName();
        }
    }
    #endregion

    #region Request Friend List
    // request list
    public async UniTaskVoid Request_RequestFriendList()
    {
        Debug.Log("Request_RequestFriendList() start()");

        JsonData jsonData = GameManager.instance.jsonData;
        RequestFriendManager requestFriendManager = GameManager.instance.requestFriendManager;

        var content = new WWWForm();
        content.AddField("token", Login.PID);
        content.AddField("ncnm", "");

        string temp_requestResult = "";

        using (UnityWebRequest www = UnityWebRequest.Post(friendListURL, content))
        {
            try
            {
                await www.SendWebRequest();

                if (www.result == UnityWebRequest.Result.Success)
                {
                    string requestResult = www.downloadHandler.text;
                    temp_requestResult = requestResult;

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
            }
            catch (UnityWebRequestException ex)
            {
                string requestResult = www.downloadHandler.text;

                Debug.Log("응답 실패 (친구 요청 리스트 결과) : " + requestResult + " // " + ex);
                temp_requestResult = requestResult;

                if (DEV.instance.isSendingErrorLog)
                {
                    // send error log
                    Send_AbnormalShutdown($"request reqeuset_friend list response failed : {www.error}").Forget();
                }
            }
        }

        // set request friend list count
        requestFriendManager.requestfriendCount = jsonData.temp_requestFriendListValues.Count;

        // frist time setting
        if (jsonData.requestFriendListValues.Count == 0 || (jsonData.requestFriendListValues.Count != jsonData.temp_requestFriendListValues.Count))
        {
            if (jsonData.requestFriendListValues.Count != 0)
            {
                // delete data
                requestFriendManager.DeleteRequestList();
            }

            List<SaveData.friendList> tempSaveData = JsonUtility.FromJson<SaveData>(temp_requestResult).frndInfoList;
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

        var content = new WWWForm();
        content.AddField("mbrNo", _mbrNo.ToString());
        content.AddField("frndMbrNo", _frndMbrNo.ToString());

        using (UnityWebRequest www = UnityWebRequest.Post(requestAcceptURL, content))
        {
            try
            {
                await www.SendWebRequest();

                if (www.result == UnityWebRequest.Result.Success)
                {
                    string requestResult = www.downloadHandler.text;
                    Debug.Log("응답 성공 (친구 요청 승락 결과) : " + requestResult);
                }
            }
            catch (UnityWebRequestException ex)
            {
                string requestResult = www.downloadHandler.text;
                Debug.Log("응답 실패 (친구 요청 승락 결과) : " + requestResult + " // " + ex);

                if (DEV.instance.isSendingErrorLog)
                {
                    // send error log
                    Send_AbnormalShutdown($"request accept response failed : {www.error}").Forget();
                }
            }
        }
    }

    // request refuse & delete
    public async UniTask Request_RefuseNDelete(int _mbrNo, int _frndMbrNo)
    {
        Debug.Log("Request_RefuseNDelete() start()");

        var content = new WWWForm();
        content.AddField("mbrNo", _mbrNo.ToString());
        content.AddField("frndMbrNo", _frndMbrNo.ToString());

        using (UnityWebRequest www = UnityWebRequest.Post(requestRefuseNDeleteURL, content))
        {
            try
            {
                await www.SendWebRequest();

                if (www.result == UnityWebRequest.Result.Success)
                {
                    string requestResult = www.downloadHandler.text;
                    Debug.Log("응답 성공 (거절 및 삭제 결과) : " + requestResult);
                }
            }
            catch (UnityWebRequestException ex)
            {
                string requestResult = www.downloadHandler.text;
                Debug.Log("응답 실패 (거절 및 삭제 결과) : " + requestResult + " // " + ex);

                if (DEV.instance.isSendingErrorLog)
                {
                    // send error log
                    Send_AbnormalShutdown($"request refuse and delete response failed : {www.error}").Forget();
                }
            }
        }
    }
    #endregion

    #region player state
    // upudate player state
    public async UniTask Update_PlayerState(int status, string token) 
    {
        Debug.Log("Update_PlayerState() start()");

        var content = new WWWForm();
        content.AddField("myStatus", status.ToString());
        content.AddField("token", token);

        using (UnityWebRequest www = UnityWebRequest.Post(playerStateUpdateURL, content))
        {
            try
            {
                await www.SendWebRequest();
                //string requestResult = www.downloadHandler.text;
            }
            catch (UnityWebRequestException ex)
            {
                string requestResult = www.downloadHandler.text;
                Debug.Log("플레이어 상태 변경 결과 requestResult : " + requestResult);

                if (requestResult == "TL_104")
                {
                    // error code : TL_104
                    Debug.Log("응답 실패 (플레이어 상태 변경 결과) : " + requestResult);

                    if (!GameManager.instance.isQuit)
                    {
                        // pid값이 유효하지 않습니다.
                        GameManager.instance.popupManager.popups[(int)PopupType.PlayerStateUpdateFailed].SetActive(true);
                    }
                }
                else if (requestResult == "no exist token")
                {
                    Debug.Log("응답 실패 (플레이어 상태 변경 결과) : " + requestResult);

                    if (!GameManager.instance.isQuit)
                    {
                        // pid값이 유효하지 않습니다.
                        GameManager.instance.popupManager.popups[(int)PopupType.InvalidPID].SetActive(true);
                    }
                }
                else
                {
                    Debug.Log("응답 성공 (플레이어 상태 변경 결과) : " + requestResult);
                    Debug.Log($"{status}번으로 상태 변경 요청 완료!!!");
                }
            }
        }
    }
    #endregion

    #region donwload file
    // checksum file donwload
    public async UniTask Request_FileDownloadURL(ServerType _pathFlag, FileType _folderFlag)
    {
        Debug.Log("Request_FileDownloadURL() start()");

        JsonData jsonData = GameManager.instance.jsonData;

        var content = new WWWForm();
        content.AddField("pathFlag", _pathFlag.ToString());
        content.AddField("folderFlag", _folderFlag.ToString());

        using (UnityWebRequest www = UnityWebRequest.Post(fileDownloadURL, content))
        {
            try
            {
                await www.SendWebRequest();

                if (www.result == UnityWebRequest.Result.Success)
                {
                    string requestResult = www.downloadHandler.text;

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
            }
            catch (UnityWebRequestException ex)
            {
                string requestResult = www.downloadHandler.text;

                Debug.Log("응답 실패 (다운로드 경로 결과) : " + requestResult + " // " + ex);
            }
        }
    }

    public async UniTask Request_FileDownloadURL_live(FileType _folderFlag)
    {
        Debug.Log("Request_FileDownloadURL_live() start()");

        JsonData jsonData = GameManager.instance.jsonData;


        var content = new WWWForm();
        content.AddField("pathFlag", "dev");
        content.AddField("folderFlag", _folderFlag.ToString());

        using (UnityWebRequest www = UnityWebRequest.Post(fileDownloadURL_Live, content))
        {
            try
            {
                await www.SendWebRequest();

                if (www.result == UnityWebRequest.Result.Success)
                {
                    string requestResult = www.downloadHandler.text;

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
            }
            catch (UnityWebRequestException ex)
            {
                string requestResult = www.downloadHandler.text;

                Debug.Log("응답 실패 (다운로드 경로 결과) : " + requestResult + " // " + ex);
            }
        }
    }
    #endregion

    #region event banner & notice
    public async UniTaskVoid Request_MainBoard(int boardType)
    {
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

        var content = new WWWForm();
        content.AddField("boardType", _boardType.ToString());

        string temp_requestResult = "";

        using (UnityWebRequest www = UnityWebRequest.Post(mainBoardURP, content))
        {
            try
            {
                await www.SendWebRequest();

                if (www.result == UnityWebRequest.Result.Success)
                {
                    string requestResult = www.downloadHandler.text;

                    temp_requestResult = requestResult;

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
            }
            catch (UnityWebRequestException ex)
            {
                string requestResult = www.downloadHandler.text;
                temp_requestResult = requestResult;
                Debug.Log("응답 실패 (게시글 요청 결과) : " + requestResult + " // " + ex);
            }
        }

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
                    List<SaveData.mainBoard> tempSaveData = JsonUtility.FromJson<SaveData>(temp_requestResult).mainboardlist;
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
                    List<SaveData.mainBoard> tempSaveData = JsonUtility.FromJson<SaveData>(temp_requestResult).mainboardlist;
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
                    List<SaveData.mainBoard> tempSaveData = JsonUtility.FromJson<SaveData>(temp_requestResult).mainboardlist;
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
                    List<SaveData.mainBoard> tempSaveData = JsonUtility.FromJson<SaveData>(temp_requestResult).mainboardlist;
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

    #region Abnormal Shutdown
    public async UniTaskVoid Send_AbnormalShutdown(string errorLog)
    {
        Debug.Log("Send_AbnormalShutdown() start()");

        JsonData jsonData = GameManager.instance.jsonData;
        //FriendListManager friendListManager = GameManager.instance.friendListManager;

        var content = new WWWForm();
        content.AddField("divType", "02");
        content.AddField("errDesc", errorLog);

        using (UnityWebRequest www = UnityWebRequest.Post(abnormalShutdownURL, content))
        {
            try
            {
                await www.SendWebRequest();

                if (www.result == UnityWebRequest.Result.Success)
                {
                    string requestResult = www.downloadHandler.text;

                    Debug.Log("응답 성공 (에러 로그) : " + requestResult);
                }
            }
            catch (UnityWebRequestException ex)
            {
                string requestResult = www.downloadHandler.text;
                Debug.Log("응답 실패 (에러 로그) : " + requestResult + " // " + ex);
            }
        }
    }
    #endregion

    #region Launcher Version Check
    public async UniTaskVoid LauncherVersionCheck()
    {
        Debug.Log("LauncherVersionCheck() start()");
        var content = new WWWForm();

        using (UnityWebRequest www = UnityWebRequest.Post(launcherVersionCheckURL, content))
        {
            try
            {
                await www.SendWebRequest();

                if (www.result == UnityWebRequest.Result.Success)
                {
                    string requestResult = www.downloadHandler.text;

                    Debug.Log("응답 성공 (런처 버전 확인) : " + requestResult);

                    string currentLauncherVersion = DEV.instance.versionManager.LoadVersion().major + "." + DEV.instance.versionManager.LoadVersion().minor + "." + DEV.instance.versionManager.LoadVersion().patch;
                    //Debug.Log($"현재 런처 버전 : {DEV.instance.versionManager.LoadVersion().major}.{DEV.instance.versionManager.LoadVersion().minor}.{DEV.instance.versionManager.LoadVersion().patch}");
                    //Debug.Log($"현재 런처 버전2 : {DEV.instance.versionManager.versionText_Login.text}");
                    //Debug.Log($"현재 런처 버전3 : {currentLauncherVersion}");

                    // 런처 버전 비교
                    if (requestResult != currentLauncherVersion)
                    //if (requestResult != "1.0.0")
                    {
                        Debug.Log("[Launcher version check] 버전 다름");
                        GameManager.instance.popupManager.ShowLauncherUpdatePopup();
                    }
                    else
                    {
                        Debug.Log("[Launcher version check] 버전 같음 / 업데이트 할 필요 없음...");
                    }
                }
            }
            catch (UnityWebRequestException ex)
            {
                Debug.Log("응답 실패 (런처 버전 확인) : " + ex);

                if (DEV.instance.isSendingErrorLog)
                {
                    // send error log
                    Send_AbnormalShutdown($"launcher version check response failed : {www.error}").Forget();
                }
            }
        }
    }
    #endregion

    #region English Video Link
    //public async UniTaskVoid Request_EnglishVideoLink(string language)
    public async UniTask<string> Request_EnglishVideoLink(string language)
    {
        Debug.Log("Request_EnglishVideoLink() start()");

        string link = "";

        var content = new WWWForm();
        content.AddField("language", language); // ko, en

        using (UnityWebRequest www = UnityWebRequest.Post(videoEnURL, content))
        {
            try
            {
                await www.SendWebRequest();

                if (www.result == UnityWebRequest.Result.Success)
                {
                    string requestResult = www.downloadHandler.text;

                    Debug.Log("응답 성공 (영문 소개 영상) : " + requestResult);
                    link = requestResult;
                    //link = "";
                }
            }
            catch (UnityWebRequestException ex)
            {
                string requestResult = www.downloadHandler.text;
                Debug.Log("응답 실패 (영문 소개 영상) : " + requestResult + " // " + ex);
                link = "";
            }
        }

        return link;
    }
    #endregion

    #region User Guide Link
    // launcher user guide
    public async UniTask<string> Request_LauncherUserGuideLink(string language)
    {
        Debug.Log("Request_LauncherUserGuideLink() start()");
        
        string link = "";

        var content = new WWWForm();
        content.AddField("language", language); // ko, en
        Debug.Log("URL : " + launcherUserGuideURL);
        using (UnityWebRequest www = UnityWebRequest.Post(launcherUserGuideURL, content))
        {
            try
            {
                await www.SendWebRequest();

                if (www.result == UnityWebRequest.Result.Success)
                {
                    string requestResult = www.downloadHandler.text;

                    Debug.Log("응답 성공 (런처 유저 가이드) : " + requestResult);
                    link = requestResult;
                }
            }
            catch (UnityWebRequestException ex)
            {
                string requestResult = www.downloadHandler.text;
                Debug.Log("응답 실패 (런처 유저 가이드) : " + requestResult + " // " + ex);
                link = "https://metaply.go.kr/";
            }
        }

        return link;
    }

    // ugc install menual
    public async UniTask<string> Request_UGCInstallMenualLink(string language)
    {
        Debug.Log("Request_UgcInstallMenualLink() start()");

        string link = "";

        var content = new WWWForm();
        content.AddField("language", language); // ko, en

        using (UnityWebRequest www = UnityWebRequest.Post(ugcInstallMenualURL, content))
        {
            try
            {
                await www.SendWebRequest();

                if (www.result == UnityWebRequest.Result.Success)
                {
                    string requestResult = www.downloadHandler.text;

                    Debug.Log("응답 성공 (UGC Install Menual) : " + requestResult);
                    link = requestResult;
                }
            }
            catch (UnityWebRequestException ex)
            {
                string requestResult = www.downloadHandler.text;
                Debug.Log("응답 실패 (UGC Install Menual) : " + requestResult + " // " + ex);
                link = "https://metaply.go.kr/";
            }
        }

        return link;
    }
    #endregion
}