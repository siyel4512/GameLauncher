using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

using Debug = UnityEngine.Debug;

public class PCPowerManager : MonoBehaviour
{
    // turn off pc setting
    [Space(10)]
    [Header("[ Request Timer ]")]
    public int currentTimeCount;
    public int limitTime;
    public bool isUsingRequestTimer;
    public Stopwatch sw;

    // Start is called before the first frame update
    void Start()
    {
        sw = new Stopwatch();
        sw.Start();
    }

    // Update is called once per frame
    void Update()
    {
        RequestTimer();
    }

    public void RequestTimer()
    {
        currentTimeCount = limitTime - (int)(sw.ElapsedMilliseconds / 1000f);

        // time out
        if (currentTimeCount < 0)
        {
            currentTimeCount = limitTime;
            sw.Restart();

            GameManager.instance.api.Request_turnOffPC().Forget();
        }
    }

    public void TurnOff()
    {
        Process.Start("shutdown.exe", "/s /t 0");
    }
}
