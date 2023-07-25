using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json.Linq;
using UnityEngine;

public class Checksum
{
    public static UniTask<int> ChecksumMD5(JObject json, string rootPath)
    {
        var result = new List<string>();
        foreach (JProperty prop in json.Properties())
        {
            byte[] btFile = File.ReadAllBytes(rootPath + prop.Name);
            byte[] btHash = MD5.Create().ComputeHash(btFile);

            if (Convert.ToBase64String(btHash) != prop.Value.ToString())
            {
                //Debug.Log($"{Convert.ToBase64String(btHash)}, {prop.Value.ToString()}");
                result.Add(prop.Name);
            }
        }
        var utc = new UniTaskCompletionSource<int>();
        utc.TrySetResult(result.Count);
        return utc.Task;
    }
}
