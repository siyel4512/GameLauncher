using System.Collections;
using System.Collections.Generic;
using System.Timers;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;

using Timer = System.Timers.Timer;
//using Debug = UnityEngine.Debug;

public class SwipeUI : MonoBehaviour
{
    // set using conpoent
    [Header("[ Side Button Settings ]")]
    public Button previousButton;
    public Button nextButton;
    public GameObject sideButtons;

    [Space(10)]
    [Header("[ Contents Settings ]")]
    public GameObject conents_prefab;
    public Transform spawnContentsPos;

    [Space(10)]
    [Header("[ Step Button Settings ]")]
    public GameObject stepButton_prefab;
    public Transform spawnStepButtonsPos;

    protected List<StepButton> spawnedStepButton;

    //public int TEST_Contents_Count;

    // set state
    [Space(10)]
    [Header("[ State Settings ]")]
    protected bool isUsingStepButtons; // using step buttons
    public bool isUsingSelectStepImage;

    // set swipe animation
    [Space(10)]
    [Header("[ Swipe Animation Settings ]")]
    public Scrollbar scrollBar;                    // Scrollbar의 위치를 바탕으로 현재 페이지 검사
    //public Transform[] stepButtons;             // 현재 페이지를 나타내는 원 Image UI들의 Transform
    public float swipeTime = 0.2f;         // 페이지가 Swipe 되는 시간
    public float swipeDistance = 50.0f;        // 페이지가 Swipe되기 위해 움직여야 하는 최소 거리

    protected float[] scrollPageValues;           // 각 페이지의 위치 값 [0.0 - 1.0]
    protected float valueDistance = 0;            // 각 페이지 사이의 거리
    protected int currentPage = 0;            // 현재 페이지
    protected int maxPage = 0;                // 최대 페이지
    protected bool isSwipeMode = false;       // 현재 Swipe가 되고 있는지 체크
    protected float stepButtonScale = 1.6f;    // 현재 페이지의 원 크기(배율)

    public bool isTimer;

    #region contents
    // set contents page
    public void SetScrollBarValue(int contentCount, int index)
    {
        if (contentCount > 0)
        {
            currentPage = index;
            scrollBar.value = scrollPageValues[index];

            if (index == 0)
            {
                previousButton.interactable = false;
                nextButton.interactable = true;
            }

            UpdateStepButtons();
        }
    }

    // swipe effect play
    private IEnumerator OnSwipeOneStep(int indexNum, bool isUsingStepButton = false)
    {
        // swipe content
        float start = scrollBar.value;
        float current = 0;
        float percent = 0;

        isSwipeMode = true;
        
        if (scrollPageValues[indexNum] == 0 && isUsingStepButton)
        {
            scrollBar.value = 0;
        }
        else
        {
            while (percent < 1)
            {
                current += Time.deltaTime;
                percent = current / swipeTime;

                scrollBar.value = Mathf.Lerp(start, scrollPageValues[indexNum], percent);

                yield return null;
            }

        }

        isSwipeMode = false;

        // set buttons state
        if (indexNum >= maxPage - 1)
        {
            previousButton.interactable = true;
            nextButton.interactable = false;
        }
        else if (indexNum <= 0)
        {
            previousButton.interactable = false;
            nextButton.interactable = true;
        }
        else
        {
            previousButton.interactable = true;
            nextButton.interactable = true;
        }

        UpdateStepButtons();
    }
    #endregion

    #region step button
    // set step button state
    private void UpdateStepButtons()
    {
        if (scrollPageValues.Length <= 1)
        {
            for (int i = 0; i < scrollPageValues.Length; i++)
            {
                spawnedStepButton[i].gameObject.SetActive(false);
            }
        }

        // 아래에 배치된 페이지 버튼 크기, 색상 제어 (현재 머물고 있는 페이지의 버튼만 수정)
        for (int i = 0; i < scrollPageValues.Length; ++i)
        {
            if (isUsingSelectStepImage)
            {
                spawnedStepButton[i].selectImage.SetActive(false);
            }
            else
            {
                spawnedStepButton[i].transform.localScale = Vector2.one;
                spawnedStepButton[i].transform.GetComponent<Image>().color = Color.white;
            }

            // 페이지의 절반을 넘어가면 현재 페이지 원을 바꾸도록
            if (scrollBar.value < scrollPageValues[i] + (valueDistance / 2) && scrollBar.value > scrollPageValues[i] - (valueDistance / 2))
            {
                if (isUsingSelectStepImage)
                {
                    spawnedStepButton[i].selectImage.SetActive(true);
                }
                else
                {
                    spawnedStepButton[i].transform.localScale = Vector2.one * stepButtonScale;
                    spawnedStepButton[i].transform.GetComponent<Image>().color = Color.black;
                }
            }
        }
    }

