using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Localization.Components;

public class DownloadStateTextAnimation : MonoBehaviour
{
    public TMP_Text stateText;
    public int count = 0;
    public string message;

    public LocalizeStringEvent LocalizeStringEvent;

    // Start is called before the first frame update
    void Start()
    {
        stateText.text = message;
        LocalizeStringEvent.RefreshString(); // LocalizeStringEvent refresh
    }

    public void OnEnable()
    {
        if (LocalizeStringEvent == null)
        {
            LocalizeStringEvent = GetComponent<LocalizeStringEvent>();
        }

        InvokeRepeating("StateAnimation", 0, 1);
    }

    public void OnDisable()
    {
        CancelInvoke("StateAnimation");
        count = 0;
        stateText.text = message;
    }

    public void StateAnimation()
    {
        if (count != 0)
        {
            stateText.text += ".";
        }

        count++;

        if (count > 4)
        {
            count = 1;
            stateText.text = message;
            LocalizeStringEvent.RefreshString();
        }
    }
}
