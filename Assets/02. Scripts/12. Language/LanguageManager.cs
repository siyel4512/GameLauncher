using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Settings;

public class LanguageManager : MonoBehaviour
{
    public int currentLanguageNum = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    bool isChange = false;

    public void ChangeLanguage(int index)
    {
        if (isChange)
            return;

        StartCoroutine(Co_ChangeLanguage(index));
    }

    IEnumerator Co_ChangeLanguage(int index)
    {
        isChange = true;
        currentLanguageNum = index;

        yield return LocalizationSettings.InitializationOperation;
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[index];

        isChange = false;
    }
}
