using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using TMPro;

public class LanguageButtonAnimation : MonoBehaviour
{
    public Transform ToggleButtonBackground; // 버튼 애니메이션 이동 기준점
    public Transform iconPos;
    
    public Image[] icons; // 언어 아이콘
    public Image[] LoginTitles; // 로그인 타이틀 이미지
    public TMP_Text[] texts;

    public Sequence buttonAnimation;
    public float animationTime; // 애니메이션 실행 속도

    public void SetLanguageButton(int languageNum)
    {
        switch (languageNum)
        {
            case 0:
                // english
                buttonAnimation = DOTween.Sequence()
                    .Append(iconPos.DOMoveX(ToggleButtonBackground.position.x + 42, animationTime))
                    .OnComplete(() =>
                    {
                        for (int i = 0; i < icons.Length; i++)
                        {
                            icons[i].gameObject.SetActive(false);
                            texts[i].gameObject.SetActive(false);
                            LoginTitles[i].gameObject.SetActive(false);
                        }

                        icons[1].gameObject.SetActive(true);
                        texts[1].gameObject.SetActive(true);
                        LoginTitles[1].gameObject.SetActive(true);
                    });
                break;
            case 1:
                // korean
                buttonAnimation = DOTween.Sequence()
                    .Append(iconPos.DOMoveX(ToggleButtonBackground.position.x - 42, animationTime))
                    .OnComplete(() =>
                    {
                        for (int i = 0; i < icons.Length; i++)
                        {
                            icons[i].gameObject.SetActive(false);
                            texts[i].gameObject.SetActive(false);
                            LoginTitles[i].gameObject.SetActive(false);
                        }

                        icons[0].gameObject.SetActive(true);
                        texts[0].gameObject.SetActive(true);
                        LoginTitles[0].gameObject.SetActive(true);
                    });
                break;
        }
    }
}
