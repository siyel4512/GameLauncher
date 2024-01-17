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
            //API.instance.Request_LauncherUserGuideLink("ko").Forget();
            //API.instance.Request_UgcInstallMenualLink("ko").Forget();
            Debug.Log(await API.instance.Request_EnglishVideoLink("ko"));
        }
    }
}
