using System;
using System.Collections.Generic;

//[System.Serializable]
public class SaveData
{
    [Serializable]
    public class friendList
    {
        public int frndNo;
        public int mbrNo;
        public int frndMbrNo;
        public string frndSttus;
        public string frndRqstSttus;
        public string frndRqstDt;
        public string upDt;
        public string regDt;
    }
    public List<friendList> friend_List;

    [Serializable]
    public class requestFriendList
    {
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
