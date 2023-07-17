using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FriendInfo : MonoBehaviour
{
    [Header("[ Info Values ]")]
    public string nickname;
    public string state;
    public bool isSelected;

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
    public TMP_Text state_text;
    public Image stateIcon;
    public GameObject selectedImage;

    // Start is called before the first frame update
    void Start()
    {
    
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //// Todo : 친구 닉네임, 접속 상태 적용
    //public void Test_SetSlotValue(int _index)
    //{
    //    nickname = $"Test_" + _index;
    //    state = "온라인";

    //    nickname_text.text = nickname;
    //    state_text.text = state;

    //    // set state icon
    //    switch (state)
    //    {
    //        case "온라인":
    //            stateIcon.color = Color.green;
    //            break;
    //        case "자리 비움":
    //            stateIcon.color = Color.yellow;
    //            break;
    //        case "다른 용무 중":
    //            stateIcon.color = Color.red;
    //            break;
    //        case "오프라인":
    //            stateIcon.color = Color.gray;
    //            break;
    //    }
    //}

    public void SetSlotValues()
    {
        nickname_text.text = nickname;
        state_text.text = state;

        // set state icon
        switch (state)
        {
            case "온라인":
                stateIcon.color = Color.green;
                break;
            case "자리 비움":
                stateIcon.color = Color.yellow;
                break;
            case "다른 용무 중":
                stateIcon.color = Color.red;
                break;
            case "오프라인":
                stateIcon.color = Color.gray;
                break;
        }
    }

    public void SelectSlot()
    {
        GameManager.instance.friendListManager.ResetSelect();
        GameManager.instance.friendListManager.isSelectedSlot = true;
        isSelected = true;
        selectedImage.SetActive(true);
    }
}
