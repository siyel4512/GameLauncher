using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class URL : LoadFile
{
    public static URL Instance;

    // login
    private string getKeyUrl;
    private string loginUrl;
    private string key_id;
    private string key_password;

    public string GetKeyUrl => getKeyUrl;
    public string LoginUrl => loginUrl;
    public string Key_id => key_id;
    public string Key_password => key_password;

    // friend list
    public string friendList;

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

        key_id = parsingData[0];
        key_password = parsingData[1];

        getKeyUrl = parsingData[3];
        loginUrl = parsingData[4];
    }
}
