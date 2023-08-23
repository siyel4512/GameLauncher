using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using UnityEngine;
using Ookii.Dialogs;
using TMPro;
using System.IO;
using System.Diagnostics;

using Application = UnityEngine.Application;
using Debug = UnityEngine.Debug;

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
        //ResetUGCFilePath();
        unityProjectExeFilePath_text.text = LoadUGCFilePath().unityProjectExeFilePath;
        objectUGCProjectDownloadPath_text.text = LoadUGCFilePath().objectUGCProjectDownloadPath;
    }

    public void Update()
    {
        if (Input.GetKeyUp(KeyCode.Space))
        {
            ////Debug.Log(@"C:\Curiverse\startObjectUGC.bat".Replace('\\', Path.DirectorySeparatorChar));
            //Debug.Log(LoadUGCFilePath().objectUGCProjectDownloadPath + "\\startObjectUGC.bat");
            Debug.Log("[ugc] " + Path.Combine(LoadUGCFilePath().objectUGCProjectDownloadPath, "startObjectUGC.bat"));

            if (File.Exists(Path.Combine(LoadUGCFilePath().objectUGCProjectDownloadPath, "startObjectUGC.bat")))
            {
                Debug.Log("[ugc] 배치 파일 있음");

                ProcessStartInfo startInfo = new ProcessStartInfo(Path.Combine(LoadUGCFilePath().objectUGCProjectDownloadPath, "startObjectUGC.bat"));
                startInfo.WorkingDirectory = LoadUGCFilePath().objectUGCProjectDownloadPath;
                Process.Start(startInfo);
            }
            else
            {
                Debug.Log("[ugc] 배치 파일 없음");
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            CreateBatchFile();
        }
    }

    public void BTN_OpenFileDialog()
    {
        VistaOpenFileDialog openDialog = new VistaOpenFileDialog();
        openDialog.Multiselect = false;
        openDialog.Title = "Select File";
        openDialog.Filter = "All Files (*.*)|*.*|txt files (*.txt)|*.txt";
        openDialog.FilterIndex = 0;
        openDialog.RestoreDirectory = true;

        // open path setting
        string[] splitPaths = LoadUGCFilePath().unityProjectExeFilePath.Split('\\');
        string openDirectoryPath = "";

        for (int i = 0; i < splitPaths.Length - 1; i++)
        {
            openDirectoryPath += (splitPaths[i] + "\\");
        }

        openDialog.FileName = openDirectoryPath; // 폴더 경로로 지정할것

        string tempPath = LoadUGCFilePath().objectUGCProjectDownloadPath + "\\";

        if (openDialog.ShowDialog(new WindowWrapper(GetActiveWindow())) == DialogResult.OK)
        {
            if (File.Exists(Path.Combine(tempPath, "startObjectUGC.bat")))
            {
                // 배치 파일 삭제
                Debug.Log("배치 파일 삭제");
                File.Delete(Path.Combine(tempPath, "startObjectUGC.bat"));
            }

            //Debug.Log(openDialog.FileName);

            // save select install path
            SaveUGCFilePath(1, openDialog.FileName);

            // set select install path
            unityProjectExeFilePath_text.text = LoadUGCFilePath().unityProjectExeFilePath;
        }
    }

    // open folder dialog
    public void BTN_OpenFolderDialog()
    {
        VistaFolderBrowserDialog openDialog = new VistaFolderBrowserDialog();
        openDialog.Description = "Select Folder";
        openDialog.UseDescriptionForTitle = true;

        // open path setting
        openDialog.SelectedPath = LoadUGCFilePath().objectUGCProjectDownloadPath + "\\";

        string tempPath = openDialog.SelectedPath;

        if (openDialog.ShowDialog(new WindowWrapper(GetActiveWindow())) == DialogResult.OK)
        {
            if (File.Exists(Path.Combine(tempPath, "startObjectUGC.bat")))
            {
                // 배치 파일 삭제
                Debug.Log("배치 파일 삭제");
                File.Delete(Path.Combine(tempPath, "startObjectUGC.bat"));
            }

            //Debug.Log(openDialog.SelectedPath);

            // save select install path
            SaveUGCFilePath(2, openDialog.SelectedPath);
            
            // set select install path
            objectUGCProjectDownloadPath_text.text = LoadUGCFilePath().objectUGCProjectDownloadPath;
            FilePath.Instance.rootPaths[2] = LoadUGCFilePath().objectUGCProjectDownloadPath;
            FilePath.Instance.ExeFolderPaths[2] = Path.Combine(FilePath.Instance.rootPaths[2], FilePath.Instance.ExeFolderNames[2]);
            FilePath.Instance.ExeZipFilePaths[2] = Path.Combine(FilePath.Instance.rootPaths[2], FilePath.Instance.ExeFolderPaths[2] + ".zip");
        }
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

    public void CreateBatchFile()
    {
        // batch 파일 삭제

        string directoryPath = LoadUGCFilePath().objectUGCProjectDownloadPath;
        //string batchFilePath = @"C:\Curiverse\startObjectUGC.bat".Replace('\\', Path.DirectorySeparatorChar);
        string batchFilePath = Path.Combine(LoadUGCFilePath().objectUGCProjectDownloadPath, "startObjectUGC.bat");

        // 유니티 프로젝트 설치 위치(사용자로부터 입력)
        //string unityPath = "C:\\Program Files\\Unity\\Hub\\Editor\\2020.3.38f1\\Editor\\Unity.exe";
        string unityPath = LoadUGCFilePath().unityProjectExeFilePath;
        string[] split_unityPath = unityPath.Split('\\');

        if (split_unityPath[split_unityPath.Length - 1] != "Unity.exe")
        {
            Debug.Log("[UGC] 유니티 프로젝트 실행파일 아님");
            return;
        }
        else
        {
            if (File.Exists(unityPath))
            {
                Debug.Log("[UGC] 유니티 프로젝트 실행파일 존재");
            }
            else
            {
                Debug.Log("[UGC] 해당 경로에 유니티 프로젝트 실행파일 없음");
                return;
            }
        }

        // 오브젝트UGC 프로젝트 다운로드 위치(사용자로부터 입력)
        //string projectPath = "C:\\Users\\JYWON\\Documents\\Unity Projects\\Curiverse Object";
        string projectPath = Path.Combine(LoadUGCFilePath().objectUGCProjectDownloadPath, FilePath.Instance.ExeFolderPaths[2]);
        //string projectPath = Path.Combine(LoadUGCFilePath().objectUGCProjectDownloadPath, "Shared-Mode (2)");

        if (Directory.Exists(projectPath))
        {
            Debug.Log("[UGC] 해당 경로에 프로젝트 있음");
        }
        else
        {
            Debug.Log("[UGC] 해당 경로에 프로젝트 없음");
            return;
        }

        if (!Directory.Exists(directoryPath))
            Directory.CreateDirectory(directoryPath);

        string batchOrder = "@echo off\n" +
                            "start \"\" \"" + unityPath + "\" -projectPath \"" + projectPath +
                            "\"\nexit";

        File.WriteAllText(batchFilePath, batchOrder);

        Debug.Log("배치 파일 생성 완료");
    }
}
//---------------------------------------------------------------//

[System.Serializable]
public class UGCFilePath
{
    public string unityProjectExeFilePath;
    public string objectUGCProjectDownloadPath;
}
