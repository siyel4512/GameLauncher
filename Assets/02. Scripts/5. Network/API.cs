using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using Unity.VisualScripting.Antlr3.Runtime;
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
    //private async UniTask<List<SaveData.friendList>> Request_FriendList()
    public async UniTask<List<SaveData.friendList>> Request_FriendList()
    {
        Debug.Log("Request_FriendList() start()");

        List<SaveData.friendList> data = new List<SaveData.friendList>();

        HttpContent content = new StringContent("", System.Text.Encoding.UTF8);

        HttpClient client = new HttpClient();
        //var response = await client.GetAsync(friendList);
        //var response = await client.GetAsync("http://101.101.218.135:5002/onlineScienceMuseumAPI/frndInfo.do");
        //var response = await client.PostAsync("http://101.101.218.135:5002/onlineScienceMuseumAPI/frndInfo.do", content);
        var response = await client.PostAsync("http://101.101.218.135:5002/authMngr/login.do", content);
        string requestResult = await response.Content.ReadAsStringAsync();

        Debug.Log(response.RequestMessage);

        if (response.IsSuccessStatusCode)
        {
            Debug.Log("친구 리스트 : " + requestResult);
            data = JsonUtility.FromJson<SaveData>(requestResult).frndInfoList;
        }
        else
        {
            Debug.Log("응답 실패 (친구 리스트 : " + requestResult);
        }

        return data;
    }

    // request list
    private async UniTask<List<SaveData.requestFriendList>> Request_RequestFriendList()
    {
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
