using Ookii.Dialogs;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using static SaveData;

public class JsonData : MonoBehaviour
{
    //public API testApi;
    public List<SaveData.friendList> temp_friendListValue;
    public List<SaveData.friendList> friendListValues;

    // Todo : data type
    //public List<SaveData.requestFriendList> temp_requestFriendListValues;
    //public List<SaveData.requestFriendList> requestFriendListValues;
    public List<SaveData.friendList> temp_requestFriendListValues;
    public List<SaveData.friendList> requestFriendListValues;

    public List<SaveData.friendList> frndInfoList1;
    public List<SaveData.friendList> frndInfoList2;

    // Start is called before the first frame update
    void Start()
    {
        //temp_friendListValue = friendListValues;

        //Test_SetJsonData();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            //temp_friendListValue.Clear();
            //temp_friendListValue = null;

            CompareToFriendList(frndInfoList1, frndInfoList2);
        }
    }

    #region Test
    // Todo : 삭제 예정
    public void Test_SetJsonData()
    {
        for (int i = 0; i < 3; i++)
        {
            frndInfoList1[i].ncnm = "배트맨";
            frndInfoList1[i].frndNo = 1;
            frndInfoList1[i].mbrNo = 1;
            frndInfoList1[i].frndMbrNo = 1;
            frndInfoList1[i].frndSttus = "1";
            frndInfoList1[i].frndRqstSttus = "2";
            frndInfoList1[i].frndRqstDt = "2023-06-22 00:00:00.0";
            frndInfoList1[i].upDt = "2023-06-22 00:00:00.0";
            frndInfoList1[i].regDt = "2023-06-22 00:00:00.0";
        }

        for (int i = 0; i < 3; i++)
        {
            frndInfoList2[i].ncnm = "슈퍼맨";
            frndInfoList2[i].frndNo = 1;
            frndInfoList2[i].mbrNo = 1;
            frndInfoList2[i].frndMbrNo = 1;
            frndInfoList2[i].frndSttus = "1";
            frndInfoList2[i].frndRqstSttus = "2";
            frndInfoList2[i].frndRqstDt = "2023-06-22 00:00:00.0";
            frndInfoList2[i].upDt = "2023-06-22 00:00:00.0";
            frndInfoList2[i].regDt = "2023-06-22 00:00:00.0";
        }
    }

    public void Test_CompareToFriendList()
    {
        bool isSame = true;

        if (frndInfoList1.Count == frndInfoList2.Count)
        {
            for (int i = 0; i < frndInfoList1.Count; i++)
            {
                SaveData.friendList friend1 = frndInfoList1[i];
                SaveData.friendList friend2 = frndInfoList2[i];

                // 각 필드 값을 비교합니다.
                if (friend1.ncnm != friend2.ncnm
                    || friend1.frndNo != friend2.frndNo
                    || friend1.mbrNo != friend2.mbrNo
                    || friend1.frndMbrNo != friend2.frndMbrNo
                    || friend1.frndSttus != friend2.frndSttus
                    || friend1.frndRqstSttus != friend2.frndRqstSttus
                    || friend1.frndRqstDt != friend2.frndRqstDt
                    || friend1.upDt != friend2.upDt
                    || friend1.regDt != friend2.regDt)
                {
                    isSame = false;
                    break;
                }
            }
        }
        else
        {
            isSame = false;
        }

        Debug.Log("[SY] : " + isSame);
    }
    #endregion

    #region compare to json data
    // compare to friend & request list
    public bool CompareToFriendList(List<SaveData.friendList> _list, List<SaveData.friendList> _temp_list)
    {
        bool isSame = true;

        // compare to values
        if (_list.Count == _temp_list.Count)
        {
            for (int i = 0; i < _list.Count; i++)
            {
                SaveData.friendList list = _list[i];
                SaveData.friendList temp_list = _temp_list[i];

                if (list.ncnm != temp_list.ncnm
                    || list.frndNo != temp_list.frndNo
                    || list.mbrNo != temp_list.mbrNo
                    || list.frndMbrNo != temp_list.frndMbrNo
                    || list.frndSttus != temp_list.frndSttus
                    || list.frndRqstSttus != temp_list.frndRqstSttus
                    || list.frndRqstDt != temp_list.frndRqstDt
                    || list.upDt != temp_list.upDt
                    || list.regDt != temp_list.regDt)
                {
                    isSame = false;
                    break;
                }
            }
        }
        else
        {
            isSame = false;
        }

        // set current value
        if (!isSame)
        {
            for (int i = 0; i < _temp_list.Count; i++)
            {
                _list[i].ncnm = _temp_list[i].ncnm;
                _list[i].frndNo = _temp_list[i].frndNo;
                _list[i].mbrNo = _temp_list[i].mbrNo;
                _list[i].frndMbrNo = _temp_list[i].frndMbrNo;
                _list[i].frndSttus = _temp_list[i].frndSttus;
                _list[i].frndRqstSttus = _temp_list[i].frndRqstSttus;
                _list[i].frndRqstDt = _temp_list[i].frndRqstDt;
                _list[i].upDt = _temp_list[i].upDt;
                _list[i].regDt = _temp_list[i].regDt;
            }
        }

        return isSame;
    }
    #endregion 
}