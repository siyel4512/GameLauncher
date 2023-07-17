using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestAPI : MonoBehaviour
{
    public API api;
    public List<SaveData.friendList> friendListValues;
    public List<SaveData.requestFriendList> requestFriendListValues;

    // Start is called before the first frame update
    void Start()
    {
        api = GetComponent<API>();

        friendListValues = JsonUtility.FromJson<SaveData>(api.friendList).friend_List;
        requestFriendListValues = JsonUtility.FromJson<SaveData>(api.requestFriendList).requestFriend_List;

        //Debug.Log($"{friendListValues.Count} / {requestFriendListValues.Count}");

        //for (int i = 0; i < friendListValues.Count; i++)
        //{
        //    Debug.Log();
        //}
    }

    //// Update is called once per frame
    //void Update()
    //{
    //    if (Input.GetKeyDown(KeyCode.Space))
    //    {
    //        friendListValues = JsonUtility.FromJson<SaveData>(api.friendList).friend_List;
    //        Debug.Log($"json 파일 변환 : {friendListValues.Count}");

    //    }
    //}
}
