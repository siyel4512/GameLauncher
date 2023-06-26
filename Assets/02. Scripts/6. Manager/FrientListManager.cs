using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Net.Http;
using Cysharp.Threading.Tasks;
using UnityEngine.Networking;

public class FrientListManager : MonoBehaviour
{
    public TMP_InputField searchFriendNickName;
    public TMP_InputField searchUserNickName;

    public Button addButton;
    public Button settingButton;

    //public Button searchUserButton;

    // Start is called before the first frame update
    void Start()
    {
        searchFriendNickName.onSubmit.AddListener(TrySearchFriend);
        searchUserNickName.onSubmit.AddListener(TrySearchUser);

        addButton.onClick.AddListener(TryAddFriend);
        settingButton.onClick.AddListener(TryDeleteFriend);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TrySearchFriend(string text)
    {
        Debug.Log("try search my friend : " + text);
        SearchFriend();
    }

    private void SearchFriend()
    {
        // Request
        // Todo : delete GameManager.instance.isTEST
        if (GameManager.instance.isTEST)
        {
            // find
            if (searchFriendNickName.text == "test")
            {
                Debug.Log("show friend list : "  + searchFriendNickName.text);
            }
            // blank
            else if (searchFriendNickName.text == "") 
            {
                GameManager.instance.popupManager.popups[(int)PopupType.BlankError].SetActive(true);
            }
            // not find
            else
            {
                Debug.Log("not found friend : " + searchFriendNickName.text);
            }
        }
        else
        {
            
        }
    }

    public void TryAddFriend()
    {
        Debug.Log("친구 추가 시도");
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
        if (GameManager.instance.isTEST)
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

    public void TryDeleteFriend()
    {
        Debug.Log("친구 삭제 시도");
    }
}
