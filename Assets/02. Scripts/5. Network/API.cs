using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Net.Http;
using UnityEngine;

public class API : URL
{
    public static API instance;

    // Start is called before the first frame update
    void Start()
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
        var response = await client.PostAsync("http://101.101.218.135:5002/onlineScienceMuseumAPI/frndInfo.do", content);
        string requestResult = await response.Content.ReadAsStringAsync();

        if (response.IsSuccessStatusCode)
        {
            Debug.Log("응답 성공");
            Debug.Log("친구 리스트 결과 : " + requestResult);
            jsonData.temp_friendListValue = JsonUtility.FromJson<SaveData>(requestResult).frndInfoList; // temp data save
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
            jsonData.friendListValues = null;            
            jsonData.friendListValues = JsonUtility.FromJson<SaveData>(requestResult).frndInfoList;

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

        //await UniTask.SwitchToThreadPool();

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
        var response = await client.PostAsync("http://101.101.218.135:5002/onlineScienceMuseumAPI/frndInfo.do", content);
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

        //await UniTask.SwitchToMainThread();

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
        var response = await client.PostAsync("http://101.101.218.135:5002/onlineScienceMuseumAPI/insertFrndInfo.do", content);
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
        var response = await client.PostAsync("http://101.101.218.135:5002/onlineScienceMuseumAPI/frndInfo.do", content);
        string requestResult = await response.Content.ReadAsStringAsync();

        if (response.IsSuccessStatusCode)
        {
            Debug.Log("응답 성공");
            Debug.Log("친구 요청 리스트 결과 : " + requestResult);
            jsonData.temp_requestFriendListValues = JsonUtility.FromJson<SaveData>(requestResult).frndInfoList; // temp data save
        }
        else
        {
            Debug.Log("응답 실패 (친구 요청 리스트 결과) : " + requestResult);
        }

        await UniTask.SwitchToMainThread();

        // set request friend list count
        requestFriendManager.requestfriendCount = jsonData.temp_requestFriendListValues.Count;

        // frist time setting
        if (jsonData.requestFriendListValues.Count == 0)
        {
            //Debug.Log("값 없음");
            jsonData.requestFriendListValues = JsonUtility.FromJson<SaveData>(requestResult).frndInfoList;

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
        JsonData jsonData = GameManager.instance.jsonData;
        RequestFriendManager requestFriendManager = GameManager.instance.requestFriendManager;

        var param = new Dictionary<string, string>
        {
            { "mbrNo", _mbrNo.ToString() },
            { "frndMbrNo", _frndMbrNo.ToString() }
        };

        var content = new FormUrlEncodedContent(param);

        HttpClient client = new HttpClient();
        var response = await client.PostAsync("http://101.101.218.135:5002/onlineScienceMuseumAPI/updateFrndReqAccept.do", content);
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
        JsonData jsonData = GameManager.instance.jsonData;
        RequestFriendManager requestFriendManager = GameManager.instance.requestFriendManager;

        var param = new Dictionary<string, string>
        {
            { "mbrNo", _mbrNo.ToString() },
            { "frndMbrNo", _frndMbrNo.ToString() }
        };

        var content = new FormUrlEncodedContent(param);

        //HttpContent content = new StringContent("", System.Text.Encoding.UTF8);

        HttpClient client = new HttpClient();
        var response = await client.PostAsync("http://101.101.218.135:5002/onlineScienceMuseumAPI/deleteFrndReq.do", content);
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
    public async UniTaskVoid Update_PlayerState() 
    {
        await UniTask.SwitchToThreadPool();
        Debug.Log("Update_PlayerState() start()");
        await UniTask.SwitchToMainThread();
    }
    #endregion

    #region donwload file
    // exe file download
    public async UniTaskVoid Request_ExeFileDownload()
    {
        await UniTask.SwitchToThreadPool();
        Debug.Log("Request_ExeFileDownload() start()");
        await UniTask.SwitchToMainThread();
    }

    // checksum file donwload
    public async UniTaskVoid Request_ChecksumFileDownload()
    {
        await UniTask.SwitchToThreadPool();
        Debug.Log("Request_ChecksumFileDownload() start()");
        await UniTask.SwitchToMainThread();
    }
    #endregion

    #region event banner & notice
    // event banner
    public async UniTaskVoid Request_EventBanner()
    {
        await UniTask.SwitchToThreadPool();
        Debug.Log("Request_EventBanner() start()");
        await UniTask.SwitchToMainThread();
    }

    // notice
    public async UniTaskVoid Request_Notice()
    {
        await UniTask.SwitchToThreadPool();
        Debug.Log("Request_Notice() start()");
        await UniTask.SwitchToMainThread();
    }

    // curiverse notice
    public async UniTaskVoid Request_CuriverseNotice()
    {
        await UniTask.SwitchToThreadPool();
        Debug.Log("Request_CuriverseNotice() start()");
        await UniTask.SwitchToMainThread();
    }

    public async UniTaskVoid Request_GuideDownload1()
    {
        await UniTask.SwitchToThreadPool();
        Debug.Log("Request_GuideDownload1() start()");
        await UniTask.SwitchToMainThread();
    }

    public async UniTaskVoid Request_GuideDownload2()
    {
        await UniTask.SwitchToThreadPool();
        Debug.Log("Request_GuideDownload2() start()");
        await UniTask.SwitchToMainThread();
    }
    #endregion
}
