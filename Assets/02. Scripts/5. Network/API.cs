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

    private async UniTask<List<SaveData.friendList>> Request_FriendList()
    {
        List<SaveData.friendList> data = new List<SaveData.friendList>();

        HttpClient client = new HttpClient();
        var response = await client.GetAsync(friendList);
        string requestResult = await response.Content.ReadAsStringAsync();

        if (response.IsSuccessStatusCode)
        {
            Debug.Log("ģ�� ����Ʈ : " + requestResult);
            data = JsonUtility.FromJson<SaveData>(requestResult).frndInfoList;
        }
        else
        {
            Debug.Log("���� ���� (ģ�� ����Ʈ) : " + requestResult);
        }

        return data;
    }
}
