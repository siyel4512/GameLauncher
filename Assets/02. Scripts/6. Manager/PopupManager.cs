using UnityEngine;
using TMPro;
using System.IO;

public enum PopupType
{
    loginFailed,
    logout,
    UserSearch,
    UserSearchFaild,
    AlreadyExistFriend,
    RequestFriend,
    BlankError,
    DeleteFriend,
    NotSelectedFriend,
    RequestFriendList,
    BlackList,
    InvalidPID,
    AlreadyExistRequestUserPopup,
    SelectServer,
    PlayerStateUpdateFailed,
    FunctionUpdate,
    DownloadSetting,
    UnityProjectPathFindFailedPopup_1,
    UnityProjectPathFindFailedPopup_2,
    UGCPathFindFailedPopup,
    IdIncorrect,
    PasswordIncorrect,
    logoutFailed,
    DeletedAccount,
    launcherUpdate
}

public class PopupManager : MonoBehaviour
{
    public GameObject[] popups;

    public TMP_Text[] popupContents;

    public TMP_Text[] blackListContents; 

    #region Login & Logout
    // Login
    public void BTN_ConfirmLoginFail()
    {
        popups[(int)PopupType.loginFailed].SetActive(false);
    }

    // id incorrect
    public void BTN_ConfirmIdIncorrect()
    {
        popups[(int)PopupType.IdIncorrect].SetActive(false);
    }

    // password incorrect
    public void BTN_ConfirmPasswordIncorrect()
    {
        popups[(int)PopupType.PasswordIncorrect].SetActive(false);
    }

    // Logout
    public void ShowLogoutPage()
    {
        popups[(int)PopupType.logout].SetActive(true);
    }

    // confirm logout
    public void BTN_ConfirmLogout()
    {
        popups[(int)PopupType.logout].SetActive(false);
        GameManager.instance.ResetLauncher();
    }

    // cancel logout
    public void BTN_CancelLogout()
    {
        popups[(int)PopupType.logout].SetActive(false);
    }

    // logout failed
    public void BTN_LogoutFailed()
    {
        popups[(int)PopupType.logoutFailed].SetActive(false);
    }
    #endregion

    #region Friend List
    // cancel user search
    public void BTN_CancelUserSearch()
    {
        popups[(int)PopupType.UserSearch].SetActive(false);

        // input field reset
        GameManager.instance.friendListManager.ResetSearchUserNickName();
    }

    // confirm request friend
    public async void BTN_ConfirmRequestFriend()
    {
        popups[(int)PopupType.UserSearch].SetActive(false);
        popups[(int)PopupType.RequestFriend].SetActive(false);

        if (GameManager.instance.jsonData.searchFriendNum != null)
        {
            Debug.Log("[SY] searchFriendNum 값 : " + GameManager.instance.jsonData.searchFriendNum);

            // 친구 신청
            Debug.Log("[SY] 친구 요청 완료");

            // 내 상태 업데이트
            //await GameManager.instance.api.Update_PlayerState(GameManager.instance.playerManager.currentState, Login.PID);

            if (DEV.instance.isUsingTokenForFriendList)
            {
                GameManager.instance.api.Request_AddFriend(Login.PID, GameManager.instance.jsonData.searchFriendNum).Forget();
            }
            else
            {
                GameManager.instance.api.Temp_Request_AddFriend(Login.playerNum, GameManager.instance.jsonData.searchFriendNum).Forget();
                //await GameManager.instance.api.Temp_Request_AddFriend(Login.playerNum, GameManager.instance.jsonData.searchFriendNum);
                //await GameManager.instance.api.Update_PlayerState(GameManager.instance.playerManager.currentState, Login.PID);
            }
        }
        else
        {
            Debug.Log("[SY] searchFriendNum 값 없음");
        }
    }

    // cancel request friend
    public void BTN_CancelRequestFriend()
    {
        popups[(int)PopupType.RequestFriend].SetActive(false);
        popups[(int)PopupType.UserSearch].SetActive(true);
    }

