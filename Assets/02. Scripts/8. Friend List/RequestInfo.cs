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
    public string ncnm;
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

    public void SetSlotValue()
    {
        nickname = ncnm;
        //state = "온라인";

        nickname_text.text = nickname;
    }

    public async void BTN_Accept()
    {
        Debug.Log("요청 수락 / 친구 리스트에 추가 요청");
        isRequestComplate = true;

        //GameManager.instance.requestFriendManager.RequestAddList();

        //// 내 상태 업데이트
        //await GameManager.instance.api.Update_PlayerState(GameManager.instance.playerManager.currentState, Login.PID);

        // 친구 리스트 추가 요청
        await GameManager.instance.api.Request_Accept(mbrNo, frndMbrNo);

        // 요청 리스트 갱신
        GameManager.instance.api.Request_RequestFriendList().Forget();

        // 친구 리스트 갱신
        GameManager.instance.api.Request_FriendList(true).Forget();
    }

    public async void BTN_Refuse()
    {
        Debug.Log("요청 거절");
        isRequestComplate = true;

        //GameManager.instance.requestFriendManager.DeleteList();

        // 요청 삭제 요청
        await GameManager.instance.api.Request_RefuseNDelete(mbrNo, frndMbrNo);

        // 요청 리스트 갱신
        GameManager.instance.api.Request_RequestFriendList().Forget();
    }
}
