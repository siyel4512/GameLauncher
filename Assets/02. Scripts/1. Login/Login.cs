using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.IO;
using System.Net.Http;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using System.Windows.Forms;

using Debug = UnityEngine.Debug;
using UnityEngine.Networking;
using Button = UnityEngine.UI.Button;
using Application = System.Windows.Forms.Application;

public class Login : MonoBehaviour
{
    [Header("[ Id & password Inputfield ]")]
    public InputField id;
    public InputField password;

    [Space(10)]
    [Header("[ Login Button ]")]
    public Button loginButton;

    public static string PID;
    public static string nickname;
    public static string playerNum;
    public static string authrtcd;

    public string temp_PID;
    public string temp_nickname;
    public string temp_playerNum;
    public string temp_authrtcd;

    public string myID;

    public TCP_Server tcp_Server;

    public GameObject batchAdminButton;


    // Start is called before the first frame update
    void Start()
    {
        id.onValueChanged.AddListener(InputValueCheck);
        password.onValueChanged.AddListener(InputValueCheck);

        loginButton.onClick.AddListener(TryRequestKey);
    }

    // Update is called once per frame
    void Update()
    {
        // Move Tab
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (id.isFocused)
            {
                password.Select();
            }
            else if (password.isFocused)
            {
                loginButton.Select();
            }
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            tcp_Server.StartServer();
        }
    }

    // Check Input Field
    private void InputValueCheck(string text)
    {
        if (id.text != "" && password.text != "")
        {
            loginButton.interactable = true;
        }
        else
        {
            loginButton.interactable = false;
        }
    }

    private void TryRequestKey()
    {
        //Debug.Log("Key값 요청");
        //tcp_Server.StartServer();

        if (DEV.instance.isTEST_Login)
        {
            SetLogin();
        }
        else
        {
            RequestKey().Forget();
        }
    }

    // Request public key
    private async UniTaskVoid RequestKey()
    {
        var content = new WWWForm();
        content.AddField("Id", id.text);

        string keyFilePath = "";

        // save publc key
#if UNITY_EDITOR
        keyFilePath = Environment.CurrentDirectory + "\\KEY\\" + id.text + ".pem";
        //Debug.Log($"key File paht : {keyFilePath}");
#else
        string tempPath = Application.ExecutablePath;

        // path setting
        string[] splitPaths = tempPath.Split('\\');
        string assemblePath = "";

        for (int i = 0; i < splitPaths.Length - 1; i++)
        {
            assemblePath += (splitPaths[i] + "\\");
        }

        keyFilePath = assemblePath + "KEY\\" + id.text + ".pem";
#endif
        //keyFilePath = Environment.CurrentDirectory + "\\KEY\\" + id.text + ".pem";

        //Debug.LogError($"[경로 확인 중] {keyFilePath}");

        if (File.Exists(keyFilePath))
        {
            Debug.Log("파일 있음");
            await TryLogin(RSAPasswordEncrypt.ImportPublicKey(File.ReadAllText(keyFilePath)));
        }
        else
        {
            Debug.Log($"파일 없음 / {API.instance.GetKeyURL}");

            using (UnityWebRequest www = UnityWebRequest.Post(API.instance.GetKeyURL, content))
            //using (UnityWebRequest www = UnityWebRequest.Post("http://101.101.218.135:5002/onlineScienceMuseumAPI/checkId.do", content))
            {
                try
                {
                    await www.SendWebRequest();

                    string requestResult = www.downloadHandler.text;

                    Debug.Log("www.result : " +www.result);

                    if (www.result == UnityWebRequest.Result.Success)
                    {
                        //string requestResult = www.downloadHandler.text;
                        Debug.Log("결과값 : " + requestResult);
                        await TryLogin(requestResult);
                    }
                }
                catch (UnityWebRequestException ex)
                {
                    string requestResult = www.downloadHandler.text;

                    Debug.Log("응답 실패 (로그인 실패) : " + requestResult);

                    // invalid ID, password value
                    //GameManager.instance.popupManager.popups[(int)PopupType.loginFailed].SetActive(true);
                    GameManager.instance.popupManager.popups[(int)PopupType.IdIncorrect].SetActive(true);
                }
            }
        }
    }

    // Try Login
    private async UniTask TryLogin(string _requestResult)
    {
        RSAPasswordEncrypt rsaPasswordEncrypt = new RSAPasswordEncrypt();
        string rsaPassword;
        rsaPassword = rsaPasswordEncrypt.GetRSAPassword(_requestResult, id.text, password.text);

        var content = new WWWForm();
        content.AddField("Id", id.text);
        content.AddField("pswd", rsaPassword);

        //Debug.Log($"FilePath.Instance.GetKeyUrl : {URL.Instance.GetKeyUrl} " +
        //    $"/ id.text : {id.text} / FilePath.Instance.Key_password : {URL.Instance.Key_password}" +
        //    $" / rsaPassword : {rsaPassword}");

        Debug.Log("Try Login : " + API.instance.TryLoginURL);
        using (UnityWebRequest www = UnityWebRequest.Post(API.instance.TryLoginURL, content))
        //using (UnityWebRequest www = UnityWebRequest.Post("http://101.101.218.135:5002/onlineScienceMuseumAPI/tryLogin.do", content))
        {
            try
            {
                await www.SendWebRequest();

                if (www.result == UnityWebRequest.Result.Success)
                {
                    myID = id.text;

                    string requestResult = www.downloadHandler.text;

                    Debug.Log("로그인 결과 : " + requestResult);

                    PID = requestResult.Split(":")[1].Split(",")[0];
                    //nickname = requestResult.Split(":")[2].Split(",")[0];
                    nickname = myID;
                    playerNum = requestResult.Split(":")[3].Split(",")[0];
                    authrtcd = requestResult.Split(":")[4].Split("}")[0];

                    temp_authrtcd = authrtcd;

                    Debug.Log($"requestResult : {PID} / {nickname} / {playerNum} / {authrtcd}");

                    // 일반 유저
                    if (authrtcd == "00" || authrtcd == "03")
                    {
                        SetLogin();
                    }
                    // 관리자 & 슈퍼 계정
                    else
                    {
                        AdminUser();
                    }
                }
            }
            catch (UnityWebRequestException ex)
            {
                string requestResult = www.downloadHandler.text;

                Debug.Log("응답 실패 (로그인 실패) : " + requestResult);

                //Debug.Log("블랙리스트 결과 : " + requestResult.Contains("TL_103"));

                bool isBlackList = requestResult.Contains("TL_103");

                // error code : TL_103
                // black list
                if (isBlackList)
                {
                    string blackListContents = requestResult.Substring(6, requestResult.Length - 6);
                    string[] contents = blackListContents.Split(" / ");

                    GameManager.instance.popupManager.SetBlackListAlertContents(contents[0], contents[1]);
                    GameManager.instance.popupManager.popups[(int)PopupType.BlackList].SetActive(true);
                }
                // error code : TL_102
                // invalid ID, password value
                else
                {
                    //GameManager.instance.popupManager.popups[(int)PopupType.loginFailed].SetActive(true);
                    GameManager.instance.popupManager.popups[(int)PopupType.PasswordIncorrect].SetActive(true);
                }
            }
        }
    }

    private async UniTask TryLogin(RSACryptoServiceProvider rsa)
    {
        string rsaPassword;
        rsaPassword = Convert.ToBase64String(rsa.Encrypt((new UTF8Encoding()).GetBytes(password.text), false));

        var content = new WWWForm();
        content.AddField("Id", id.text);
        content.AddField("pswd", rsaPassword);

        Debug.Log("rsaPassword " + rsaPassword);
        //Debug.Log("content 인코딩 완료");

        //var content = new FormUrlEncodedContent(loginValues);
        Debug.Log("Try Login : " + API.instance.TryLoginURL);
        using (UnityWebRequest www = UnityWebRequest.Post(API.instance.TryLoginURL, content))
        //using (UnityWebRequest www = UnityWebRequest.Post("http://101.101.218.135:5002/onlineScienceMuseumAPI/tryLogin.do", content))
        {
            try
            {
                await www.SendWebRequest();

                Debug.Log("www.result : " + www.result);

                if (www.result == UnityWebRequest.Result.Success)
                {
                    string requestResult = www.downloadHandler.text;

                    Debug.Log("로그인 결과 : " + requestResult);

                    myID = id.text;

                    PID = requestResult.Split(":")[1].Split(",")[0];
                    //nickname = requestResult.Split(":")[2].Split(",")[0];
                    nickname = myID;
                    playerNum = requestResult.Split(":")[3].Split(",")[0];
                    authrtcd = requestResult.Split(":")[4].Split("}")[0];

                    temp_authrtcd = authrtcd;

                    Debug.Log($"requestResult : {PID} / {nickname} / {playerNum} / {authrtcd}");

                    // 일반 유저
                    if (authrtcd == "00" || authrtcd == "03")
                    {
                        SetLogin();
                    }
                    // 관리자 & 슈퍼 계정
                    else
                    {
                        AdminUser();
                    }
                }
                
            }
            catch (UnityWebRequestException ex)
            {
                string requestResult = www.downloadHandler.text;

                Debug.Log("응답 실패 (로그인 실패) : " + requestResult);

                //Debug.Log("블랙리스트 결과 : " + requestResult.Contains("TL_103"));

                bool isBlackList = requestResult.Contains("TL_103");

                // error code : TL_103
                // black list
                if (isBlackList)
                {
                    string blackListContents = requestResult.Substring(6, requestResult.Length - 6);
                    string[] contents = blackListContents.Split(" / ");

                    GameManager.instance.popupManager.SetBlackListAlertContents(contents[0], contents[1]);
                    GameManager.instance.popupManager.popups[(int)PopupType.BlackList].SetActive(true);
                }
                // error code : TL_102
                // invalid ID, password value
                else
                {
                    //GameManager.instance.popupManager.popups[(int)PopupType.loginFailed].SetActive(true);
                    GameManager.instance.popupManager.popups[(int)PopupType.PasswordIncorrect].SetActive(true);
                }
            }
        }
    }

    // Success login
    public void SetLogin()
    {
        GameManager gameManager = GameManager.instance;

        gameManager.GetComponent<SelectServer>().SetLiveServer();

        gameManager.isLogin = true; // login

        gameManager.playerManager.nickname.text = myID;// set id
        gameManager.playerManager.nickname_legacy.text = myID;// set id

        gameManager.playerManager.SetPlayerState(1); // set player state
        gameManager.SetPage(1); // set main page

        gameManager.api.Request_FriendList().Forget();// create friedn list
        gameManager.api.Request_RequestFriendList().Forget(); // create request friend list

        gameManager.bannerNoticeManager.CreateAllContents();
        
        // start TCP server
        tcp_Server.StartServer();

        // id & password input field reset
        id.text = "";
        password.text = "";
    }

    public void AdminUser()
    {
        GameManager gameManager = GameManager.instance;

        gameManager.GetComponent<SelectServer>().SetTestServer();
        gameManager.popupManager.popups[(int)PopupType.SelectServer].SetActive(true);

        batchAdminButton.SetActive(true);
    }

    public void BTN_SelectServer()
    {
        GameManager gameManager = GameManager.instance;

        gameManager.popupManager.popups[(int)PopupType.SelectServer].SetActive(false);

        FilePath.Instance.SetDownloadURL(gameManager.selectedServerNum);

        gameManager.isLogin = true; // login
        gameManager.playerManager.nickname.text = myID;// set id
        gameManager.playerManager.nickname_legacy.text = myID;// set id

        gameManager.playerManager.SetPlayerState(1); // set player state
        gameManager.SetPage(1); // set main page

        gameManager.api.Request_FriendList().Forget();// create friedn list
        gameManager.api.Request_RequestFriendList().Forget(); // create request friend list

        gameManager.bannerNoticeManager.CreateAllContents();

        // start TCP server
        tcp_Server.StartServer();

        // id & password input field reset
        id.text = "";
        password.text = "";
    }

    // Success logout
    public void SetLogOut()
    {
        GameManager gameManager = GameManager.instance;
        gameManager.isLogin = false; // logout

        if (DEV.instance.isAdmin)
        {
            batchAdminButton.SetActive(false);
        }

        DEV.instance.isAdmin = false;

        gameManager.bannerNoticeManager.mainBoardScrollPos.anchoredPosition = new Vector2(0, 0); // scroll reset
        GameManager.instance.SetSelectButton(0); // set file download button

        // offline
        gameManager.playerManager.RequestPlayerStateUpdate(0);

        // delete nick name
        gameManager.playerManager.nickname.text = "";
        gameManager.playerManager.nickname_legacy.text = "";

        // reset friend list & request list
        gameManager.friendListManager.isSelectedSlot = false; // selected friend slot reset
        gameManager.friendListManager.DeleteList(); // delete friend list
        gameManager.requestFriendManager.DeleteRequestList(); // delete request friend list
        gameManager.playerManager.StopTimer(); // player state change timer reset
        
        // event banner & notice & news delete
        gameManager.bannerNoticeManager.bannerUI.DeleteContents();
        gameManager.bannerNoticeManager.shortNotice.ResetShortNoticeInfo();
        gameManager.bannerNoticeManager.noticeUI.DeleteContents();

        // user guide reset
        gameManager.bannerNoticeManager.guideInfo[0].ResetLinkURL();
        gameManager.bannerNoticeManager.guideInfo[1].ResetLinkURL();

        //GameManager.instance.ResetBuildFilePath();

        // reset json data
        gameManager.jsonData.ResetFriendListJsonData();

        gameManager.ForceQuit();

        // stop TCP server
        tcp_Server.StopServer();

        PID = "";
    }

    private void OnApplicationQuit()
    {
        //if (DEV.instance.isUsingAbnormalShutdown)
        //{
        //    GameManager.instance.api.Send_AbnormalShutdown().Forget();
        //}

        tcp_Server.StopServer();

        PID = "";
    }
}
