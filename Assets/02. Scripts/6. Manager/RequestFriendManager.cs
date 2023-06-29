using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Reflection;
//using static SaveData;

public class RequestFriendManager : MonoBehaviour
{
    // friend list
    public RectTransform requestScrollPos;
    public GameObject requestContent;
    public GameObject requestSlot;
    public List<RequestInfo> requestList;
    //public bool isSelectedSlot;

    // Start is called before the first frame update
    void Start()
    {
        CreateRequestList();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    #region Set List
    // Todo : 임시 친구 리스트 생성
    private void CreateRequestList()
    {
        for (int i = 0; i < 100; i++)
        {
            GameObject clone = Instantiate(requestSlot);
            clone.transform.SetParent(requestContent.transform, false);
            clone.GetComponent<RequestInfo>().Test_SetSlotValue(i);
            requestList.Add(clone.GetComponent<RequestInfo>());
        }

        requestScrollPos.anchoredPosition = new Vector2(0, 0);
    }
    #endregion

    #region Request
    public void RequestAddList()
    {
        FrientListManager friendListManager = GameManager.instance.friendListManager;

        for (int i = 0; i < requestList.Count; i++)
        {
            if (requestList[i].isRequestComplate)
            {
                friendListManager.temp_friendList.Add(new FriendInfo() { nickname = requestList[i].GetComponent<RequestInfo>().nickname, state = requestList[i].GetComponent<RequestInfo>().state });

                // add request
                GameObject clone = Instantiate(friendListManager.listSlot);
                clone.transform.SetParent(friendListManager.listContent.transform, false);

                clone.GetComponent<FriendInfo>().nickname = requestList[i].GetComponent<RequestInfo>().nickname;
                clone.GetComponent<FriendInfo>().state = requestList[i].GetComponent<RequestInfo>().state;
                clone.GetComponent<FriendInfo>().SetSlotValues();

                friendListManager.friendList.Add(clone.GetComponent<FriendInfo>());

                // delete
                Destroy(requestList[i].gameObject);
                requestList.RemoveAt(i);
                break;
            }
        }

        // ascending order sort
        friendListManager.temp_friendList = friendListManager.temp_friendList.OrderBy(x => x.nickname).ToList();

        // descending order sort
        //friendListManager.friendList = friendListManager.friendList.OrderByDescending(x => x.nickname).ToList();

        // sort slots
        for (int i = 0; i < friendListManager.friendList.Count; i++)
        {
            FriendInfo friendInfo = friendListManager.listContent.transform.GetChild(i).GetComponent<FriendInfo>();
            friendInfo.nickname = friendListManager.temp_friendList[i].nickname;
            friendInfo.state = friendListManager.temp_friendList[i].state;
            friendInfo.SetSlotValues();
        }
    }

    public void DeleteList()
    {
        for (int i = 0; i < requestList.Count; i++)
        {
            if (requestList[i].isRequestComplate)
            {
                Destroy(requestList[i].gameObject);
                requestList.RemoveAt(i);
                break;
            }
        }
    }
    #endregion
}
