using System;
using System.Collections.Generic;
using UnityEditor;

//[System.Serializable]
public class SaveData
{
    [Serializable]
    public class friendList
    {
        public string ncnm;
        public int frndNo;
        public int mbrNo;
        public int frndMbrNo;
        public string frndSttus;
        public string frndRqstSttus;
        public string frndRqstDt;
        public string upDt;
        public string regDt;
    }
    public List<friendList> frndInfoList;

    [Serializable]
    public class requestFriendList
    {
        public string ncnm;
        public int frndNo;
        public int mbrNo;
        public int frndMbrNo;
        public string frndSttus;
        public string frndRqstSttus;
        public string frndRqstDt;
        public string upDt;
        public string regDt;
    }
    public List<requestFriendList> requestFriend_List;

    [Serializable]
    public class mainBoard
    {
        public int boardNum;
        public string writer;
        public string title;
        public string content;
        public string webImg;
        public string lnchrImg;
        public string boardType;
        public string openYn;
        public string exprPeriod;
        public string regDt;
        public string upDt;
        public string boardUrl;
    }
    public List<mainBoard> mainboardlist;

    [Serializable]
    public class downloadUrlList
    {
        public string zip_path;
        public string json_path;
    }
    public downloadUrlList downloadUrl;
}
