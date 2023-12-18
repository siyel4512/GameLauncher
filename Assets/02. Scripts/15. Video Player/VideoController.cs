using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class VideoController : MonoBehaviour
{
    public Transform videoPosition;
    public GameObject scrollbar;
    public RectTransform contents;
    public RectTransform viewport;

    [Header("Maskable Targets")]
    public Image[] PlayAndVolumeImage;
    public RawImage AudioSpectrumImage;

    [Space(10)]
    public Image[] CenterPlayButtonImages;
    public GameObject CenterPlayButton;

    [Space(10)]
    public GameObject TimeLineImage;

    [Header("Maskable set limit line")]
    public float controlButtonsLimeLine;
    public float timeLineLimeLine;

    [Space(10)]
    [Header("Range correction value")]
    public float min_correctionValue;
    public float max_correctionValue;

    // Update is called once per frame
    void Update()
    {
        // 영어 버전일때만 실행
        if (GameManager.instance.languageManager.currentLanguageNum == 0)
        {
            SetControllerButtons();
            SetCenterPlayButton();
        }
    }

    public void SetControllerButtons()
    {
        if (!DEV.instance.isUsingVideoAnimControl) return;

        // 스크롤바가 생겼을때
        if (viewport.rect.height < contents.rect.height)
        {
            float scrollPos = ((viewport.rect.height + contents.anchoredPosition.y) / contents.rect.height) * 100;

            // 재생 버튼 & 볼륨 스피커 maskable 숨기기
            if (scrollPos <= controlButtonsLimeLine)
            {
                for (int i = 0; i < PlayAndVolumeImage.Length; i++)
                {
                    PlayAndVolumeImage[i].maskable = true;
                    //PlayAndVolumeImage[i].gameObject.SetActive(false);
                }

                AudioSpectrumImage.maskable = true;
                //AudioSpectrumImage.gameObject.SetActive(false);
            }
            //  재생 버튼 & 볼륨 스피커 애니메이션 작동
            else
            {
                for (int i = 0; i < PlayAndVolumeImage.Length; i++)
                {
                    PlayAndVolumeImage[i].maskable = false;
                    //PlayAndVolumeImage[i].gameObject.SetActive(true);
                }

                AudioSpectrumImage.maskable = false;
                //AudioSpectrumImage.gameObject.SetActive(true);
                TimeLineImage.SetActive(true);
            }

            // 타임라인바 숨기기
            if (scrollPos <= timeLineLimeLine)
            {
                TimeLineImage.SetActive(false);
            }
            // 타임라인바 표시
            else
            {
                TimeLineImage.SetActive(true);
            }
        }
        else
        {
            for (int i = 0; i < PlayAndVolumeImage.Length; i++)
            {
                PlayAndVolumeImage[i].maskable = false;
                //PlayAndVolumeImage[i].gameObject.SetActive(true);
            }

            AudioSpectrumImage.maskable = false;
            //AudioSpectrumImage.gameObject.SetActive(true);
            TimeLineImage.SetActive(true);
        }
    }

    public void SetCenterPlayButton()
    {
        if (!DEV.instance.isUsingVideoAnimControl) return;

        // 스크롤바가 생겼을때
        if (viewport.rect.height < contents.rect.height)
        {
            // 스크롤 범위 계산
            float minPos = contents.anchoredPosition.y;
            float maxPos = contents.anchoredPosition.y + viewport.rect.height;

            if (minPos + min_correctionValue <= Mathf.Abs(videoPosition.localPosition.y) && maxPos - max_correctionValue >= Mathf.Abs(videoPosition.localPosition.y))
            {
                for (int i = 0; i < CenterPlayButtonImages.Length; i++)
                {
                    CenterPlayButtonImages[i].maskable = false;
                    CenterPlayButtonImages[i].gameObject.SetActive(true);
                }
                //CenterPlayButton.SetActive(true);
            }
            else
            {
                for (int i = 0; i < CenterPlayButtonImages.Length; i++)
                {
                    CenterPlayButtonImages[i].maskable = true;
                    CenterPlayButtonImages[i].gameObject.SetActive(false);
                }
                //CenterPlayButton.SetActive(false);
            }
        }
    }
}
