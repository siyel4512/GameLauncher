using System;
using System.Collections.Generic;

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
}
