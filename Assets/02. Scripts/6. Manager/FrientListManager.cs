using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Net.Http;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
//using System.Threading.Tasks;

public class FrientListManager : MonoBehaviour
{
    // main buttons
    [Space(10)]
    [Header("[ Main Buttons ]")]
    public Button addButton;
    public Button settingButton;

    // input field
    [Header("[ Friend Searching ]")]
    public TMP_InputField searchFriendNickName;
    public TMP_InputField searchUserNickName;

    // setting menu
    [Space(10)]
    [Header("[ Setting Menu ]")]
    public GameObject settingMenu;
    public bool isFriendSettings;

    // friend list
    [Space(10)]
    [Header("[ Friedn List ]")]
    public RectTransform listScrollPos;
    public GameObject listContent;
    public GameObject listSlot;
    public List<FriendInfo> friendList;
    public List<FriendInfo> temp_friendList;
    public bool isSelectedSlot;
    public int friendCount;

    // friend request list
    [Space(10)]
    [Header("[ Request List ]")]
    public RectTransform requestListScrollPos;

    // Start is called before the first frame update
    void Start()
    {
        searchFriendNickName.onSubmit.AddListener(InputEnter);
        searchFriendNickName.onValueChanged.AddListener(CheckedNickName);

        searchUserNickName.onSubmit.AddListener(TrySearchUser);

        addButton.onClick.AddListener(TryAddFriend);
        settingButton.onClick.AddListener(ShowSettingMenu);

        listScrollPos.anchoredPosition = new Vector2(0, 0);
        requestListScrollPos.anchoredPosition = new Vector2(0, 0);
    }

    #region Set List
    public void ResetSelect()
    {
        isSelectedSlot = false;

        for (int i = 0; i < friendList.Count; i++)
        {
            friendList[i].selectedImage.SetActive(false);
            friendList[i].isSelected = false;
        }
    }

    // Todo : 임시 친구 리스트 생성
    public void CreateList()
    {
        for (int i = 0; i < friendCount; i++)
        {
            temp_friendList.Add(new FriendInfo() { nickname = $"Test_" + i, state = "온라인" });

            GameObject clone = Instantiate(listSlot);
            clone.transform.SetParent(listContent.transform, false);

            // set friend info
            FriendInfo info = clone.GetComponent<FriendInfo>();
            info.nickname = temp_friendList[i].nickname;
            info.state = temp_friendList[i].state;
            info.frndNo = GameManager.instance.jsonData.friendListValues[i].frndNo;
            info.mbrNo = GameManager.instance.jsonData.friendListValues[i].mbrNo;
            info.frndMbrNo = GameManager.instance.jsonData.friendListValues[i].frndMbrNo;
            info.frndSttus = GameManager.instance.jsonData.friendListValues[i].frndSttus;
            info.frndRqstSttus = GameManager.instance.jsonData.friendListValues[i].frndRqstSttus;
            info.frndRqstDt = GameManager.instance.jsonData.friendListValues[i].frndRqstDt;
            info.upDt = GameManager.instance.jsonData.friendListValues[i].upDt;
            info.regDt = GameManager.instance.jsonData.friendListValues[i].regDt;
            info.SetSlotValues();

            friendList.Add(clone.GetComponent<FriendInfo>());
        }

        listScrollPos.anchoredPosition = new Vector2(0, 0);
    }

    public void DeleteList()
    {
        for (int i = 0; i < friendList.Count; i++)
        {
            Destroy(friendList[i].gameObject);
        }

        friendList.Clear();
        temp_friendList.Clear();
    }
    #endregion

    #region Search Friend
    public void CheckedNickName(string text)
    {
        isSelectedSlot = false;

        for (int i = 0; i < friendList.Count; i++)
        {
            friendList[i].selectedImage.SetActive(false);
            friendList[i].isSelected = false;

            string nickname = friendList[i].nickname_text.text;

            // contains nickname
            if (nickname.Contains(text))
            {
                friendList[i].gameObject.SetActive(true);
            }
            // not contains
            else
            {
                friendList[i].gameObject.SetActive(false);
            }
        }
    }

