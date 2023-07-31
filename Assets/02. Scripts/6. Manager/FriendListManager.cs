using Cysharp.Threading.Tasks;
using Org.BouncyCastle.Crypto.Signers;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
//using System.Threading.Tasks;

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

    public void CreateList()
    //public IEnumerator CreateList()
    {
        //yield return null;
        for (int i = 0; i < friendCount; i++)
        {
            List<SaveData.friendList> friendListValues = GameManager.instance.jsonData.friendListValues;

            // create & set temp friend info
            temp_friendList.Add(new FriendInfo()
            {
                //nickname = $"Test_" + i,
                //state = "온라인",

                ncnm = friendListValues[i].ncnm,
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

            //info.nickname = temp_friendList[i].nickname;
            //info.state = temp_friendList[i].state;

            info.ncnm = friendListValues[i].ncnm;
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

        // avoid duplicate creation
        if (friendList.Count != GameManager.instance.jsonData.friendListValues.Count)
        {
            DeleteList();

            for (int i = 0; i < friendCount; i++)
            {
                List<SaveData.friendList> friendListValues = GameManager.instance.jsonData.friendListValues;

                if (friendListValues[i].frndRqstSttus == "1")
                {
                    // create & set temp friend info
                    temp_friendList.Add(new FriendInfo()
                    {
                        //nickname = $"Test_" + i,
                        //state = "온라인",

                        ncnm = friendListValues[i].ncnm,
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

                    //info.nickname = temp_friendList[i].nickname;
                    //info.state = temp_friendList[i].state;

                    info.ncnm = friendListValues[i].ncnm;
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
            }
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
        SearchUser();
    }

    public async void SearchUser()
    {
        // Requset
        // Todo : delete GameManager.instance.isTEST
        if (DEV.instance.isTEST_Login)
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
            if (searchUserNickName.text != "")
            {
                bool isSuccess = await GameManager.instance.api.Request_SearchFriend(searchUserNickName.text);
                //Debug.Log("검색 결과 : " + CheckFriendList(searchUserNickName.text));
                if (isSuccess)
                {
                    Debug.Log("Success find user : " + searchUserNickName.text);

                    //bool isCompareResult_Old = Old_CheckFriendList(searchUserNickName.text);

                    //// Todo : 친구 검색 및 요청에 대한 수정 필요
                    //if (!isCompareResult_Old)
                    //{
                    //    // 아직 내 친구가 아닐때
                    //    GameManager.instance.popupManager.SetContents(1, searchUserNickName.text); // set nick name in popup
                    //    GameManager.instance.popupManager.popups[(int)PopupType.RequestFriend].SetActive(true); // open popup
                        
                    //    // 친구 요청
                    //    Debug.Log("친구 요청");
                    //}
                    //else
                    //{
                    //    // 내 친구일때
                    //    GameManager.instance.popupManager.popups[(int)PopupType.AlreadyExistFriend].SetActive(true); // open popup
                    //    ResetSearchUserNickName();
                    //}

                    int isCompareResult = CheckFriendList(searchUserNickName.text);

                    //GameManager.instance.popupManager.SetContents(1, searchUserNickName.text); // set nick name in popup
                    //GameManager.instance.popupManager.popups[(int)PopupType.RequestFriend].SetActive(true); // open popup

                    switch (isCompareResult)
                    {
                        case 0:
                            // not exist friend
                            Debug.Log("addable user");
                            GameManager.instance.popupManager.SetContents(1, searchUserNickName.text); // set nick name in popup
                            GameManager.instance.popupManager.popups[(int)PopupType.RequestFriend].SetActive(true); // open popup
                            break;
                        case 1:
                            // exist friend
                            Debug.Log("my friend");
                            GameManager.instance.popupManager.popups[(int)PopupType.AlreadyExistFriend].SetActive(true); // open popup
                            //ResetSearchUserNickName();
                            break;
                        case 2:
                            // exist request user
                            Debug.Log("request list user");
                            GameManager.instance.popupManager.popups[(int)PopupType.AlreadyExistRequestUserPopup].SetActive(true); // open popup
                            break;
                    }
                }
                else
                {
                    Debug.Log("Failed find user : " + searchUserNickName.text);
                    GameManager.instance.popupManager.popups[(int)PopupType.UserSearchFaild].SetActive(true);
                }
            }
            else if (searchUserNickName.text == "")
            {
                GameManager.instance.popupManager.popups[(int)PopupType.BlankError].SetActive(true);
            }
        }
    }

    //// compare to seache user and my friend list
    //private bool Old_CheckFriendList(string _searchUserNickName)
    //{
    //    bool isExistInList = false;

    //    List<SaveData.friendList> friendListValuse = GameManager.instance.jsonData.friendListValues;

    //    for (int i = 0; i < friendListValuse.Count; i++)
    //    {
    //        if (friendListValuse[i].ncnm == _searchUserNickName)
    //        {
    //            isExistInList = true;
    //            break;
    //        }
    //        else
    //        {
    //            isExistInList = false;
    //        }
    //    }
    //    return isExistInList;
    //}

    // compare to seache user and my friend list
    private int CheckFriendList(string _searchUserNickName)
    {
        int isExistInList = 0; // 0: not exist friend, 1:exist friend, 2: exist request user

        List<SaveData.friendList> friendListValuse = GameManager.instance.jsonData.friendListValues;
        
        for (int i = 0; i < friendListValuse.Count; i++)
        {
            if (friendListValuse[i].ncnm == _searchUserNickName)
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

        if (isExistInList == 0)
        {
            List<SaveData.friendList> requestListValuse = GameManager.instance.jsonData.requestFriendListValues;
            
            for (int i = 0; i < requestListValuse.Count; i++)
            {
                if (requestListValuse[i].ncnm == _searchUserNickName)
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
    public void ShowRequestFriendList()
    {
        ResetSelect();
        requestListScrollPos.anchoredPosition = new Vector2(0, 0);

        isFriendSettings = false;
        settingMenu.SetActive(isFriendSettings);
        GameManager.instance.popupManager.popups[(int)PopupType.RequestFriendList].SetActive(true);
    }
    #endregion

    public void ResetSearchUserNickName()
    {
        searchUserNickName.text = "";
        GameManager.instance.jsonData.searchFriend = null;
    }
}
