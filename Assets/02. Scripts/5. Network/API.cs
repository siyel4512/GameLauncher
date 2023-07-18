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
    //private async UniTask<List<SaveData.friendList>> Request_FriendList()
    public async UniTask<List<SaveData.friendList>> Request_FriendList()
    {
        await UniTask.SwitchToThreadPool();

        Debug.Log("Request_FriendList() start()");
        List<SaveData.friendList> data = new List<SaveData.friendList>();
        JsonData jsonData = GameManager.instance.jsonData;

        var param = new Dictionary<string, string>
        {
            { "token", Login.PID },
            { "ncnm", "" }
        };

        var content = new FormUrlEncodedContent(param);
        
        //HttpContent content = new StringContent("", System.Text.Encoding.UTF8);

        HttpClient client = new HttpClient();
        var response = await client.PostAsync("http://101.101.218.135:5002/onlineScienceMuseumAPI/frndInfo.do", content);
        //var response = await client.PostAsync("http://101.101.218.135:5002/authMngr/login.do", content);
        string requestResult = await response.Content.ReadAsStringAsync();

        if (response.IsSuccessStatusCode)
        {
            //Debug.Log("친구 리스트 : " + requestResult);
            Debug.Log("응답 성공");
            jsonData.friendListValues = JsonUtility.FromJson<SaveData>(requestResult).frndInfoList; // temp data save
        }
        else
        {
            Debug.Log("응답 실패 (친구 리스트 : " + requestResult);
        }

        // compare data
        bool isCompareResult = jsonData.CompareToFriendList(jsonData.friendListValues, jsonData.temp_friendListValue);

        Debug.Log("[SY] : " + isCompareResult);
        
        await UniTask.SwitchToMainThread();

        if (!isCompareResult)
        {
            FriendListManager friendListManager = GameManager.instance.friendListManager;

            // delete date
            friendListManager.DeleteList();

            if (isTEST)
            {
                // create data
                friendListManager.CreateList(); // Todo : Change Coroutine
            }
        }

        return data;
    }

    // request list
    private async UniTask<List<SaveData.requestFriendList>> Request_RequestFriendList()
    {
        await UniTask.SwitchToThreadPool();
        List<SaveData.requestFriendList> data = new List<SaveData.requestFriendList>();

        HttpClient client = new HttpClient();
        var response = await client.GetAsync(friendList);
        string requestResult = await response.Content.ReadAsStringAsync();

        if (response.IsSuccessStatusCode)
        {
            Debug.Log("친구 요청 리스트 : " + requestResult);
            data = JsonUtility.FromJson<SaveData>(requestResult).requestFriend_List;
        }
        else
        {
            Debug.Log("응답 실패 (친구 요청 리스트 : " + requestResult);
        }

        JsonData jsonData = GameManager.instance.jsonData;

        // compare data
        bool isCompareResult = jsonData.CompareToFriendList(jsonData.friendListValues, jsonData.temp_friendListValue);

        await UniTask.SwitchToMainThread();

        if (!isCompareResult)
        {
            FriendListManager friendListManager = GameManager.instance.friendListManager;

            // delete date
            friendListManager.DeleteList();

            // create data
            friendListManager.CreateList(); // Todo : Change Coroutine
        }

        return data;
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
