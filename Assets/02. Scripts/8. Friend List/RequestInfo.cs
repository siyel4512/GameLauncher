using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using TMPro;

public class RequestInfo : MonoBehaviour
{
    public string nickname;
    public string state;

    public bool isRequestComplate;

    public TMP_Text nickname_text;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Test_SetSlotValue(int _index)
    {
        nickname = $"Request_" + _index;
        state = "�¶���";

        nickname_text.text = nickname;
    }

    public void BTN_Accept()
    {
        Debug.Log("��û ���� / ģ�� ����Ʈ�� �߰� ��û");
        isRequestComplate = true;

        GameManager.instance.requestFriendManager.RequestAddList();
    }

    public void BTN_Refuse()
    {
        Debug.Log("��û ����");
        isRequestComplate = true;

        GameManager.instance.requestFriendManager.DeleteList();
    }
}