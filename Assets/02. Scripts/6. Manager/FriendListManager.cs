using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FriendListManager : MonoBehaviour
{
    // main buttons
    [Space(10)]
    [Header("[ Main Buttons ]")]
    public Button addButton;
    public Button settingButton;

    // input field
    [Header("[ Friend Searching ]")]
    public TMP_InputField searchFriendNickName;
    public InputField searchFriendNickname;
    public TMP_InputField searchUserNickName;
    public InputField searchUserNickname;
    public TMP_Text searchUserWaringText;

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

    public GameObject warningText;

    // Start is called before the first frame update
    void Start()
    {
        searchFriendNickname.onSubmit.AddListener(InputEnter);
        searchFriendNickname.onValueChanged.AddListener(CheckedNickName);

        searchUserNickname.onSubmit.AddListener(TrySearchUser);

        addButton.onClick.AddListener(TryAddFriend);
        settingButton.onClick.AddListener(ShowSettingMenu);

        listScrollPos.anchoredPosition = new Vector2(0, 0);
        requestListScrollPos.anchoredPosition = new Vector2(0, 0);

        searchUserWaringText.text = "";
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

    public void CreateList()
    //public IEnumerator CreateList()
    {
        List<SaveData.friendList> friendListValues = GameManager.instance.jsonData.friendList_List;
        
        //yield return null;
        for (int i = 0; i < friendCount; i++)
        {
            // create & set temp friend info
            temp_friendList.Add(new FriendInfo()
            {
                id = friendListValues[i].id,
                frndNo = friendListValues[i].frndNo,
                mbrNo = friendListValues[i].mbrNo,
                frndMbrNo = friendListValues[i].frndMbrNo,
                frndSttus = friendListValues[i].frndSttus,
                frndRqstSttus = friendListValues[i].frndRqstSttus,
                frndRqstDt = friendListValues[i].frndRqstDt,
                upDt = friendListValues[i].upDt,
                regDt = friendListValues[i].regDt
            });

            // create friend list
            GameObject clone = Instantiate(listSlot);
            clone.transform.SetParent(listContent.transform, false);

            // set friend info
            FriendInfo info = clone.GetComponent<FriendInfo>();

            info.id = friendListValues[i].id;
            info.frndNo = friendListValues[i].frndNo;
            info.mbrNo = friendListValues[i].mbrNo;
            info.frndMbrNo = friendListValues[i].frndMbrNo;
            info.frndSttus = friendListValues[i].frndSttus;
            info.frndRqstSttus = friendListValues[i].frndRqstSttus;
            info.frndRqstDt = friendListValues[i].frndRqstDt;
            info.upDt = friendListValues[i].upDt;
            info.regDt = friendListValues[i].regDt;
            info.SetSlotValues();

            friendList.Add(info);
        }

        listScrollPos.anchoredPosition = new Vector2(0, 0);

        DeduplicationFriendListSlot(friendList);

        CheckActiveSlot(friendList);
    }

    // deduplication
    public void DeduplicationFriendListSlot(List<FriendInfo> _friendList)
    {
        // find duplicates by grouping (using ncnm, frndMbrNo)
        var groupsByCombinedKey = _friendList
            .GroupBy(info => (info.id, info.frndMbrNo))
            .Where(group => group.Count() > 1);

        // duplicate element output
        foreach (var group in groupsByCombinedKey)
        {
            //Debug.Log("중복된 요소: ncnm=" + group.Key.ncnm + ", frndMbrNo=" + group.Key.frndMbrNo);

            // 그룹 내의 요소들의 인덱스 출력
            int index = 0;
            foreach (var duplicate in group)
            {
                if (index > 0)
                {
                    // 첫 번째 요소 이외의 나머지 요소를 비활성화
                    duplicate.gameObject.SetActive(false);
                }
                index++;
            }
        }
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

    public void CheckActiveSlot(List<FriendInfo> _friendList)
    {
        if (_friendList.Count <= 0)
        {
            warningText.SetActive(true);
            return;
        }

        for (int i = 0; i < _friendList.Count; i++)
        {
            if (_friendList[i].gameObject.activeSelf)
            {
                warningText.SetActive(false);
                break;
            }
            else
            {
                warningText.SetActive(true);
            }
        }
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

        DeduplicationFriendListSlot(friendList);
    }

    private void InputEnter(string text)
    {
        Debug.Log("try search my friend : " + text);

        // blank
        if (searchFriendNickname.text == "")
        {
            //GameManager.instance.popupManager.popups[(int)PopupType.BlankError].SetActive(true);
            searchUserWaringText.text = "추가할 유저 닉네임을 입력해 주세요.";
        }
    }
    #endregion

    #region Add Friend
    public void TryAddFriend()
    {
        Debug.Log("친구 추가 시도");
        searchUserWaringText.text = "";
        ResetSelect();
        GameManager.instance.popupManager.popups[(int)PopupType.UserSearch].SetActive(true);
    }

    public void TrySearchUser(string text)
    {
        SearchUser();
    }

    public async void SearchUser()
    {
        //await GameManager.instance.api.Update_PlayerState(GameManager.instance.playerManager.currentState, Login.PID);

        // Requset
        // Todo : delete GameManager.instance.isTEST
        if (DEV.instance.isTEST_Login)
        {
            // find
            //if (searchUserNickName.text == "test")
            if (searchUserNickname.text == "test")
            {
                //Debug.Log("Success find user : " + searchUserNickName.text);
                Debug.Log("Success find user : " + searchUserNickname.text);
                //GameManager.instance.popupManager.SetContents(1, searchUserNickName.text);
                GameManager.instance.popupManager.SetContents(1, searchUserNickname.text);
                GameManager.instance.popupManager.popups[(int)PopupType.RequestFriend].SetActive(true);
                //searchUserNickName.text = "";
                searchUserNickname.text = "";
            }
            // blank
            //else if (searchUserNickName.text == "")
            else if (searchUserNickname.text == "")
            {
                GameManager.instance.popupManager.popups[(int)PopupType.BlankError].SetActive(true);
            }
            // not find
            else
            {
                //Debug.Log("Failed find user : " + searchUserNickName.text);
                Debug.Log("Failed find user : " + searchUserNickname.text);
                GameManager.instance.popupManager.popups[(int)PopupType.UserSearchFaild].SetActive(true);
            }
        }
        else
        {
            //if (searchUserNickName.text != "")
            if (searchUserNickname.text != "")
            {
                //bool isSuccess = await GameManager.instance.api.Request_SearchFriend(searchUserNickName.text);
                bool isSuccess = await GameManager.instance.api.Request_SearchFriend(searchUserNickname.text);
                //Debug.Log("검색 결과 : " + CheckFriendList(searchUserNickName.text));
                if (isSuccess)
                {
                    //Debug.Log("Success find user : " + searchUserNickName.text);
                    Debug.Log("Success find user : " + searchUserNickname.text);

                    //int isCompareResult = CheckFriendList(searchUserNickName.text);
                    int isCompareResult = CheckFriendList(searchUserNickname.text);

                    switch (isCompareResult)
                    {
                        case 0:
                            // not exist friend
                            Debug.Log("addable user");
                            //GameManager.instance.popupManager.SetContents(1, searchUserNickName.text); // set nick name in popup
                            GameManager.instance.popupManager.SetContents(1, searchUserNickname.text); // set nick name in popup
                            GameManager.instance.popupManager.popups[(int)PopupType.RequestFriend].SetActive(true); // open popup

                            searchUserWaringText.text = "";
                            break;
                        case 1:
                            // exist friend
                            Debug.Log("my friend");
                            //GameManager.instance.popupManager.popups[(int)PopupType.AlreadyExistFriend].SetActive(true); // open popup
                            searchUserWaringText.text = "이미 친구 추가된 유저입니다.";
                            break;
                        case 2:
                            // exist request user
                            Debug.Log("request list user");
                            //GameManager.instance.popupManager.popups[(int)PopupType.AlreadyExistRequestUserPopup].SetActive(true); // open popup
                            searchUserWaringText.text = "친구 요청이 들어온 유저입니다.";
                            break;
                        case 3:
                            Debug.Log("my nick name");
                            searchUserWaringText.text = "요청할 수 없는 유저입니다.";
                            break;
                    }
                }
                else
                {
                    //Debug.Log("Failed find user : " + searchUserNickName.text);
                    Debug.Log("Failed find user : " + searchUserNickname.text);
                    //GameManager.instance.popupManager.popups[(int)PopupType.UserSearchFaild].SetActive(true);

                    searchUserWaringText.text = "해당 유저를 찾을 수 없습니다.";
                }
            }
            //else if (searchUserNickName.text == "")
            else if (searchUserNickname.text == "")
            {
                //GameManager.instance.popupManager.popups[(int)PopupType.BlankError].SetActive(true);

                searchUserWaringText.text = "추가할 유저 닉네임을 입력해 주세요.";
            }
        }
    }

    // compare to seache user and my friend list
    //private int CheckFriendList(string _searchUserNickName)
    private int CheckFriendList(string _searchUserNickname)
    {
        int isExistInList = 0; // 0: not exist friend, 1:exist friend, 2: exist request user, 3: my nick name

        List<SaveData.friendList> friendListValuse = GameManager.instance.jsonData.friendList_List;

        // check my friend list
        for (int i = 0; i < friendListValuse.Count; i++)
        {
            if (friendListValuse[i].id == _searchUserNickname)
            {
                isExistInList = 1;
                Debug.Log("[SY] 해당 친구 있음");
                break;
            }
            else
            {
                isExistInList = 0;
            }
        }

        // check request list
        if (isExistInList == 0)
        {
            List<SaveData.friendList> requestListValuse = GameManager.instance.jsonData.requestFriendListValues;

            for (int i = 0; i < requestListValuse.Count; i++)
            {
                if (requestListValuse[i].id == _searchUserNickname)
                {
                    isExistInList = 2;
                    Debug.Log("[SY] 해당 요청 유저 있음");
                    break;
                }
                else
                {
                    isExistInList = 0;
                }
            }
        }

        // check my nick name
        if (isExistInList == 0)
        {
            if (Login.nickname == _searchUserNickname)
            {
                isExistInList = 3;
            }
            else
            {
                isExistInList = 0;
            }
        }

        return isExistInList;
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

    public async void DeleteFriend()
    {
        Debug.Log("친구 삭제");

        for (int i = 0; i < friendList.Count; i++)
        {
            if (friendList[i].isSelected)
            {
                // 삭제 요청
                await GameManager.instance.api.Request_RefuseNDelete(friendList[i].mbrNo, friendList[i].frndMbrNo);

                Destroy(friendList[i].gameObject);
                friendList.RemoveAt(i);
                isSelectedSlot = false;
                break;
            }
        }

        // 리스트 갱신
        GameManager.instance.api.Request_FriendList().Forget();
    }
    #endregion

    #region Request Friend
    public async void ShowRequestFriendList()
    {
        //await GameManager.instance.api.Update_PlayerState(GameManager.instance.playerManager.currentState, Login.PID);

        ResetSelect();
        requestListScrollPos.anchoredPosition = new Vector2(0, 0);

        isFriendSettings = false;
        settingMenu.SetActive(isFriendSettings);
        GameManager.instance.popupManager.popups[(int)PopupType.RequestFriendList].SetActive(true);
    }
    #endregion

    public void ResetSearchUserNickName()
    {
        searchUserNickname.text = "";
        GameManager.instance.jsonData.searchFriend = null;
        GameManager.instance.jsonData.searchFriendNum = "";
        searchUserWaringText.text = "";
    }
}