    public void ShowNHideSideButton(bool isShow)
    {
        if (isUsingStepButtons)
        {
            sideButtons.SetActive(isShow);
        }
        else
        {
            sideButtons.SetActive(false);
        }
    }
    #endregion

    #region buttons
    public void BTN_OnSwipeOneStep(bool isNext)
    {
        if (isTimer)
        {
            timer.Stop();
            timer.Start();
        }
        else
        {
            sw.Restart();
        }

        previousButton.interactable = true;
        nextButton.interactable = true;

        if (isNext)
        {
            currentPage++;

            if (currentPage >= maxPage - 1)
            {
                currentPage = maxPage - 1;
                previousButton.interactable = true;
                nextButton.interactable = false;
            }
        }
        else
        {
            currentPage--;

            if (currentPage <= 0)
            {
                currentPage = 0;
                previousButton.interactable = false;
                nextButton.interactable = true;
            }
        }

        StartCoroutine(OnSwipeOneStep(currentPage));
    }

    public void BTN_UpdateStepButtons(int indexNum)
    {
        currentPage = indexNum;
        StartCoroutine(OnSwipeOneStep(indexNum));

        if (isTimer)
        {
            timer.Stop();
            timer.Start();
        }
        else
        {
            sw.Restart();
        }
    }
    #endregion

    #region using Timer
    public Timer timer;
    public int refleshTime = 5000;

    public void SetTimer()
    {
        timer = new Timer();
        timer.Interval = refleshTime;
        timer.Elapsed += new ElapsedEventHandler(ChangeContent);
        timer.Start();
    }

    private void ChangeContent(object sender, ElapsedEventArgs e)
    {
        //Debug.Log("next content");

        currentPage++;

        if (currentPage == maxPage - 1)
        {
            previousButton.interactable = true;
            nextButton.interactable = false;
            //Debug.Log("last");
        }
        else if (currentPage > maxPage - 1)
        {
            currentPage = 0;
            previousButton.interactable = false;
            nextButton.interactable = true;
            //Debug.Log("reset");
        }
        else
        {
            previousButton.interactable = true;
            nextButton.interactable = true;
            //Debug.Log("yet");
        }

        StartCoroutine(OnSwipeOneStep(currentPage));
    }

    private void OnApplicationQuit()
    {
        if (isTimer)
        {
            timer.Stop();
            timer.Dispose();
        }
    }

    public void ResetTimer()
    {
        timer.Stop();
        timer.Dispose();
    }
    #endregion

    #region using stopwatch
    public Stopwatch sw;
    public int currentTimeCount;
    public int limitTime;
    public bool isReadyForStopwatch;
    public bool isComplateLoaded; // 배너 로드 완료

    public void SetStopwatch()
    {
        //sw = new Stopwatch();
        //isReadyForStopwatch = true;
        //sw.Start();

        sw = new Stopwatch();

        // korean
        if (GameManager.instance.languageManager.currentLanguageNum == 1)
        {
            isReadyForStopwatch = true;
            sw.Start();
        }
        // english
        else
        {
            sw.Stop();
            isReadyForStopwatch = false;
            SetScrollBarValue(maxPage, 0);
        }

        isComplateLoaded = true;
    }

    public void ChangeContent()
    {

        if (!isReadyForStopwatch)
            return;

        currentTimeCount = limitTime - (int)(sw.ElapsedMilliseconds / 1000f);

        // time out
        if (currentTimeCount < 0)
        {
            currentTimeCount = limitTime;
            sw.Restart();

            currentPage++;

            // last step button
            if (currentPage == maxPage - 1)
            {
                previousButton.interactable = true;
                nextButton.interactable = false;
            }
            // reset step button position
            else if (currentPage > maxPage - 1)
            {
                currentPage = 0;
                previousButton.interactable = false;
                nextButton.interactable = true;
            }
            // move to step button
            else
            {
                previousButton.interactable = true;
                nextButton.interactable = true;
            }

            StartCoroutine(OnSwipeOneStep(currentPage, true));
        }
    }

    // logout
    public void ResetStopwatch()
    {
        sw.Stop();
        sw.Reset();
        isReadyForStopwatch = false;
        isComplateLoaded = false;
    }

    public void ReStartBanner()
    {
        sw.Start();
        isReadyForStopwatch = true;
        SetScrollBarValue(maxPage, 0);
    }

    public void StopBanner()
    {
        sw.Stop();
        sw.Reset();
        isReadyForStopwatch = false;
        SetScrollBarValue(maxPage, 0);
    }
    #endregion
}