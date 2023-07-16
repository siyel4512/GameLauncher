using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using UnityEngine;
using Ookii.Dialogs;
using TMPro;
using Org.BouncyCastle.Asn1.BC;

public class FolderDialog : MonoBehaviour
{
    [DllImport("user32.dll")]
    private static extern IntPtr GetActiveWindow();

    public int popupNum; // Folder Dialog Number
    public TMP_InputField installPath_text; // Install Path

    private void OnEnable()
    {
        // set select install path
        installPath_text.text = GameManager.instance.filePath.ExeFolderPaths[popupNum];
    }

    public void BTN_OpenFolderDialog()
    {
        VistaFolderBrowserDialog openDialog = new VistaFolderBrowserDialog();
        openDialog.Description = "Select Folder";
        openDialog.UseDescriptionForTitle = true;

        switch (popupNum)
        {
            case 0:
                openDialog.SelectedPath = GameManager.instance.filePath.LoadPath().pcPath + "\\";
                break;
            case 1:
                openDialog.SelectedPath = GameManager.instance.filePath.LoadPath().vrPath + "\\";
                break;
            case 2:
                openDialog.SelectedPath = GameManager.instance.filePath.LoadPath().ugcPath + "\\";
                break;
            case 3:
                openDialog.SelectedPath = GameManager.instance.filePath.LoadPath().batchPath + "\\";
                break;
        }

        if (openDialog.ShowDialog(new WindowWrapper(GetActiveWindow())) == DialogResult.OK)
        {
            Debug.Log(openDialog.SelectedPath);

            // save select install path
            GameManager.instance.filePath.SavePath(popupNum, openDialog.SelectedPath);

            // set select install path
            installPath_text.text = GameManager.instance.filePath.ExeFolderPaths[popupNum];
        }
    }

    public void BTN_ClosePopup()
    {
        gameObject.SetActive(false);
    }
}

public class WindowWrapper : IWin32Window
{
    public WindowWrapper(IntPtr handle)
    {
        _hwnd = handle;
    }

    public IntPtr Handle
    {
        get { return _hwnd; }
    }

    private IntPtr _hwnd;
}
