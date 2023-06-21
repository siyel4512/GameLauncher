using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class FilePath : LoadFile
{
    public static FilePath Instance;

    // file download path
    private string rootPath;

    private string[] buildFileUrls = new string[4];
    private string[] jsonFileUrls = new string[4];

    private string[] gameBuildPaths = new string[4];
    private string[] gameZipPaths = new string[4];

    public string RootPath => rootPath;
    public string[] BuildFileUrls => buildFileUrls;
    public string[] JsonFileUrls => jsonFileUrls;
    public string[] GameBuildPaths => gameBuildPaths;
    public string[] GameZipPaths => gameZipPaths;

    private string[] buildFileNames = new string[4];
    public string[] BuildFileNames => buildFileNames;

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
            string[] buildFileFullName = buildFileUrls[i].Split("/");
            string[] buildFileName = buildFileFullName[buildFileFullName.Length - 1].Split(".");
            
            buildFileNames[i] = buildFileName[0];
            gameBuildPaths[i] = Path.Combine(rootPath, buildFileName[0]);
            gameZipPaths[i] = Path.Combine(rootPath, gameBuildPaths[i] + ".zip");
        }
    }
}
