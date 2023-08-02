using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ExecuteButton : MonoBehaviour
{
    public FileDownload fileDownload;
    public TMP_Text buttonText;

    public void OnDisable()
    {
        // reset button text
        //Debug.Log($"[SY] {fileDownload.buttonNum} ��ư ����");
        //buttonText.text = "-";
        buttonText.text = FilePath.Instance.fileCheckText;
    }
}