    private void InputEnter(string text)
    {
        Debug.Log("try search my friend : " + text);

        // blank
        if (searchFriendNickName.text == "")
        {
            GameManager.instance.popupManager.popups[(int)PopupType.BlankError].SetActive(true);
        }
    }
    #endregion

    #region Add Friend
    public void TryAddFriend()
    {
        Debug.Log("친구 추가 시도");
        ResetSelect();
        GameManager.instance.popupManager.popups[(int)PopupType.UserSearch].SetActive(true);
    }

    public void TrySearchUser(string text)
    {
        //Debug.Log("try search user : " + text);
        SearchUser();
    }

    public async void SearchUser()
    {
        // Requset
        // Todo : delete GameManager.instance.isTEST
        if (DEV.instance.isTEST)
        {
            // find
            if (searchUserNickName.text == "test")
            {
                Debug.Log("Success find user : " + searchUserNickName.text);
                GameManager.instance.popupManager.SetContents(1, searchUserNickName.text);
                GameManager.instance.popupManager.popups[(int)PopupType.RequestFriend].SetActive(true);
                searchUserNickName.text = "";
            }
            // blank
            else if (searchUserNickName.text == "")
            {
                GameManager.instance.popupManager.popups[(int)PopupType.BlankError].SetActive(true);
            }
            // not find
            else
            {
                Debug.Log("Failed find user : " + searchUserNickName.text);
                GameManager.instance.popupManager.popups[(int)PopupType.UserSearchFaild].SetActive(true);
            }
        }
        else
        {
            // Todo : select uniyWebRequest or httpClient
            var param = new Dictionary<string, string>
            {
                { "dd", "dd" }
            };

            var content = new FormUrlEncodedContent(param);

            HttpClient client = new HttpClient();

            var response = await client.PostAsync(URL.Instance.GetKeyUrl, content);
            string requestResult = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                //await TryLogin(requestResult);
            }
            else
            {
                Debug.Log("응답 실패 (키값 받아오기) : " + requestResult);
            }
        }
    }
    #endregion

    #region Delete Friend
   
    public void ShowSettingMenu()
    {
        if (!isFriendSettings)
        {
            isFriendSettings = true;
            Debug.Log("Show Setting menu");
            settingMenu.SetActive(isFriendSettings);
        }
        else
        {
            isFriendSettings = false;
            Debug.Log("Hide Setting menu");
            settingMenu.SetActive(isFriendSettings);
        }
    }

    public void TryDeleteFriend()
    {
        Debug.Log("친구 삭제 시도");
        isFriendSettings = false;
        settingMenu.SetActive(isFriendSettings);

        // slot이 선택되어 있는지 확인하기
        if (isSelectedSlot)
        {
            GameManager.instance.popupManager.popups[(int)PopupType.DeleteFriend].SetActive(true);
        }
        else
        {
            GameManager.instance.popupManager.popups[(int)PopupType.NotSelectedFriend].SetActive(true);
        }
    }

    public void DeleteFriend()
    {
        Debug.Log("친구 삭제");

        for (int i = 0; i < friendList.Count; i++)
        {
            if (friendList[i].isSelected)
            {
                Destroy(friendList[i].gameObject);
                friendList.RemoveAt(i);
                isSelectedSlot = false;
                break;
            }
        }
    }
    #endregion

    #region Request Friend
    public void ShowRequestFriendList()
    {
        ResetSelect();
        requestListScrollPos.anchoredPosition = new Vector2(0, 0);

        isFriendSettings = false;
        settingMenu.SetActive(isFriendSettings);
        GameManager.instance.popupManager.popups[(int)PopupType.RequestFriendList].SetActive(true);
    }
    #endregion
}
