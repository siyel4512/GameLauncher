using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    //public static GameManager Instance;

    //// login
    //public string getKeyUrl;
    //public string loginUrl;
    //public string key_id;
    //public string key_password;

    //public string GetkeyUrl => getKeyUrl;
    //public string LoginUrl => loginUrl;
    //public string Key_id => key_id;
    //public string Key_password => key_password;

    //// file download path
    //public string rootPath;
    //public string gameBuildPath;
    //public string gameZipPath;
    //public string gameExePath;

    //// file download
    //public string buildFileUrl;
    //public string jsonFileUrl;

    //// Start is called before the first frame update
    //void Start()
    //{
    //    if (Instance == null)
    //    {
    //        Instance = new GameManager();
    //    }
    //    else
    //    {
    //        Destroy(gameObject);
    //    }
    //}

    //// read setting file content
    //public void SetSettingValues()
    //{
    //    string settingFilePath = ChekTextFile();

    //    char[] delims = new[] { '\r', '\n' };
    //    string[] parsingDate = settingFilePath.Split(delims, StringSplitOptions.RemoveEmptyEntries);

    //    getKeyUrl = parsingDate[0];
    //    loginUrl = parsingDate[1];
    //    key_id = parsingDate[2];
    //    key_password = parsingDate[3];

    //    rootPath = parsingDate[4];
    //    gameBuildPath = Path.Combine(rootPath, parsingDate[5]);
    //    gameZipPath = Path.Combine(rootPath, gameBuildPath + ".zip");
    //    gameExePath = Path.Combine(rootPath, gameBuildPath, parsingDate[6] + ".exe");

    //    buildFileUrl = parsingDate[7];
    //    jsonFileUrl = parsingDate[8];
    //}

    //// search setting file
    //private string ChekTextFile()
    //{
    //    string parentDirectory = Directory.GetParent(Directory.GetCurrentDirectory()).FullName;

    //    while (!Directory.GetFiles(parentDirectory, "*.sln").Any())
    //    {
    //        parentDirectory = Directory.GetParent(parentDirectory).FullName;
    //    }

    //    return File.ReadAllText(Path.Combine(parentDirectory, "SettingValues.txt"));
    //}
}
