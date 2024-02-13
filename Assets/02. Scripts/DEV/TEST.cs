using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TEST : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            //API.instance.Request_UrgentNotice().Forget();
            GameManager.instance.urgentNoticeManager.CreateAllContents();
            //GameManager.instance.urgentNoticeManager.BTN_Close();
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            GameManager.instance.urgentNoticeManager.urgentNoticeUI.DeleteContents();
            //GameManager.instance.urgentNoticeManager.urgentNoticeUI.TEST_Coroutine();

            //DateTime currentTime = DateTime.Now;
            //Debug.Log($"날짜 체크 : {currentTime.Year} / {currentTime.Month} / {currentTime.Day}");
        }
    }
}
