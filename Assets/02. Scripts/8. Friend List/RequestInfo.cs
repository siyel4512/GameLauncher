using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using TMPro;

public class RequestInfo : MonoBehaviour
{
    [Header("[ Info Values ]")]
    public string nickname;
    public string state;
    public bool isRequestComplate;

    // json 값 저정
    public int frndNo;
    public int mbrNo;
    public int frndMbrNo;
    public string frndSttus;
    public string frndRqstSttus;
    public string frndRqstDt;
    public string upDt;
    public string regDt;
    
    [Space(10)]
    [Header("[ UI ]")]
    public TMP_Text nickname_text;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Test_SetSlotValue(int _index)
    {
        nickname = $"Request_" + _index;
        state = "온라인";

        nickname_text.text = nickname;
    }

    public void BTN_Accept()
    {
        Debug.Log("요청 수락 / 친구 리스트에 추가 요청");
        isRequestComplate = true;

        GameManager.instance.requestFriendManager.RequestAddList();
    }

    public void BTN_Refuse()
    {
        Debug.Log("요청 거절");
        isRequestComplate = true;

        GameManager.instance.requestFriendManager.DeleteList();
    }
}
