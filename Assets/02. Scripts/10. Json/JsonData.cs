using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JsonData : MonoBehaviour
{
    public List<SaveData.friendList> temp_friendList_List;
    public List<SaveData.friendList> friendList_List;

    public SaveData.friendList searchFriend = new SaveData.friendList();
    public string searchFriendNum;

    public List<SaveData.friendList> temp_requestFriendListValues;
    public List<SaveData.friendList> requestFriendListValues;

    public SaveData.downloadUrlList temp_donwloadUrl;
    public SaveData.downloadUrlList donwloadUrl;

    public List<SaveData.downloadUrlList> temp_donwloadUrlList;

    public List<SaveData.mainBoard> temp_event_List;
    public List<SaveData.mainBoard> event_List;

    public List<SaveData.mainBoard> temp_shortNotice_List;
    public List<SaveData.mainBoard> shortNotice_List;

    public List<SaveData.mainBoard> temp_news_List;
    public List<SaveData.mainBoard> news_List;

    public List<SaveData.mainBoard> temp_guide_List;
    public List<SaveData.mainBoard> guide_List;

    #region compare to json data (friend list)
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

    #region compare to json data (mainboard)
    public bool CompareToMainBoard(List<SaveData.mainBoard> _list, List<SaveData.mainBoard> _temp_list)
    {
        bool isSame = true;

        // compare to values
        if (_list.Count == _temp_list.Count)
        {
            for (int i = 0; i < _list.Count; i++)
            {
                SaveData.mainBoard list = _list[i];
                SaveData.mainBoard temp_list = _temp_list[i];

                if (list.boardNum != temp_list.boardNum
                    || list.writer != temp_list.writer
                    || list.title != temp_list.title
                    || list.content != temp_list.content
                    || list.webImg != temp_list.webImg
                    || list.lnchrImg != temp_list.lnchrImg
                    || list.boardType != temp_list.boardType
                    || list.openYn != temp_list.openYn
                    || list.exprPeriod != temp_list.exprPeriod
                    || list.upDt != temp_list.upDt
                    || list.regDt != temp_list.regDt
                    || list.boardUrl != temp_list.boardUrl)
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
                _list[i].boardNum = _temp_list[i].boardNum;
                _list[i].writer = _temp_list[i].writer;
                _list[i].title = _temp_list[i].title;
                _list[i].content = _temp_list[i].content;
                _list[i].webImg = _temp_list[i].webImg;
                _list[i].lnchrImg = _temp_list[i].lnchrImg;
                _list[i].boardType = _temp_list[i].boardType;
                _list[i].openYn = _temp_list[i].openYn;
                _list[i].exprPeriod = _temp_list[i].exprPeriod;
                _list[i].upDt = _temp_list[i].upDt;
                _list[i].regDt = _temp_list[i].regDt;
                _list[i].boardUrl = _temp_list[i].boardUrl;
            }
        }

        return isSame;
    }
    #endregion

    #region reset
    public void ResetFriendListJsonData()
    {
        temp_friendList_List = new List<SaveData.friendList>();
        friendList_List = new List<SaveData.friendList>();

        searchFriend = new SaveData.friendList();
        searchFriendNum = "";

        temp_requestFriendListValues = new List<SaveData.friendList>();
        requestFriendListValues = new List<SaveData.friendList>();

        temp_event_List = new List<SaveData.mainBoard>();
        event_List = new List<SaveData.mainBoard>();

        temp_shortNotice_List = new List<SaveData.mainBoard>();
        shortNotice_List = new List<SaveData.mainBoard>();

        temp_news_List = new List<SaveData.mainBoard>();
        news_List = new List<SaveData.mainBoard>();

        temp_guide_List = new List<SaveData.mainBoard>();
        guide_List = new List<SaveData.mainBoard>();
}
    #endregion
}