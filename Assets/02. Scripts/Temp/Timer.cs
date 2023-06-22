using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;

using Debug = UnityEngine.Debug;

public class Timer : MonoBehaviour
{
    private Stopwatch sw;

    public int limitTime;

    public int currentTimeCount;

    // Start is called before the first frame update
    void Start()
    {
        sw = new Stopwatch();
        sw.Start();
    }

    // Update is called once per frame
    void Update()
    {
        currentTimeCount = limitTime - (int)(sw.ElapsedMilliseconds / 1000f);
        Debug.Log("Timer Count : " + currentTimeCount);
    }
}
