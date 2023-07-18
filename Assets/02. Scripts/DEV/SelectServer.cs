using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SelectServer : MonoBehaviour
{
    public TMP_Dropdown selectServer;
    public int selectServerNum;

    // Start is called before the first frame update
    void Start()
    {
        selectServer.onValueChanged.AddListener(delegate {
            OnChangedValue(selectServer);
        });

        selectServerNum = 0;
    }

    public void OnChangedValue(TMP_Dropdown change)
    {
        selectServerNum = change.value;

        Debug.Log("New Value: " + change.value);
        Debug.Log("New Value: " + change.options[change.value].text);
        Debug.Log("New Value: " + change.captionText.text);
    }
}
