using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using TMPro;
using System;

public enum VersionUpdateType
{
    Major,
    Minor,
    Patch
}

public class VersionManager : MonoBehaviour
{
    public Version version;
    public TMP_Text versionText_Login;
    public TMP_Text versionText_MainPage;

    // Start is called before the first frame update
    void Start()
    {
        SetVersion(LoadVersion());
    }

    // Update is called once per frame
    void Update()
    {
        // Test version Value
        if (Input.GetKeyDown(KeyCode.Q))
        {
            SaveVersion(VersionUpdateType.Major);
            SetVersion(LoadVersion());
        }

        if (Input.GetKeyDown(KeyCode.W))
        {
            SaveVersion(VersionUpdateType.Minor);
            SetVersion(LoadVersion());
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            SaveVersion(VersionUpdateType.Patch);
            SetVersion(LoadVersion());
        }
    }

    // show version value
    public void SetVersion(Version _version)
    {
        versionText_Login.text = $"{_version.major}.{_version.minor}.{_version.patch}";
        versionText_MainPage.text = $"{_version.major}.{_version.minor}.{_version.patch}";
    }

    public void SaveVersion(VersionUpdateType updateType)
    {
        switch (updateType)
        {
            case VersionUpdateType.Major:
                version.major++;
                version.minor = version.patch = 0;
                break;
            case VersionUpdateType.Minor:
                version.minor++;
                version.patch = 0;
                break;
            case VersionUpdateType.Patch:
                version.patch++;
                break;
        }

        string jsonData = JsonUtility.ToJson(version, true);
        string path = Path.Combine(Application.streamingAssetsPath + "/Version", "version.json");
        File.WriteAllText(path, jsonData);
    }

    // load version value
    public Version LoadVersion()
    {
        string path = Path.Combine(Application.streamingAssetsPath + "/Version", "version.json");
        string jsonData = File.ReadAllText(path);
        version = JsonUtility.FromJson<Version>(jsonData);

        return version;
    }

    // reset current version value
    public void ResetVersion()
    {
        version.major = 1;
        version.minor = version.patch = 0;

        string jsonData = JsonUtility.ToJson(version, true);
        string path = Path.Combine(Application.streamingAssetsPath + "/Version", "version.json");
        File.WriteAllText(path, jsonData);
    }
}

[System.Serializable]
public class Version
{
    public int major;
    public int minor;
    public int patch;
}
