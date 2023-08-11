using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FriendInfo : MonoBehaviour
{
    [Header("[ Info Values ]")]
    public bool isSelected;

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
    public TMP_Text state_text;
    public Image stateIcon;
    public GameObject[] stateIcons;
    public GameObject selectedImage;

    public void SetSlotValues()
    {
        //nickname_text.text = nickname;
        nickname_text.text = ncnm;

        switch (frndSttus)
        {
            case "0":
                state_text.text = "오프라인";
                //stateIcon.color = Color.gray;
                SetStateIcon(3);
                break;
            case "1":
                state_text.text = "온라인";
                //stateIcon.color = Color.green;
                SetStateIcon(0);
                break;
            case "2":
                state_text.text = "자리 비움";
                //stateIcon.color = Color.yellow;
                SetStateIcon(1);
                break;
            case "3":
                state_text.text = "다른 용무 중";
                //stateIcon.color = Color.red;
                SetStateIcon(2);
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

    public void SetStateIcon(int iconNum)
    {
        for (int i = 0; i < stateIcons.Length; i++)
        {
            stateIcons[i].SetActive(false);
        }

        stateIcons[iconNum].SetActive(true);
    }
}
