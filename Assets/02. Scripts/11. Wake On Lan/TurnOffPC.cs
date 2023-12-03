using UnityEngine;
using System.Diagnostics;

using Debug = UnityEngine.Debug;

public class TurnOffPC : MonoBehaviour
{
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

            //if ()
            {
                Debug.Log("PC off");
            }
        }
    }

    public void TurnOff()
    {
        Process.Start("shutdown.exe", "/s /t 0");
    }
}
