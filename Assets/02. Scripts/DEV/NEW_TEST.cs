using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NEW_TEST : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    async void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("API Å×½ºÆ®...");
            Debug.Log("Launcher User Guide : " + await API.instance.Request_LauncherUserGuideLink("ko"));
            Debug.Log("Ugc Install Menual : " + await API.instance.Request_UGCInstallMenualLink("ko"));
            Debug.Log("English Video : " + await API.instance.Request_EnglishVideoLink("ko"));
        }
    }
}
