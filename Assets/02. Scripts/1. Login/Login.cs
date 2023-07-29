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
            PID = requestResult;
            Debug.Log("PID : " + PID);

            SetLogin();
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

        //Debug.Log("content 인코딩 완료");

        HttpClient client = new HttpClient();
        client.BaseAddress = new Uri(API.instance.BaseServer);
        //var response = await client.PostAsync(URL.Instance.Old_LoginUrl, content);
        var response = await client.PostAsync(API.instance.TryLoginURL, content);
        string requestResult = await response.Content.ReadAsStringAsync();

        if (response.IsSuccessStatusCode)
        {
            PID = requestResult;
            //Debug.Log("PID : " + PID);

            SetLogin();
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
        // set nick name
        GameManager.instance.playerManager.nickname.text = "Player Nick Name1";

        GameManager.instance.isLogin = true; // login
        GameManager.instance.playerManager.SetPlayerState(0); // set player state
        //GameManager.instance.pages[1].SetActive(true); // set main page
        GameManager.instance.SetPage(1);
        GameManager.instance.SetSelectButton(0); // set file download button

        //GameManager.instance.friendListManager.CreateList(); // create friedn list
        //GameManager.instance.requestFriendManager.CreateRequestList(); // create request friend list
        GameManager.instance.api.Request_FriendList().Forget();// create friedn list
        GameManager.instance.api.Request_RequestFriendList().Forget(); // create request friend list

        GameManager.instance.bannerNoticeManager.CreateAllContents();
        GameManager.instance.bannerNoticeManager.SetGuideDownloadLink();

        //GameManager.instance.filePath.Test_SetDownloadURL();
        
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
        // delete nick name
        GameManager.instance.playerManager.nickname.text = "";

        GameManager.instance.isLogin = false; // logout
        GameManager.instance.friendListManager.isSelectedSlot = false; // selected friend slot reset
        GameManager.instance.friendListManager.DeleteList(); // delete friend list
        GameManager.instance.requestFriendManager.DeleteRequestList(); // delete request friend list
        GameManager.instance.playerManager.StopTimer(); // player state change timer reset
        GameManager.instance.bannerNoticeManager.bannerUI.DeleteContents();
        GameManager.instance.bannerNoticeManager.noticeUIs[0].DeleteContents();
        GameManager.instance.bannerNoticeManager.noticeUIs[1].DeleteContents();

        //GameManager.instance.ResetBuildFilePath();

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
