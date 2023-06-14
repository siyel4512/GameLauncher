using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Text.RegularExpressions;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System;
using Unity.VisualScripting.Antlr3.Runtime;

public class Login : MonoBehaviour
{
    public InputField id;
    public InputField password;

    public Button loginButton;

    // Start is called before the first frame update
    void Start()
    {
        id.onValueChanged.AddListener(InputValueCheck);
        password.onValueChanged.AddListener(InputValueCheck);

        loginButton.onClick.AddListener(RequestKey);
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

    private void RequestKey()
    {
        Debug.Log("Key값 요청");
    }

    public void TryLogin()
    {
        Debug.Log("로그인 시도");
    }

    //
    // Request public key
    // login
    private string getKeyUrl;
    private string loginUrl;
    private string key_id;
    private string key_password;

    // file download path
    private string rootPath;
    private string gameBuildPath;
    private string gameZipPath;
    private string gameExePath;

    // file download
    private string buildFileUrl;
    private string jsonFileUrl;

    // TCP
    //private TCP_Server tcp_Server;
    public static string token;

    //private async Task RequestKey()
    //{

    //    var idValue = new Dictionary<string, string>
    //        {
    //            { key_id, ID_TextBox.Text }
    //        };

    //    string keyFilePath = Environment.CurrentDirectory + "\\KEY\\" + ID_TextBox.Text + ".pem";
    //    if (File.Exists(keyFilePath))
    //    {
    //        await TryLogin(RSAPasswordEncrypt.ImportPublicKey(File.ReadAllText(keyFilePath)));
    //    }
    //    else
    //    {
    //        var content = new FormUrlEncodedContent(idValue);
    //        HttpClient client = new HttpClient();
    //        var response = await client.PostAsync(getKeyUrl, content);
    //        string requestResult = await response.Content.ReadAsStringAsync();
    //        if (response.IsSuccessStatusCode)
    //        {
    //            await TryLogin(requestResult);
    //        }
    //        else
    //        {
    //            Debug.Log("응답 실패 (키값 받아오기) : " + requestResult);
    //        }
    //    }
    //}//여기 추가할 것

    //// Try Login
    //private async Task TryLogin(string _requestResult)
    //{
    //    RSAPasswordEncrypt rsaPasswordEncrypt = new RSAPasswordEncrypt();
    //    string rsaPassword;
    //    rsaPassword = rsaPasswordEncrypt.GetRSAPassword(_requestResult, ID_TextBox.Text, Password_PasswordBox.Password);

    //    var loginValues = new Dictionary<string, string>
    //        {
    //            { key_id, ID_TextBox.Text },
    //            { key_password, rsaPassword }
    //        };

    //    var content = new FormUrlEncodedContent(loginValues);

    //    HttpClient client = new HttpClient();
    //    var response = await client.PostAsync(loginUrl, content);
    //    string requestResult = await response.Content.ReadAsStringAsync();

    //    if (response.IsSuccessStatusCode)
    //    {
    //        token = requestResult;
    //        Login.Visibility = Visibility.Hidden;
    //        FileCheck.Visibility = Visibility.Visible;
    //        CheckForUpdates();
    //    }
    //    else
    //    {
    //        Debug.Log("응답 실패 (로그인 실패) : " + requestResult);
    //    }
    //}
    //private async Task TryLogin(RSACryptoServiceProvider rsa)
    //{
    //    //RSAPasswordEncrypt rsaPasswordEncrypt = new RSAPasswordEncrypt();
    //    string rsaPassword;
    //    //rsaPassword = rsaPasswordEncrypt.GetRSAPassword(_requestResult, Password_PasswordBox.Password);
    //    rsaPassword = Convert.ToBase64String(rsa.Encrypt((new UTF8Encoding()).GetBytes(Password_PasswordBox.Password), false));

    //    var loginValues = new Dictionary<string, string>
    //        {
    //            { key_id, ID_TextBox.Text },
    //            { key_password, rsaPassword }
    //        };

    //    var content = new FormUrlEncodedContent(loginValues);

    //    HttpClient client = new HttpClient();
    //    var response = await client.PostAsync(loginUrl, content);
    //    string requestResult = await response.Content.ReadAsStringAsync();

    //    if (response.IsSuccessStatusCode)
    //    {
    //        token = requestResult;
    //        Login.Visibility = Visibility.Hidden;
    //        FileCheck.Visibility = Visibility.Visible;
    //        CheckForUpdates();
    //    }
    //    else
    //    {
    //        Debug.Log("응답 실패 (로그인 실패) : " + requestResult);
    //    }
    //}
}
