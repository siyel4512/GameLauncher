using System.Collections;
using System.Collections.Generic;
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
        SetControllerButtons();
        SetCenterPlayButton();
    }

    public void SetControllerButtons()
    {
        if (!DEV.instance.isUsingVideoAnimControl) return;

        // ��ũ�ѹٰ� ��������
        if (viewport.rect.height < contents.rect.height)
        {
            float scrollPos = ((viewport.rect.height + contents.anchoredPosition.y) / contents.rect.height) * 100;

            //Debug.Log("��ũ�� ��ġ ��� : " + ((viewport.rect.height + contents.anchoredPosition.y) / contents.rect.height) * 100);

            // ��� ��ư & ���� ����Ŀ maskable �����
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
            //  ��� ��ư & ���� ����Ŀ �ִϸ��̼� �۵�
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

            // Ÿ�Ӷ��ι� �����
            if (scrollPos <= timeLineLimeLine)
            {
                TimeLineImage.SetActive(false);
            }
            // Ÿ�Ӷ��ι� ǥ��
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

        // ��ũ�ѹٰ� ��������
        if (viewport.rect.height < contents.rect.height)
        {
            //Debug.Log($"contents ���� : {rectTransform1.rect.height} / �̹��� ���� : {rectTransform2.rect.height} / ��ũ�� ��ġ : {rectTransform3.anchoredPosition.y} / �̹��� ���� + ��ũ�� ��ġ : {rectTransform2.rect.height + rectTransform3.anchoredPosition.y}");

            //float scrollPos1 = ((rectTransform2.rect.height + rectTransform3.anchoredPosition.y) / rectTransform1.rect.height) * 100;
            float scrollPos1 = ((viewport.rect.height + contents.anchoredPosition.y) / contents.rect.height) * 100;
            //Debug.Log($"��ũ�� ��ġ �ۼ�Ʈ : {scrollPos1}");

            float scrollPos2 = viewport.rect.height + contents.anchoredPosition.y;

            //Debug.Log($"���� ��ġ1 : {Mathf.Abs(videoTransform.localPosition.y) + 25} / �̹��� ���� - ��ũ�� ��ġ : {scrollPos2} / �Ÿ� ��� {Mathf.Abs((Mathf.Abs(videoTransform.localPosition.y) + 25) - scrollPos2)}");
            //Debug.Log($"���� ��ġ2 : {Mathf.Abs(videoTransform.localPosition.y)} / ��ũ�� ��ġ / 2 : {rectTransform3.anchoredPosition.y / 2} / (��ũ�� ��ġ + (�̹��� ���� / 2)) : {rectTransform3.anchoredPosition.y + (rectTransform2.rect.height /*/ 2*/)} / (��ũ�� ��ġ - (�̹��� ���� / 2)) : {rectTransform3.anchoredPosition.y - (rectTransform2.rect.height /*/ 2*/)}");

            float minPos = contents.anchoredPosition.y;
            float maxPos = contents.anchoredPosition.y + viewport.rect.height;

            if (minPos + min_correctionValue <= Mathf.Abs(videoPosition.localPosition.y) && maxPos - max_correctionValue >= Mathf.Abs(videoPosition.localPosition.y))
            {
                Debug.Log("[TEST] ȭ�鿡 ����...");

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
