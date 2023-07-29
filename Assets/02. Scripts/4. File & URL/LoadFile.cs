using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

// 삭제 예정
public class LoadFile : MonoBehaviour
{
    // read file content
    protected string[] ParsingData()
    {
        StreamReader streamReader = new StreamReader(Application.dataPath + "/StreamingAssets" + "/" + "SettingValues.txt");
        string content = streamReader.ReadToEnd();
        streamReader.Close();

        char[] delims = new[] { '\r', '\n' };
        string[] parsingDate = content.Split(delims, StringSplitOptions.RemoveEmptyEntries);

        return parsingDate;
    }
}
