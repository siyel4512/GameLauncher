using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.IO;
using System.Net.Http;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;

using Debug = UnityEngine.Debug;

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

    private TCP_Server tcp_Server = new TCP_Server();


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
        var idValue = new Dictionary<string, string>
        {
            { "Id", id.text }
        };

        string keyFilePath = Environment.CurrentDirectory + "\\KEY\\" + id.text + ".pem";
        //Debug.Log($"key File paht : {keyFilePath}");
        
        if (File.Exists(keyFilePath))
        {
            Debug.Log("파일 있음");
            await TryLogin(RSAPasswordEncrypt.ImportPublicKey(File.ReadAllText(keyFilePath)));
        }
        else
        {
            //Debug.Log($"파일 없음 / {URL.Instance.Old_GetKeyUrl}");
            Debug.Log($"파일 없음 / {API.instance.GetKeyURL}");
            var content = new FormUrlEncodedContent(idValue);
            HttpClient client = new HttpClient();
            //var response = await client.PostAsync(URL.Instance.Old_GetKeyUrl, content);
            var response = await client.PostAsync(API.instance.GetKeyURL, content);
            string requestResult = await response.Content.ReadAsStringAsync();
            
            if (response.IsSuccessStatusCode)
            {
                Debug.Log("결과값 : " + requestResult);
                await TryLogin(requestResult);
            }
            else
            {
                Debug.Log("응답 실패 (로그인 실패) : " + requestResult);

                // invalid ID, password value
                GameManager.instance.popupManager.popups[(int)PopupType.loginFailed].SetActive(true);
            }
        }
    }

    // Try Login
    private async UniTask TryLogin(string _requestResult)
    {
        RSAPasswordEncrypt rsaPasswordEncrypt = new RSAPasswordEncrypt();
        string rsaPassword;
        rsaPassword = rsaPasswordEncrypt.GetRSAPassword(_requestResult, id.text, password.text);

        var loginValues = new Dictionary<string, string>
        {
            { "Id", id.text },
            { "pswd", rsaPassword }
        };

        //Debug.Log($"FilePath.Instance.GetKeyUrl : {URL.Instance.GetKeyUrl} " +
        //    $"/ id.text : {id.text} / FilePath.Instance.Key_password : {URL.Instance.Key_password}" +
        //    $" / rsaPassword : {rsaPassword}");

        var content = new FormUrlEncodedContent(loginValues);

        HttpClient client = new HttpClient();
        client.BaseAddress = new Uri(API.instance.BaseServer);
        //var response = await client.PostAsync(URL.Instance.Old_LoginUrl, content);
        var response = await client.PostAsync(API.instance.TryLoginURL, content);
        string requestResult = await response.Content.ReadAsStringAsync();

        if (response.IsSuccessStatusCode)
        {
            //PID = requestResult;
            //Debug.Log("PID : " + PID);

            Debug.Log("로그인 결과 : " + requestResult);

            PID = requestResult.Split(":")[1].Split(",")[0];
            nickname = requestResult.Split(":")[2].Split(",")[0];
            playerNum = requestResult.Split(":")[3].Split("}")[0];
            authrtcd = requestResult.Split(":")[4].Split("}")[0];

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
        else
        {
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
                GameManager.instance.popupManager.popups[(int)PopupType.loginFailed].SetActive(true);
            }
        }
    }

    private async UniTask TryLogin(RSACryptoServiceProvider rsa)
    {
        string rsaPassword;
        rsaPassword = Convert.ToBase64String(rsa.Encrypt((new UTF8Encoding()).GetBytes(password.text), false));

        var loginValues = new Dictionary<string, string>
        {
            { "Id", id.text },
            { "pswd", rsaPassword }
        };

        var content = new FormUrlEncodedContent(loginValues);
        Debug.Log("rsaPassword " + rsaPassword);
        //Debug.Log("content 인코딩 완료");

        HttpClient client = new HttpClient();
        client.BaseAddress = new Uri(API.instance.BaseServer);
        //var response = await client.PostAsync(URL.Instance.Old_LoginUrl, content);
        var response = await client.PostAsync(API.instance.TryLoginURL, content);
        string requestResult = await response.Content.ReadAsStringAsync();

        if (response.IsSuccessStatusCode)
        {
            //PID = requestResult;
            //Debug.Log("PID : " + PID);

            Debug.Log("로그인 결과 : " + requestResult);

            PID = requestResult.Split(":")[1].Split(",")[0];
            nickname = requestResult.Split(":")[2].Split(",")[0];
            playerNum = requestResult.Split(":")[3].Split("}")[0];
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
        else
        {
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
                GameManager.instance.popupManager.popups[(int)PopupType.loginFailed].SetActive(true);
            }
        }
    }

    // Success login
    public void SetLogin()
    {
        GameManager gameManager = GameManager.instance;

        //gameManager.selectedServerNum = 3;

        //FilePath.Instance.Test_SetDownloadURL2(gameManager.selectedServerNum);

        gameManager.GetComponent<SelectServer>().SetLiveServer();

        gameManager.isLogin = true; // login
        gameManager.playerManager.nickname.text = nickname;// set nick name
        gameManager.playerManager.nickname_legacy.text = nickname;// set nick name
        
        gameManager.playerManager.SetPlayerState(1); // set player state
        gameManager.SetPage(1); // set main page

        gameManager.api.Request_FriendList().Forget();// create friedn list
        gameManager.api.Request_RequestFriendList().Forget(); // create request friend list

        gameManager.bannerNoticeManager.CreateAllContents();
        
        //GameManager.instance.pages[0].SetActive(false); // hide login page

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
    }

    public void BTN_SelectServer()
    {
        GameManager gameManager = GameManager.instance;

        //gameManager.GetComponent<SelectServer>().SetTestServer();

        gameManager.popupManager.popups[(int)PopupType.SelectServer].SetActive(false);

        FilePath.Instance.Test_SetDownloadURL2(gameManager.selectedServerNum);

        gameManager.isLogin = true; // login
        gameManager.playerManager.nickname.text = nickname;// set nick name
        gameManager.playerManager.nickname_legacy.text = nickname;// set nick name

        gameManager.playerManager.SetPlayerState(1); // set player state
        gameManager.SetPage(1); // set main page

        gameManager.api.Request_FriendList().Forget();// create friedn list
        gameManager.api.Request_RequestFriendList().Forget(); // create request friend list

        gameManager.bannerNoticeManager.CreateAllContents();

        //GameManager.instance.pages[0].SetActive(false); // hide login page

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
        tcp_Server.StopServer();

        PID = "";
    }
}
