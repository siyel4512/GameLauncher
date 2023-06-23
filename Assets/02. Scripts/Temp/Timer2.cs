using System.Timers;
using UnityEngine;
using Cysharp.Threading.Tasks;

public class Timer2 : MonoBehaviour
{
    Timer timer;
    public int refleshTime = 1000;

    // Start is called before the first frame update
    void Start()
    {
        timer = new Timer();
        timer.Interval = refleshTime;
        timer.Elapsed += (sender, e) => HandleTimer().Forget();

        timer.Start();
    }

    private async UniTaskVoid HandleTimer()
    {
        Debug.Log("호출 테스트2");

        // 필요한 API 요청
    }

    private void OnApplicationQuit()
    {
        timer.Stop();
    }
}
