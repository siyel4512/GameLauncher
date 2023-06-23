using UnityEngine;
using TMPro;
using System.Diagnostics;

using Debug = UnityEngine.Debug;

//public enum PlayerState
//{
//    Online,
//    Take_a_Break,
//    Other_Work,
//    Offline
//} 

public class DropdownTest : MonoBehaviour
{
    public TMP_Dropdown dropdown;
    public int currentState;

    private Stopwatch sw;
    public int limitTime;
    public int currentTimeCount;

    // Start is called before the first frame update
    void Start()
    {
        currentState = dropdown.value;

        sw = new Stopwatch();
        sw.Start();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1) || Input.GetMouseButtonDown(2))
        {
            Debug.Log("타이머 리셋");
            currentTimeCount = limitTime;
            sw.Restart();
        }

        currentTimeCount = limitTime - (int)(sw.ElapsedMilliseconds / 1000f);

        if (currentTimeCount < 0)
        {
            Debug.Log("타이머 종료");
            currentTimeCount = limitTime;
            dropdown.value = 1;
            sw.Restart();
        }
    }

    //private void OnDropDown()
    //{
    //    //string op1 = dropdown.options[dropdown.value].text;
    //    ////result.text = op1;
    //    //result_img.sprite = dropdown.options[dropdown.value].image;
    //    //Debug.LogError("Dropdown Result!\n" + op1);

    //    //string op2 = select.options[select.value].text;
    //    //message.text = op2;
    //    //Debug.Log("Dropdown Change!\n" + op2);
    //}

    public void OnChengedValue()
    {
        currentState = dropdown.value;
        Debug.Log("update player state");
    }
}
