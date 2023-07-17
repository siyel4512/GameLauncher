using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using static SaveData;

public class JsonData : MonoBehaviour
{
    //public API testApi;
    public List<SaveData.friendList> temp_friendListValue;
    public List<SaveData.friendList> friendListValues;

    public List<SaveData.requestFriendList> temp_requestFriendListValues;
    public List<SaveData.requestFriendList> requestFriendListValues;

    public List<SaveData.friendList> frndInfoList1;
    public List<SaveData.friendList> frndInfoList2;

    // Start is called before the first frame update
    void Start()
    {
        temp_friendListValue = friendListValues;

        for (int i = 0; i < 3; i++)
        {
            //frndInfoList1[i].c;
            //ncnm = friendListValues[i].ncnm,
            //frndNo = friendListValues[i].frndNo,
            //mbrNo = friendListValues[i].mbrNo,
            //frndMbrNo = friendListValues[i].frndMbrNo,
            //frndSttus = friendListValues[i].frndSttus,
            //frndRqstSttus = friendListValues[i].frndRqstSttus,
            //frndRqstDt = friendListValues[i].frndRqstDt,
            //upDt = friendListValues[i].upDt,
            //regDt = friendListValues[i].regDt
        }

        for (int j = 0; j < frndInfoList2.Count; j++)
        {

        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            temp_friendListValue.Clear();
            //temp_friendListValue = null;
        }
    }

    // compare data
    public void Test1()
    {
        if (temp_friendListValue.Count == friendListValues.Count)
        {
            for (int i = 0; i < temp_friendListValue.Count; i++)
            {

            }
        }
        else
        {
            friendListValues = temp_friendListValue;
        }
    }

    public void Test2()
    {

    }
}

//using System.Linq;

//List<int> list1 = new List<int>() { 1, 2, 3, 4, 5 };
//List<int> list2 = new List<int>() { 1, 2, 3, 6, 7 };

//bool isSame = list1.SequenceEqual(list2);

//if (isSame)
//{
//    Console.WriteLine("Lists are the same.");
//}
//else
//{
//    Console.WriteLine("Lists are different.");
//}


//bool isSame = true;

//if (frndInfoList1.Count == frndInfoList2.Count)
//{
//    for (int i = 0; i < frndInfoList1.Count; i++)
//    {
//        friendList friend1 = frndInfoList1[i];
//        friendList friend2 = frndInfoList2[i];

//        // 각 필드 값을 비교합니다.
//        if (friend1.ncnm != friend2.ncnm
//            || friend1.frndNo != friend2.frndNo
//            || friend1.mbrNo != friend2.mbrNo
//            || friend1.frndMbrNo != friend2.frndMbrNo
//            || friend1.frndSttus != friend2.frndSttus
//            || friend1.frndRqstSttus != friend2.frndRqstSttus
//            || friend1.frndRqstDt != friend2.frndRqstDt
//            || friend1.upDt != friend2.upDt
//            || friend1.regDt != friend2.regDt)
//        {
//            isSame = false;
//            break;
//        }
//    }
//}
//else
//{
//    isSame = false;
//}

//bool isSame = frndInfoList1.OrderBy(x => x.frndNo)
//                          .SequenceEqual(frndInfoList2.OrderBy(x => x.frndNo));