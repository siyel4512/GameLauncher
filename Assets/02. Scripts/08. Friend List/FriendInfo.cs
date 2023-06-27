using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FriendInfo : MonoBehaviour
{
    public TMP_Text nickname;
    public TMP_Text state;
    public Image stateIcon;
    public bool isSelected;
    public GameObject selectedImage;

    // Start is called before the first frame update
    void Start()
    {
    
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Todo : ģ�� �г���, ���� ���� ����
    public void Test_SetSlotValue(int _index)
    {
        nickname.text = $"Test_" + _index;
        state.text = "�¶���";
        stateIcon.color = Color.green;
    }

    public void SelectSlot()
    {
        GameManager.instance.friendListManager.ResetSelect();
        GameManager.instance.friendListManager.isSelectedSlot = true;
        isSelected = true;
        selectedImage.SetActive(true);
    }
}
