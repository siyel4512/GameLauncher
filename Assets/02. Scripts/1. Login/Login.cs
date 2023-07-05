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
    public InputField id;
    public InputField password;

    public Button loginButton;

    public static string token;

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

        if (DEV.instance.isTEST)
        {
            GameManager.instance.playerManager.SetPlayerState(0);
            GameManager.instance.SetPage(1);
        }
        else
        {
            RequestKey().Forget();
        }
    }

    // Request public key
    private async UniTaskVoid RequestKey()
    {
        Debug.Log($"{URL.Instance.Key_id} / {id.text}");

        var idValue = new Dictionary<string, string>
        {
            { URL.Instance.Key_id, id.text }
        };

        string keyFilePath = Environment.CurrentDirectory + "\\KEY\\" + id.text + ".pem";
        Debug.Log($"key File paht : {keyFilePath}");
        
        if (File.Exists(keyFilePath))
        {
            Debug.Log("파일 있음");
            await TryLogin(RSAPasswordEncrypt.ImportPublicKey(File.ReadAllText(keyFilePath)));
        }
        else
        {
            Debug.Log($"파일 없음 / {URL.Instance.GetKeyUrl}");
            var content = new FormUrlEncodedContent(idValue);
            HttpClient client = new HttpClient();
            var response = await client.PostAsync(URL.Instance.GetKeyUrl, content);
            string requestResult = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
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
            { URL.Instance.Key_id, id.text },
            { URL.Instance.Key_password, rsaPassword }
        };

        Debug.Log($"FilePath.Instance.GetKeyUrl : {URL.Instance.GetKeyUrl} " +
            $"/ id.text : {id.text} / FilePath.Instance.Key_password : {URL.Instance.Key_password}" +
            $" / rsaPassword : {rsaPassword}");

        var content = new FormUrlEncodedContent(loginValues);

        HttpClient client = new HttpClient();
        var response = await client.PostAsync(URL.Instance.LoginUrl, content);
        string requestResult = await response.Content.ReadAsStringAsync();

        if (response.IsSuccessStatusCode)
        {
            token = requestResult;
            Debug.Log("totkon : " + token);
            
            GameManager.instance.isLogin = true;
            GameManager.instance.playerManager.SetPlayerState(0);
            GameManager.instance.SetPage(1);
        }
        else
        {
            Debug.Log("응답 실패 (로그인 실패) : " + requestResult);

            // invalid ID, password value
            GameManager.instance.popupManager.popups[(int)PopupType.loginFailed].SetActive(true);
        }
    }

    private async UniTask TryLogin(RSACryptoServiceProvider rsa)
    {
        Debug.Log("TryLogin 들어옴");
        string rsaPassword;
        rsaPassword = Convert.ToBase64String(rsa.Encrypt((new UTF8Encoding()).GetBytes(password.text), false));
        Debug.Log(rsaPassword);

        var loginValues = new Dictionary<string, string>
            {
                { URL.Instance.Key_id, id.text },
                { URL.Instance.Key_password, rsaPassword }
            };

        var content = new FormUrlEncodedContent(loginValues);

        Debug.Log("content 인코딩 완료");

        HttpClient client = new HttpClient();
        Debug.Log("요청 시작");
        var response = await client.PostAsync(URL.Instance.LoginUrl, content);
        Debug.Log("요청 완료");
        string requestResult = await response.Content.ReadAsStringAsync();

        Debug.Log(requestResult);

        if (response.IsSuccessStatusCode)
        {
            token = requestResult;
            Debug.Log("totkon : " + token);
            
            GameManager.instance.isLogin = true;
            GameManager.instance.playerManager.SetPlayerState(0);
            GameManager.instance.SetPage(1);
        }
        else
        {
            Debug.Log("응답 실패 (로그인 실패) : " + requestResult);

            // invalid ID, password value
            GameManager.instance.popupManager.popups[(int)PopupType.loginFailed].SetActive(true);
        }
    }

    public void LogOut()
    {
        GameManager.instance.isLogin = false;

        id.text = "";
        password.text = "";
    }
}
