using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public enum PopupType
{
    loginFailed,
    logout,
    UserSearch,
    UserSearchFaild,
    RequestFriend,
    BlankError,
    DeleteFriend,
    NotSelectedFriend,
    RequestFriendList,
}

public class PopupManager : MonoBehaviour
{
    public GameObject[] popups;

    public TMP_Text[] popupContents;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    #region Login & Logout
    // Login
    public void BTN_ConfirmLoginFail()
    {
        popups[(int)PopupType.loginFailed].SetActive(false);
    }

    // Logout
    public void ShowLogoutPage()
    {
        popups[(int)PopupType.logout].SetActive(true);
    }

    public void BTN_ConfirmLogout()
    {
        popups[(int)PopupType.logout].SetActive(false);
        GameManager.instance.ResetLauncher();
    }

    public void BTN_CancelLogout()
    {
        popups[(int)PopupType.logout].SetActive(false);
    }
    #endregion

    #region Friend List
    public void BTN_CancelUserSearch()
    {
        popups[(int)PopupType.UserSearch].SetActive(false);

        // input field reset
        GameManager.instance.friendListManager.searchUserNickName.text = "";
    }

    public void BTN_ConfirmRequestFriend()
    {
        popups[(int)PopupType.UserSearch].SetActive(false);
        popups[(int)PopupType.RequestFriend].SetActive(false);
    }

    public void BTN_CancelRequestFriend()
    {
        popups[(int)PopupType.RequestFriend].SetActive(false);
    }

    public void BTN_ConfirmSearchUserFailed()
    {
        popups[(int)PopupType.UserSearchFaild].SetActive(false);
    }

    public void BTN_BlankError()
    {
        popups[(int)PopupType.BlankError].SetActive(false);

    }

    public void BTN_ConfirmDeleteFriend()
    {
        popups[(int)PopupType.DeleteFriend].SetActive(false);
        GameManager.instance.friendListManager.DeleteFriend();
    }

    public void BTN_CancelDeleteFriend()
    {
        popups[(int)PopupType.DeleteFriend].SetActive(false);
    }

    public void BTN_ConfrimNotSelectedFriend()
    {
        popups[(int)PopupType.NotSelectedFriend].SetActive(false);
    }

    public void BTN_CloseRequestFriendList()
    {
        popups[(int)PopupType.RequestFriendList].SetActive(false);
    }

    public void SetContents(int index, string nickname)
    {
        popupContents[index].text = $"'{nickname}'님께 친구 요청을 하시겠습니까?";
    }  
    #endregion
}
