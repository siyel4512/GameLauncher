using System.IO;
using UnityEngine;
using TMPro;

public class VersionManager : MonoBehaviour
{
    public Version version;
    public TMP_Text versionText_Login;
    public TMP_Text versionText_MainPage;

    // Start is called before the first frame update
    void Start()
    {
        if (DEV.instance.isUpdateLauncher)
        {
            // set launcher version
            SetVersion(LoadVersion());

            ////check launcher version
            //GameManager.instance.api.LauncherVersionCheck().Forget();
        }
    }

    // show version value
    public void SetVersion(Version _version)
    {
        versionText_Login.text = $"v{_version.major}.{_version.minor}.{_version.patch}";
        versionText_MainPage.text = $"v{_version.major}.{_version.minor}.{_version.patch}";
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
        version.major = version.minor = version.patch = "0";

        string jsonData = JsonUtility.ToJson(version, true);
        string path = Path.Combine(Application.streamingAssetsPath + "/Version", "version.json");
        File.WriteAllText(path, jsonData);
    }
}

[System.Serializable]
public class Version
{
    public string major;
    public string minor;
    public string patch;
}
