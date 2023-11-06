using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Tables;
using UnityEngine.ResourceManagement.AsyncOperations;

public class LanguageManager : MonoBehaviour
{
    public int currentLanguageNum = 0;

    public PlayerManager playerManager;
    public FriendListManager friendListManager;

    public TMP_Dropdown selectServer;

    // Start is called before the first frame update
    void Start()
    {
        playerManager = GameManager.instance.playerManager;
        friendListManager = GameManager.instance.friendListManager;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log(LocalizationSettings.StringDatabase.GetLocalizedString("Player State Table", "online button"));
        }
    }

    bool isChange = false;

    public void ChangeLanguage(int index)
    {
        if (isChange)
            return;

        StartCoroutine(Co_ChangeLanguage(index));
    }

    IEnumerator Co_ChangeLanguage(int index)
    {
        isChange = true;
        currentLanguageNum = index;

        yield return LocalizationSettings.InitializationOperation;
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[index];

        isChange = false;

        var localizationLoadOperation = LocalizationSettings.InitializationOperation;

        while (!localizationLoadOperation.IsDone)
        {
            yield return null;
        }

        SetPlayerState();
        SetFriendListState();
        SetAddFriendWarningText();
        SetSelectServerDropdown();
    }

    private void SetPlayerState()
    {
        Debug.Log(LocalizationSettings.StringDatabase.GetLocalizedString("Player State Table", "online button"));

        switch (playerManager.currentState)
        {
            case 1:
                // online
                playerManager.stateName.text = LocalizationSettings.StringDatabase.GetLocalizedString("Player State Table", "online button");
                break;
            case 2:
                // Take a Break
                playerManager.stateName.text = LocalizationSettings.StringDatabase.GetLocalizedString("Player State Table", "take a break button");
                break;
            case 3:
                // Other Work
                playerManager.stateName.text = LocalizationSettings.StringDatabase.GetLocalizedString("Player State Table", "other work button");
                break;
        }
    }

    private void SetFriendListState()
    {
        if (friendListManager.friendList.Count <= 0)
            return;

        for (int i = 0; i < friendListManager.friendList.Count; i++)
        {
            friendListManager.friendList[i].SetSlotValues();
        }
    }

    private void SetAddFriendWarningText()
    {
        switch (friendListManager.currentWarningTextNum)
        {
            case 1:
                friendListManager.searchUserWaringText.text = LocalizationSettings.StringDatabase.GetLocalizedString("Friend List Table", "search user waring text 1");
                break;
            case 2:
                friendListManager.searchUserWaringText.text = LocalizationSettings.StringDatabase.GetLocalizedString("Friend List Table", "search user waring text 2");
                break;
            case 3:
                friendListManager.searchUserWaringText.text = LocalizationSettings.StringDatabase.GetLocalizedString("Friend List Table", "search user waring text 3");
                break;
            case 4:
                friendListManager.searchUserWaringText.text = LocalizationSettings.StringDatabase.GetLocalizedString("Friend List Table", "search user waring text 4");
                break;
            case 5:
                friendListManager.searchUserWaringText.text = LocalizationSettings.StringDatabase.GetLocalizedString("Friend List Table", "search user waring text 5");
                break;
        }
    }


    private void SetSelectServerDropdown()
    {
        for (int i = 0; i < selectServer.options.Count; i++)
        {
            switch (i)
            {
                case 0:
                    selectServer.options[i].text = LocalizationSettings.StringDatabase.GetLocalizedString("Login Table", "dropdown-Dev server");
                    break;
                case 1:
                    selectServer.options[i].text = LocalizationSettings.StringDatabase.GetLocalizedString("Login Table", "dropdown-Test server");
                    break;
                case 2:
                    selectServer.options[i].text = LocalizationSettings.StringDatabase.GetLocalizedString("Login Table", "dropdown-Staging server");
                    break;
                case 3:
                    selectServer.options[i].text = LocalizationSettings.StringDatabase.GetLocalizedString("Login Table", "dropdown-Live server");
                    break;
            }
            
        }

        selectServer.captionText.text = selectServer.options[selectServer.value].text;
    }
}
