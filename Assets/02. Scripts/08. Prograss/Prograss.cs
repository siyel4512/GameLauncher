using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Prograss : MonoBehaviour
{
    public Image bar;
    public TMP_Text persent_txt;

    public void SetBarState(int _fillAmount)
    {
        bar.fillAmount = _fillAmount * 0.01f;
    }

    public void SetPersent(int _persent)
    {
        persent_txt.text = _persent.ToString();
    }

    public void ResetState()
    {
        bar.fillAmount = 0;
        persent_txt.text = "0";
    }
}
