using Org.BouncyCastle.Asn1.Mozilla;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using static Unity.VisualScripting.Member;

public class FilePath : MonoBehaviour
{
    public static FilePath Instance;

    // login
    private string getKeyUrl;
    private string loginUrl;
    private string key_id;
    private string key_password;
    public string GetKeyUrl => getKeyUrl;
    public string LoginUrl => loginUrl;
    public string Key_id => key_id;
    public string Key_password => key_password;

    // file download path
    private string rootPath;
    private string gameBuildPath;
    private string gameZipPath;
    public string RootPath => rootPath;
    public string GameBuildPath => gameBuildPath;
    public string GameZipPath => gameZipPath;

    // file download
    private string buildFileUrl;
    private string jsonFileUrl;
    public string BuildFileUrl => buildFileUrl;
    public string JsonFileUrl => jsonFileUrl;


    // file download
    public string[] buildFileUrls = new string[4];
    public string[] jsonFileUrls = new string[4];

    public string[] gameBuildPaths = new string[4];
    public string[] gameZipPaths = new string[4];

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
        
    }

    // read setting file content
    public void SetSettingValues()
    {
        //-------------------------------- °ø¿ë -------------------------------------//
        string settingFilePath = ChekTextFile();

        char[] delims = new[] { '\r', '\n' };
        string[] parsingDate = settingFilePath.Split(delims, StringSplitOptions.RemoveEmptyEntries);

        getKeyUrl = parsingDate[0];
        loginUrl = parsingDate[1];
        key_id = parsingDate[2];
        key_password = parsingDate[3];

        rootPath = parsingDate[4];
        //--------------------------------------------------------------------------//

        buildFileUrl = parsingDate[5];
        jsonFileUrl = parsingDate[6];

        string[] buildFileFullName = buildFileUrl.Split("/");
        string[] buildFileName = buildFileFullName[buildFileFullName.Length - 1].Split(".");

        gameBuildPath = Path.Combine(rootPath, buildFileName[0]);
        gameZipPath = Path.Combine(rootPath, gameBuildPath + ".zip");
    }

    // search setting file
    private string ChekTextFile()
    {
        //string parentDirectory = Directory.GetParent(Directory.GetCurrentDirectory()).FullName;

        //while (!Directory.GetFiles(parentDirectory, "*.sln").Any())
        //{
        //    parentDirectory = Directory.GetParent(parentDirectory).FullName;
        //}

        //return File.ReadAllText(Path.Combine(parentDirectory, "SettingValues.txt"));
       
        StreamReader streamReader = new StreamReader(Application.dataPath + "/StreamingAssets" + "/" + "SettingValues.txt");
        string content = streamReader.ReadToEnd();
        streamReader.Close();
        
        return content;
    }
}
