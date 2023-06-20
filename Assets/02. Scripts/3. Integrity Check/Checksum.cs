using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using Newtonsoft.Json.Linq;
using UnityEngine;

public class Checksum
{
    public static int ChecksumMD5(JObject json, string rootPath)
    {
        var result = new List<string>();
        foreach (JProperty prop in json.Properties())
        {
            byte[] btFile = File.ReadAllBytes(rootPath + prop.Name);
            byte[] btHash = MD5.Create().ComputeHash(btFile);

            if (Convert.ToBase64String(btHash) != prop.Value.ToString())
            {
                Debug.Log($"{Convert.ToBase64String(btHash)}, {prop.Value.ToString()}");
                result.Add(prop.Name);
            }
        }
        return result.Count;
    }
}
