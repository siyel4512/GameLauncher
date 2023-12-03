using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Localization.Settings;

public class FriendInfo : MonoBehaviour
{
    [Header("[ Info Values ]")]
    public bool isSelected;

    // json 값 저정
    public string id;
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
        nickname_text.text = id;

        switch (frndSttus)
        {
            case "0":
                // Offline
                state_text.text = LocalizationSettings.StringDatabase.GetLocalizedString("Player State Table", "offline");
                SetStateIcon(3);
                break;
            case "1":
                // Online
                state_text.text = LocalizationSettings.StringDatabase.GetLocalizedString("Player State Table", "online button");
                SetStateIcon(0);
                break;
            case "2":
                // Take a Break
                state_text.text = LocalizationSettings.StringDatabase.GetLocalizedString("Player State Table", "take a break button");
                SetStateIcon(1);
                break;
            case "3":
                // Other Work
                state_text.text = LocalizationSettings.StringDatabase.GetLocalizedString("Player State Table", "other work button");
                SetStateIcon(2);
                break;
        }
    }

    // select this slot
    public void SelectSlot()
    {
        GameManager.instance.friendListManager.ResetSelect();
        GameManager.instance.friendListManager.isSelectedSlot = true;
        isSelected = true;
        selectedImage.SetActive(true);
    }

    // set select state icon
    public void SetStateIcon(int iconNum)
    {
        for (int i = 0; i < stateIcons.Length; i++)
        {
            stateIcons[i].SetActive(false);
        }

        stateIcons[iconNum].SetActive(true);
    }
}
