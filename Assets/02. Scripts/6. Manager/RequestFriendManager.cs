using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Cysharp.Threading.Tasks;
using System;

public class RequestFriendManager : MonoBehaviour
{
    // friend list
    [Header("[ Reqeuset List Settings ]")]
    public RectTransform requestScrollPos;
    public GameObject requestContent;
    public GameObject requestSlot;
    public List<RequestInfo> requestList;
    public int requestfriendCount;

    public bool isUsingAlarmIcon;
    public GameObject[] AlarmIcons;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    #region Set List
    // Todo : 임시 친구 리스트 생성
    //public void CreateRequestList()
    public IEnumerator CreateRequestList()
    {
        yield return null;

        for (int i = 0; i < requestfriendCount; i++)
        {
            List<SaveData.friendList> friendListValues = GameManager.instance.jsonData.friendListValues;
            
            GameObject clone = Instantiate(requestSlot);
            clone.transform.SetParent(requestContent.transform, false);

            // set request info
            RequestInfo info = clone.GetComponent<RequestInfo>();
            info.ncnm = GameManager.instance.jsonData.requestFriendListValues[i].ncnm;
            info.frndNo = GameManager.instance.jsonData.requestFriendListValues[i].frndNo;
            info.mbrNo = GameManager.instance.jsonData.requestFriendListValues[i].mbrNo;
            info.frndMbrNo = GameManager.instance.jsonData.requestFriendListValues[i].frndMbrNo;
            info.frndSttus = GameManager.instance.jsonData.requestFriendListValues[i].frndSttus;
            info.frndRqstSttus = GameManager.instance.jsonData.requestFriendListValues[i].frndRqstSttus;
            info.frndRqstDt = GameManager.instance.jsonData.requestFriendListValues[i].frndRqstDt;
            info.upDt = GameManager.instance.jsonData.requestFriendListValues[i].upDt;
            info.regDt = GameManager.instance.jsonData.requestFriendListValues[i].regDt;
            info.SetSlotValue();

            requestList.Add(clone.GetComponent<RequestInfo>());
        }

        requestScrollPos.anchoredPosition = new Vector2(0, 0);

        // Alarm Icon
        if (isUsingAlarmIcon)
        {
            if (requestfriendCount > 0)
            {
                AlarmIcons[0].SetActive(true);
                AlarmIcons[1].SetActive(true);
            }
            else
            {
                AlarmIcons[0].SetActive(false);
                AlarmIcons[1].SetActive(false);
            }
        }
    }

    public void DeleteRequestList()
    {
        for (int i = 0; i < requestList.Count; i++)
        {
            Destroy(requestList[i].gameObject);
        }

        requestList.Clear();

        AlarmIcons[0].SetActive(false);
        AlarmIcons[1].SetActive(false);
    }
    #endregion

    #region Request
    public void RequestAddList()
    {
        FriendListManager friendListManager = GameManager.instance.friendListManager;

        for (int i = 0; i < requestList.Count; i++)
        {
            if (requestList[i].isRequestComplate)
            {
                friendListManager.temp_friendList.Add(new FriendInfo() 
                { 
                    nickname = requestList[i].GetComponent<RequestInfo>().nickname, 
                    state = requestList[i].GetComponent<RequestInfo>().state,

                    ncnm = requestList[i].GetComponent<RequestInfo>().ncnm,
                    frndNo = requestList[i].GetComponent<RequestInfo>().frndNo,
                    mbrNo = requestList[i].GetComponent<RequestInfo>().mbrNo,
                    frndMbrNo = requestList[i].GetComponent<RequestInfo>().frndMbrNo,
                    frndSttus = requestList[i].GetComponent<RequestInfo>().frndSttus,
                    frndRqstSttus = requestList[i].GetComponent<RequestInfo>().frndRqstSttus,
                    frndRqstDt = requestList[i].GetComponent<RequestInfo>().frndRqstDt,
                    upDt = requestList[i].GetComponent<RequestInfo>().upDt,
                    regDt = requestList[i].GetComponent<RequestInfo>().regDt
            });

                // add request
                GameObject clone = Instantiate(friendListManager.listSlot);
                clone.transform.SetParent(friendListManager.listContent.transform, false);

                FriendInfo friendtInfo = clone.GetComponent<FriendInfo>();

                friendtInfo.nickname = requestList[i].GetComponent<RequestInfo>().nickname;
                //friendtInfo.state = requestList[i].GetComponent<RequestInfo>().state;
                
                friendtInfo.ncnm = requestList[i].GetComponent<RequestInfo>().ncnm;
                friendtInfo.frndNo = requestList[i].GetComponent<RequestInfo>().frndNo;
                friendtInfo.mbrNo = requestList[i].GetComponent<RequestInfo>().mbrNo;
                friendtInfo.frndMbrNo = requestList[i].GetComponent<RequestInfo>().frndMbrNo;
                friendtInfo.frndSttus = requestList[i].GetComponent<RequestInfo>().frndSttus;
                friendtInfo.frndRqstSttus = requestList[i].GetComponent<RequestInfo>().frndRqstSttus;
                friendtInfo.frndRqstDt = requestList[i].GetComponent<RequestInfo>().frndRqstDt;
                friendtInfo.upDt = requestList[i].GetComponent<RequestInfo>().upDt;
                friendtInfo.regDt = requestList[i].GetComponent<RequestInfo>().regDt;

                friendtInfo.SetSlotValues();

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

        // Alarm Icon
        if (isUsingAlarmIcon)
        {
            if (requestList.Count > 0)
            {
                AlarmIcons[0].SetActive(true);
                AlarmIcons[1].SetActive(true);
            }
            else
            {
                AlarmIcons[0].SetActive(false);
                AlarmIcons[1].SetActive(false);
            }
        }
    }
    #endregion
}
