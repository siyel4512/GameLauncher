using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Net.Http;
using Cysharp.Threading.Tasks;
using UnityEngine.Networking;
using System.Globalization;
using Unity.VisualScripting;
//using System.Threading.Tasks;

public class FrientListManager : MonoBehaviour
{
    // input field
    public TMP_InputField searchFriendNickName;
    public TMP_InputField searchUserNickName;

    // main buttons
    public Button addButton;
    public Button settingButton;

    // setting menu
    public GameObject settingMenu;
    public bool isFriendSettings;

    // friend list
    public RectTransform listScrollPos;
    public GameObject listContent;
    public GameObject listSlot;
    public List<FriendInfo> friendList;
    public List<FriendInfo> temp_friendList;
    public bool isSelectedSlot;

    // friend request list
    public RectTransform requestListScrollPos;

    //public Button searchUserButton;

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
    public async UniTaskVoid CreateList()
    {
        await UniTask.SwitchToTaskPool();

        for (int i = 0; i < 100; i++)
        //for (int i = 0; i < 10; i++)
        {
            temp_friendList.Add(new FriendInfo() { nickname = $"Test_" + i, state = "온라인" });

            GameObject clone = Instantiate(listSlot);
            clone.transform.SetParent(listContent.transform, false);
            clone.GetComponent<FriendInfo>().nickname = temp_friendList[i].nickname;
            clone.GetComponent<FriendInfo>().state = temp_friendList[i].state;
            clone.GetComponent<FriendInfo>().SetSlotValues();
            friendList.Add(clone.GetComponent<FriendInfo>());
        }

        listScrollPos.anchoredPosition = new Vector2(0, 0);

        await UniTask.SwitchToMainThread();
        
    }

    public async UniTaskVoid DeleteList()
    {
        await UniTask.SwitchToTaskPool();
        friendList.Clear();
        temp_friendList.Clear();
        await UniTask.SwitchToMainThread();
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
