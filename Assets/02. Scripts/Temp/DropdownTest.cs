using UnityEngine;
using TMPro;

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

    // Start is called before the first frame update
    void Start()
    {
        currentState = dropdown.value;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // 타이머로 자동 적용
            dropdown.value = 1;
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
