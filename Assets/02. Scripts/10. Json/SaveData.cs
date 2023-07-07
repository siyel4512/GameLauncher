using System;
using System.Collections.Generic;

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
    public List<friendList> frndInfoList;
}
