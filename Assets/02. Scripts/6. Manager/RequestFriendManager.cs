using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static SaveData;

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
        for (int i = 0; i < requestList.Count; i++)
        {
            if (requestList[i].isRequestComplate)
            {
                FrientListManager friendListManager = GameManager.instance.friendListManager;
                
                // 추가 요청
                GameObject clone = Instantiate(friendListManager.listSlot);
                clone.transform.SetParent(friendListManager.listContent.transform, false);

                clone.GetComponent<FriendInfo>().nickname = requestList[i].GetComponent<RequestInfo>().nickname;
                clone.GetComponent<FriendInfo>().state = requestList[i].GetComponent<RequestInfo>().state;
                clone.GetComponent<FriendInfo>().SetSlotValues();

                friendListManager.friendList.Add(clone.GetComponent<FriendInfo>());

                // 삭제
                Destroy(requestList[i].gameObject);
                requestList.RemoveAt(i);
                break;
            }
        };
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
        };
    }
    #endregion
}
