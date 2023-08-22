using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using UnityEngine;
using Ookii.Dialogs;
using TMPro;
using System.IO;

using Application = UnityEngine.Application;

public class UGCManager : MonoBehaviour
{
    [DllImport("user32.dll")]
    private static extern IntPtr GetActiveWindow();

    public int popupNum; // Folder Dialog Number

    public TMP_InputField unityProjectExeFilePath_text; // unity project EXE file path
    public TMP_InputField objectUGCProjectDownloadPath_text; // object UGC Project download path


    public string defaultDataPath = "C:\\Curiverse";
    public FileDownload fileDownload;

    public void Start()
    {
        ResetUGCFilePath();
    }

    //private void OnEnable()
    //{
    //    // set select install path
    //    installPath_text.text = GameManager.instance.filePath.ExeFolderPaths[popupNum];
    //}

    public void BTN_Oopn()
    {
        VistaFolderBrowserDialog openDialog = new VistaFolderBrowserDialog();
        openDialog.Description = "Select Folder";
        openDialog.UseDescriptionForTitle = true;

        openDialog.SelectedPath = LoadUGCFilePath().unityProjectExeFilePath + "\\";

        if (openDialog.ShowDialog(new WindowWrapper(GetActiveWindow())) == DialogResult.OK)
        {
            Debug.Log(openDialog.SelectedPath);

            // save select install path
            SaveUGCFilePath(1, openDialog.SelectedPath);

            // set select install path
            unityProjectExeFilePath_text.text = GameManager.instance.filePath.ExeFolderPaths[popupNum];
        }
    }

    // open folder dialog
    public void BTN_OpenFolderDialog()
    {
        VistaFolderBrowserDialog openDialog = new VistaFolderBrowserDialog();
        openDialog.Description = "Select Folder";
        openDialog.UseDescriptionForTitle = true;

        openDialog.SelectedPath = LoadUGCFilePath().objectUGCProjectDownloadPath + "\\";

        if (openDialog.ShowDialog(new WindowWrapper(GetActiveWindow())) == DialogResult.OK)
        {
            Debug.Log(openDialog.SelectedPath);

            // save select install path
            SaveUGCFilePath(2, openDialog.SelectedPath);

            // set select install path
            objectUGCProjectDownloadPath_text.text = GameManager.instance.filePath.ExeFolderPaths[popupNum];
        }
    }

    public void BTN_ClosePopup()
    {
        gameObject.SetActive(false);
    }

    //---------------------------------------------------------------//
    public UGCFilePath ugcFilePath;

    public void SaveUGCFilePath(int i, string newDonwloadURL)
    {
        switch(i)
        {
            case 1:
                ugcFilePath.unityProjectExeFilePath = newDonwloadURL;
                break;
            case 2:
                ugcFilePath.objectUGCProjectDownloadPath = newDonwloadURL;
                break;
        }

        string jsonData = JsonUtility.ToJson(ugcFilePath, true);
        string path = Path.Combine(Application.streamingAssetsPath + "/Data Path", "ugcFilePath.json");
        File.WriteAllText(path, jsonData);
    }

    public UGCFilePath LoadUGCFilePath()
    {
        string path = Path.Combine(Application.streamingAssetsPath + "/Data Path", "ugcFilePath.json");
        string jsonData = File.ReadAllText(path);
        ugcFilePath = JsonUtility.FromJson<UGCFilePath>(jsonData);

        return ugcFilePath;
    }

    public void ResetUGCFilePath()
    {
        Debug.Log("ResetUGCFilePath()");
        ugcFilePath.unityProjectExeFilePath = defaultDataPath;
        ugcFilePath.objectUGCProjectDownloadPath = defaultDataPath;

        string jsonData = JsonUtility.ToJson(ugcFilePath, true);
        string path = Path.Combine(Application.streamingAssetsPath + "/Data Path", "ugcFilePath.json");
        File.WriteAllText(path, jsonData);
    }
}

[System.Serializable]
public class UGCFilePath
{
    public string unityProjectExeFilePath;
    public string objectUGCProjectDownloadPath;
}