    // confirm search user failed
    public void BTN_ConfirmSearchUserFailed()
    {
        popups[(int)PopupType.UserSearchFaild].SetActive(false);
    }

    // show blank error popup
    public void BTN_BlankError()
    {
        popups[(int)PopupType.BlankError].SetActive(false);
    }

    // confirm delete friend
    public void BTN_ConfirmDeleteFriend()
    {
        popups[(int)PopupType.DeleteFriend].SetActive(false);
        GameManager.instance.friendListManager.DeleteFriend();
    }

    // cancel delete friend
    public void BTN_CancelDeleteFriend()
    {
        popups[(int)PopupType.DeleteFriend].SetActive(false);
        Debug.Log("친구 삭제 취소...");
    }

    public void BTN_ConfrimNotSelectedFriend()
    {
        popups[(int)PopupType.NotSelectedFriend].SetActive(false);
    }

    public void BTN_CloseRequestFriendList()
    {
        popups[(int)PopupType.RequestFriendList].SetActive(false);
    }

    // set popup contents
    public void SetContents(int index, string nickname)
    {
        popupContents[index].text = $"'{nickname}'님께 친구 요청을 하시겠습니까?";
    } 
    
    // close already friend popup
    public void BTN_CloseAlreadyFriend()
    {
        popups[(int)PopupType.AlreadyExistFriend].SetActive(false);
    }

    // close already request user popup
    public void BTN_CloseAlreadyRequestUser()
    {
        popups[(int)PopupType.AlreadyExistRequestUserPopup].SetActive(false);
    }
    #endregion

    #region Black List
    // close black list alert popup
    public void BTN_ConfirmBlackList()
    {
        popups[(int)PopupType.BlackList].SetActive(false);
    }

    // set black list alert contents
    public void SetBlackListAlertContents(string content, string reason)
    {
        blackListContents[0].text = content;
        blackListContents[1].text = reason;
    }
    #endregion

    #region Player State Update
    public void BTN_ConfirmInvalidPIDError()
    {
        popups[(int)PopupType.InvalidPID].SetActive(false);
    }

    public void BTN_ConfirmPlayerStateUpdateFailed()
    {
        popups[(int)PopupType.PlayerStateUpdateFailed].SetActive(false);
    }
    #endregion

    #region Function Update
    public void BTN_ConfirmFunctionUpdate()
    {
        popups[(int)PopupType.FunctionUpdate].SetActive(false);
    }
    #endregion

    #region Download Path Setting
    public void BTN_ConfirmDownloadPathSetting()
    {
        popups[(int)PopupType.DownloadSetting].SetActive(false);
    }
    #endregion

    #region UGC File Download
    public void BTN_ConfirmUnityProjectPathFindFailedPopup1()
    {
        popups[(int)PopupType.UnityProjectPathFindFailedPopup_1].SetActive(false);
    }

    public void BTN_ConfirmUnityProjectPathFindFailedPopup2()
    {
        popups[(int)PopupType.UnityProjectPathFindFailedPopup_2].SetActive(false);
    }

    public void BTN_ConfirmUGCPathFindFailedPopup()
    {
        popups[(int)PopupType.UGCPathFindFailedPopup].SetActive(false);
    }
    #endregion

    #region Deleted Account
    public void BTN_ConfirmDeletedAccount()
    {
        popups[(int)PopupType.DeletedAccount].SetActive(false);
    }
    #endregion

    #region Launcher Update
    // Logout
    public void ShowLauncherUpdatePopup()
    {
        popups[(int)PopupType.launcherUpdate].SetActive(true);
    }

    // confirm logout
    public void BTN_ConfirmLauncherUpdate()
    {
        popups[(int)PopupType.launcherUpdate].SetActive(false);

        // launcher update(download)
        GameManager.instance.launcherDownload.FileDownload().Forget();
    }

    // cancel logout
    public void BTN_CancelLauncherUpdate()
    {
        //popups[(int)PopupType.launcherUpdate].SetActive(false);
        Application.Quit();
    }
    #endregion
}
