using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using System.Text.RegularExpressions;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System;
using Unity.VisualScripting.Antlr3.Runtime;
using Cysharp.Threading.Tasks;
using System.IO;


using Newtonsoft.Json.Linq;
using System.Net;
using System.ComponentModel;
using System.Diagnostics;
using System.IO.Compression;
using System.Linq;
//using UnityEngine.UIElements;

using Debug = UnityEngine.Debug;

public enum LauncherStatus
{
    ready,
    failed,
    downloadingGame,
    downloadingUpdate
}

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

        loginButton.onClick.AddListener(TestRequestKey);
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

    private void TestRequestKey()
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
    private TCP_Server tcp_Server;
    public static string token;

    private LauncherStatus _status;
    internal LauncherStatus Status
    {
        get => _status;
        set
        {
            _status = value;
            switch (_status)
            {
                case LauncherStatus.ready:
                    SetPage(false, "Play");

                    // start TCP server
                    tcp_Server.StartServer();
                    break;
                case LauncherStatus.failed:
                    SetPage(false, "Update Failed - Retry");
                    break;
                case LauncherStatus.downloadingGame:
                    SetPage(true, "Downloading Game");
                    break;
                case LauncherStatus.downloadingUpdate:
                    SetPage(true, "Downloading Update");
                    break;
                default:
                    break;
            }
        }
    }

    private async UniTask RequestKey()
    {

        var idValue = new Dictionary<string, string>
            {
                { key_id, id.text }
            };

        string keyFilePath = Environment.CurrentDirectory + "\\KEY\\" + id.text + ".pem";
        if (File.Exists(keyFilePath))
        {
            await TryLogin(RSAPasswordEncrypt.ImportPublicKey(File.ReadAllText(keyFilePath)));
        }
        else
        {
            var content = new FormUrlEncodedContent(idValue);
            HttpClient client = new HttpClient();
            var response = await client.PostAsync(getKeyUrl, content);
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
    }//여기 추가할 것

    // Try Login
    private async UniTask TryLogin(string _requestResult)
    {
        RSAPasswordEncrypt rsaPasswordEncrypt = new RSAPasswordEncrypt();
        string rsaPassword;
        rsaPassword = rsaPasswordEncrypt.GetRSAPassword(_requestResult, id.text, password.text);

        var loginValues = new Dictionary<string, string>
            {
                { key_id, id.text },
                { key_password, rsaPassword }
            };

        var content = new FormUrlEncodedContent(loginValues);

        HttpClient client = new HttpClient();
        var response = await client.PostAsync(loginUrl, content);
        string requestResult = await response.Content.ReadAsStringAsync();

        if (response.IsSuccessStatusCode)
        {
            token = requestResult;
            //Login.Visibility = Visibility.Hidden;
            //FileCheck.Visibility = Visibility.Visible;
            CheckForUpdates();
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
                { key_id, id.text },
                { key_password, rsaPassword }
            };

        var content = new FormUrlEncodedContent(loginValues);

        HttpClient client = new HttpClient();
        var response = await client.PostAsync(loginUrl, content);
        string requestResult = await response.Content.ReadAsStringAsync();

        if (response.IsSuccessStatusCode)
        {
            token = requestResult;
            //Login.Visibility = Visibility.Hidden;
            //FileCheck.Visibility = Visibility.Visible;
            CheckForUpdates();
        }
        else
        {
            Debug.Log("응답 실패 (로그인 실패) : " + requestResult);
        }
    }

    #region Change file load Page
    // Set launcher UI layout
    private void SetPage(bool _isLoading, string _stateText)
    {
        //if (_isLoading)
        //{
        //    StateText.Visibility = Visibility.Visible;
        //    StateText.Text = _stateText;
        //    PrograssBar.Visibility = Visibility.Visible;

        //    PlayButton.Visibility = Visibility.Hidden;
        //}
        //else
        //{
        //    StateText.Visibility = Visibility.Hidden;
        //    PrograssBar.Visibility = Visibility.Hidden;

        //    PlayButton.Visibility = Visibility.Visible;
        //    PlayButton.Content = _stateText;
        //}
    }
    #endregion

    #region File Check
    private void CheckForUpdates()
    {
        if (Directory.Exists(gameBuildPath))
        {
            try
            {
                WebClient webClient = new WebClient();
                var onlineJson = webClient.DownloadString(jsonFileUrl); // download json file

                JObject jObject = JObject.Parse(onlineJson);
                int resultCount = ChecksumMD5(jObject, gameBuildPath);

                if (resultCount == 0)
                {
                    Status = LauncherStatus.ready;
                }
                else
                {
                    InstallGameFiles(true);
                }
            }
            catch (Exception ex)
            {
                Status = LauncherStatus.failed;
                Debug.LogError($"Error checking for game updates: {ex}");
            }
        }
        else
        {
            InstallGameFiles(false);
        }
    }
    #endregion

    #region File Download
    private void InstallGameFiles(bool _isUpdate)
    {
        try
        {
            WebClient webClient = new WebClient();

            if (_isUpdate)
            {
                Status = LauncherStatus.downloadingUpdate;
            }
            else
            {
                Status = LauncherStatus.downloadingGame;
            }

            webClient.DownloadProgressChanged += new DownloadProgressChangedEventHandler(DownloadProgressCallback);
            webClient.DownloadFileCompleted += new AsyncCompletedEventHandler(DownloadGameCompletedCallback);
            webClient.DownloadFileAsync(new Uri(buildFileUrl), gameZipPath); // download build file
        }
        catch (Exception ex)
        {
            Status = LauncherStatus.failed;
            Debug.Log($"Error installing game files: {ex}");
        }
    }

    private void DownloadProgressCallback(object sender, DownloadProgressChangedEventArgs e)
    {
        //PrograssBar.Value = e.ProgressPercentage;
        Debug.Log(e.ProgressPercentage);
    }

    private void DownloadGameCompletedCallback(object sender, AsyncCompletedEventArgs e)
    {
        try
        {
            if (Directory.Exists(rootPath + "\\Build"))
            {
                Directory.CreateDirectory(rootPath + "\\Build");
            }
            ZipFile.ExtractToDirectory(gameZipPath, rootPath + "\\Build", true);
            File.Delete(gameZipPath);

            Status = LauncherStatus.ready;
        }
        catch (Exception ex)
        {
            Status = LauncherStatus.failed;
            Debug.Log($"Error finishing download: {ex}");
        }
    }
    #endregion

    #region File Execute
    private void PlayButton_Click(object sender/*, RoutedEventArgs e*/)
    {
        if (File.Exists(gameExePath) && Status == LauncherStatus.ready)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo(gameExePath);
            startInfo.WorkingDirectory = Path.Combine(rootPath, "Build");
            Process.Start(startInfo);

            //Close(); // launcher window close
        }
        else if (Status == LauncherStatus.failed)
        {
            CheckForUpdates();
        }
    }
    #endregion

    #region Checksum
    public static int ChecksumMD5(JObject json, string rootPath)
    {
        var result = new List<string>();
        foreach (JProperty prop in json.Properties())
        {
            byte[] btFile = File.ReadAllBytes(rootPath + prop.Name);
            byte[] btHash = MD5.Create().ComputeHash(btFile);

            if (Convert.ToBase64String(btHash) != prop.Value.ToString())
            {
                Debug.Log($"{Convert.ToBase64String(btHash)}, {prop.Value.ToString()}");
                result.Add(prop.Name);
            }
        }
        Debug.Log(result.Count);
        return result.Count;
    }
    #endregion

    #region Set Setting values
    // read setting file content
    private void SetSettingValues()
    {
        string settingFilePath = ChekTextFile();

        char[] delims = new[] { '\r', '\n' };
        string[] parsingDate = settingFilePath.Split(delims, StringSplitOptions.RemoveEmptyEntries);

        getKeyUrl = parsingDate[0];
        loginUrl = parsingDate[1];
        key_id = parsingDate[2];
        key_password = parsingDate[3];

        rootPath = parsingDate[4];
        gameBuildPath = Path.Combine(rootPath, parsingDate[5]);
        gameZipPath = Path.Combine(rootPath, gameBuildPath + ".zip");
        gameExePath = Path.Combine(rootPath, gameBuildPath, parsingDate[6] + ".exe");

        buildFileUrl = parsingDate[7];
        jsonFileUrl = parsingDate[8];
    }

    // search setting file
    private string ChekTextFile()
    {
        string parentDirectory = Directory.GetParent(Directory.GetCurrentDirectory()).FullName;

        while (!Directory.GetFiles(parentDirectory, "*.sln").Any())
        {
            parentDirectory = Directory.GetParent(parentDirectory).FullName;
        }

        return File.ReadAllText(Path.Combine(parentDirectory, "SettingValues.txt"));
    }
    #endregion

    private void Window_Closed(object sender, EventArgs e)
    {
        if (tcp_Server.Server != null) tcp_Server.Server.Stop();
        if (tcp_Server.Client != null) tcp_Server.Client.Close();
    }
}
