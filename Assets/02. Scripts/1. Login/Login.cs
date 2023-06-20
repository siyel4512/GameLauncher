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

    private async void TryRequestKey()
    {
        //Debug.Log("Key값 요청");
        //tcp_Server.StartServer();
        await RequestKey();

        //GameManager.instance.SetPage(1);
    }

    // Request public key
    private async UniTask RequestKey()
    {
        Debug.Log($"{FilePath.Instance.Key_id} / {id.text}");

        var idValue = new Dictionary<string, string>
        {
            { FilePath.Instance.Key_id, id.text }
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
            Debug.Log($"파일 없음 / {FilePath.Instance.GetKeyUrl}");
            var content = new FormUrlEncodedContent(idValue);
            HttpClient client = new HttpClient();
            var response = await client.PostAsync(FilePath.Instance.GetKeyUrl, content);
            string requestResult = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                await TryLogin(requestResult);
            }
            else
            {
                Debug.Log("응답 실패 (키값 받아오기) : " + requestResult);
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
            { FilePath.Instance.Key_id, id.text },
            { FilePath.Instance.Key_password, rsaPassword }
        };

        Debug.Log($"FilePath.Instance.GetKeyUrl : {FilePath.Instance.GetKeyUrl} " +
            $"/ id.text : {id.text} / FilePath.Instance.Key_password : {FilePath.Instance.Key_password}" +
            $" / rsaPassword : {rsaPassword}");

        var content = new FormUrlEncodedContent(loginValues);

        HttpClient client = new HttpClient();
        var response = await client.PostAsync(FilePath.Instance.LoginUrl, content);
        string requestResult = await response.Content.ReadAsStringAsync();

        if (response.IsSuccessStatusCode)
        {
            token = requestResult;
            Debug.Log("totkon : " + token);
            //Login.Visibility = Visibility.Hidden;
            //FileCheck.Visibility = Visibility.Visible;

            //CheckForUpdates();
            GameManager.instance.SetPage(1);
        }
        else
        {
            Debug.Log("응답 실패 (로그인 실패) : " + requestResult);
        }
    }

    private async UniTask TryLogin(RSACryptoServiceProvider rsa)
    {
        //RSAPasswordEncrypt rsaPasswordEncrypt = new RSAPasswordEncrypt();
        string rsaPassword;
        //rsaPassword = rsaPasswordEncrypt.GetRSAPassword(_requestResult, Password_PasswordBox.Password);
        rsaPassword = Convert.ToBase64String(rsa.Encrypt((new UTF8Encoding()).GetBytes(password.text), false));

        var loginValues = new Dictionary<string, string>
            {
                { FilePath.Instance.Key_id, id.text },
                { FilePath.Instance.Key_password, rsaPassword }
            };

        var content = new FormUrlEncodedContent(loginValues);

        HttpClient client = new HttpClient();
        var response = await client.PostAsync(FilePath.Instance.LoginUrl, content);
        string requestResult = await response.Content.ReadAsStringAsync();

        if (response.IsSuccessStatusCode)
        {
            token = requestResult;
            Debug.Log("totkon : " + token);
            //Login.Visibility = Visibility.Hidden;
            //FileCheck.Visibility = Visibility.Visible;

            //CheckForUpdates();
            GameManager.instance.SetPage(1);
        }
        else
        {
            Debug.Log("응답 실패 (로그인 실패) : " + requestResult);
        }
    }
}
