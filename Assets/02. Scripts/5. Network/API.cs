using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Net.Http;
using UnityEngine;

public class API : URL
{
    public static API instance;

    public bool isTEST;

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
            Debug.Log("친구 리스트 : " + requestResult);
            jsonData.temp_friendListValue = JsonUtility.FromJson<SaveData>(requestResult).frndInfoList; // temp data save
        }
        else
        {
            Debug.Log("응답 실패 (친구 리스트 : " + requestResult);
        }

        await UniTask.SwitchToMainThread();

        // set friend list count
        friendListManager.friendCount = jsonData.temp_friendListValue.Count;

        // frist time setting
        if (jsonData.friendListValues.Count == 0)
        {
            //Debug.Log("값 없음");
            jsonData.friendListValues = JsonUtility.FromJson<SaveData>(requestResult).frndInfoList;

            // create data
            StartCoroutine(friendListManager.CreateList());
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

                //if (isTEST)
                {
                    // create data
                    StartCoroutine(friendListManager.CreateList());
                }
            }
        }
    }

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
            Debug.Log("친구 요청 리스트 : " + requestResult);
            jsonData.temp_requestFriendListValues = JsonUtility.FromJson<SaveData>(requestResult).frndInfoList; // temp data save
        }
        else
        {
            Debug.Log("응답 실패 (친구 리스트 : " + requestResult);
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

                //if (isTEST)
                {
                    // create data
                    StartCoroutine(requestFriendManager.CreateRequestList());
                }
            }
        }
    }

    // add friend

    // delete friend

    #endregion

    #region player state
    // upudate player state

    #endregion

    #region donwload file
    // exe file download

    // checksum file donwload

    #endregion

    #region event banner & notice
    // event banner

    // notice

    // curiverse notice

    #endregion
}
