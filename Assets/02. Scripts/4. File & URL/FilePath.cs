using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using UnityEditor;
using UnityEngine;
using Cysharp.Threading.Tasks;

public class FilePath : LoadFile
{
    public static FilePath Instance;

    // file download path
    private string rootPath;

    private string[] buildFileUrls = new string[4];
    private string[] jsonFileUrls = new string[4];

    private string[] exeFolderPaths = new string[4];
    private string[] exeZipFilePaths = new string[4];

    public string RootPath => rootPath;
    public string[] BuildFileUrls => buildFileUrls;
    public string[] JsonFileUrls => jsonFileUrls;
    public string[] ExeFolderPaths => exeFolderPaths;
    public string[] ExeZipFilePaths => exeZipFilePaths;

    private string[] exeFolderNames = new string[4];
    public string[] ExeFolderNames => exeFolderNames;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        SetSettingValues();
    }

    // Start is called before the first frame update
    void Start()
    {
        //FilePathCheck(0);
    }

    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.Space))
        //{
        //    Debug.Log("삭제 시작");
        //    Directory.Delete("C:\\Users\\LSY\\Desktop\\TEST", true);
        //    Debug.Log("삭제 완료");
        //}
    }

    // read setting file content
    public void SetSettingValues()
    {
        //---------- old version ----------//
        string[] parsingData = ParsingData();

        rootPath = parsingData[2];

        buildFileUrls[0] = parsingData[5];
        buildFileUrls[1] = parsingData[7];
        buildFileUrls[2] = parsingData[9];
        buildFileUrls[3] = parsingData[11];

        jsonFileUrls[0] = parsingData[6];
        jsonFileUrls[1] = parsingData[8];
        jsonFileUrls[2] = parsingData[10];
        jsonFileUrls[3] = parsingData[12];

        for (int i = 0; i < 4; i++)
        {
            string[] folderFullName = buildFileUrls[i].Split("/");
            string[] exeFolderName = folderFullName[folderFullName.Length - 1].Split(".");
            
            exeFolderNames[i] = exeFolderName[0];
            exeFolderPaths[i] = Path.Combine(rootPath, exeFolderName[0]);
            exeZipFilePaths[i] = Path.Combine(rootPath, exeFolderPaths[i] + ".zip");
        }
        //---------------------------------//

        //---------- new version ----------//

        //---------------------------------//
    }

    //---------------------------------------------//
    //---------------------------------------------//
    //---------------------------------------------//
    //---------------------------------------------//
    //---------------------------------------------//

    public string[] currentExeFileURL;
    //public string[] newExeFileURL;

    public string[] currentJsonFileURL;
    //public string[] newJsonFileURL;

    // Todo : file path check Function modify
    private async void FilePathCheck(int _index)
    {
        // Todo : 
        currentExeFileURL = new string[4];

        currentJsonFileURL = new string[4];

        if (currentExeFileURL[_index] != "")
        {
            Debug.Log("value is null...");

            // set path
            currentExeFileURL[_index] = await RequestExeFilePathURL();
            
            string[] folderFullName = currentExeFileURL[_index].Split("/");
            string[] exeFolderName = folderFullName[folderFullName.Length - 1].Split(".");

            exeFolderNames[_index] = exeFolderName[0];
            exeFolderPaths[_index] = Path.Combine(rootPath, exeFolderName[0]);
            exeZipFilePaths[_index] = Path.Combine(rootPath, exeFolderPaths[_index] + ".zip");

            // set .json file path
            currentJsonFileURL[_index] = await RequestJsonFilePathURL();

            return;
        }

        string newExeFileURL = await RequestExeFilePathURL();

        if (currentExeFileURL[_index] != newExeFileURL)
        {
            Debug.Log("value is different...");

            // file delete
            if (Directory.Exists(ExeFolderPaths[_index]))
            {
                Directory.Delete(ExeFolderPaths[_index], true);
            }

            // set .exe file path
            currentExeFileURL[_index] = newExeFileURL;

            string[] folderFullName = currentExeFileURL[_index].Split("/");
            string[] exeFolderName = folderFullName[folderFullName.Length - 1].Split(".");

            exeFolderNames[_index] = exeFolderName[0];
            exeFolderPaths[_index] = Path.Combine(rootPath, exeFolderName[0]);
            exeZipFilePaths[_index] = Path.Combine(rootPath, exeFolderPaths[_index] + ".zip");

            // set .json file path
            currentJsonFileURL[_index] = await RequestJsonFilePathURL();
        }
    }
    
    private async UniTask<string> RequestExeFilePathURL()
    {
        var idValue = new Dictionary<string, string>
        {
            { "dd", "dd" }
        };

        var content = new FormUrlEncodedContent(idValue);

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

        return requestResult;
    }

    private async UniTask<string> RequestJsonFilePathURL()
    {
        var idValue = new Dictionary<string, string>
        {
            { "dd", "dd" }
        };

        var content = new FormUrlEncodedContent(idValue);

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

        return requestResult;
    }
}
