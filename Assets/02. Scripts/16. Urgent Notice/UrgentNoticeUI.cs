using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UrgentNoticeUI : SwipeUI
{
    private List<UrgentNoticeInfo> spawnedContents;
    public GameObject warningText;

    // Start is called before the first frame update
    void Start()
    {
        warningText.SetActive(false);
    }

    private void Update()
    {
        if (!isTimer)
        {
            ChangeContent();
        }
    }

    public void TryAddContents(int contentCount)
    {
        if (contentCount == 0)
        {
            warningText.SetActive(true);
        }
        else
        {
            warningText.SetActive(false);
        }

        StartCoroutine(AddContents(contentCount));
    }

    #region Create & delete
    IEnumerator AddContents(int contentCount)
    {
        yield return null;

        if (contentCount <= 0)
            yield break;

        spawnedContents = new List<UrgentNoticeInfo>();
        spawnedStepButton = new List<StepButton>();

        List<SaveData.mainBoard> eventNewsInfoValue = GameManager.instance.jsonData.urgentNotice_List;

        // create contents & step buttons
        for (int i = 0; i < contentCount; i++)
        {
            // contents
            GameObject content = Instantiate(conents_prefab);
            content.transform.SetParent(spawnContentsPos, false);
            UrgentNoticeInfo noticeInfo = content.GetComponent<UrgentNoticeInfo>();

            noticeInfo.boardNum = eventNewsInfoValue[i].boardNum;
            noticeInfo.writer = eventNewsInfoValue[i].writer;
            noticeInfo.title = eventNewsInfoValue[i].title;
            noticeInfo.content = eventNewsInfoValue[i].content;
            noticeInfo.webImg = eventNewsInfoValue[i].webImg;
            noticeInfo.lnchrImg = eventNewsInfoValue[i].lnchrImg;
            noticeInfo.boardType = eventNewsInfoValue[i].boardType;
            noticeInfo.openYn = eventNewsInfoValue[i].openYn;
            noticeInfo.exprPeriod = eventNewsInfoValue[i].exprPeriod;
            noticeInfo.regDt = eventNewsInfoValue[i].regDt;
            noticeInfo.upDt = eventNewsInfoValue[i].upDt;
            noticeInfo.linkURL = eventNewsInfoValue[i].boardUrl;

            noticeInfo.noticeUI = this;
            spawnedContents.Add(noticeInfo);

            // step button
            GameObject stepButton = Instantiate(stepButton_prefab, spawnStepButtonsPos.position, Quaternion.identity);
            StepButton _stepButton = stepButton.GetComponent<StepButton>();
            _stepButton.transform.SetParent(spawnStepButtonsPos, false);
            _stepButton.indexNum = i;
            _stepButton.swipeUI = this;
            spawnedStepButton.Add(_stepButton);

            noticeInfo.SetContents();
        }


        // using side buttons
        if (spawnedContents.Count > 1)
        {
            isUsingStepButtons = true;
        }

        //scrollPageValues = new float[transform.childCount]; // 스크롤 되는 페이지의 각 value 값을 저장하는 배열 메모리 할당
        scrollPageValues = new float[spawnedContents.Count]; // 스크롤 되는 페이지의 각 value 값을 저장하는 배열 메모리 할당
        valueDistance = 1f / (scrollPageValues.Length - 1f); // 스크롤 되는 페이지 사이의 거리

        // 스크롤 되는 페이지의 각 value 위치 설정 [0 <= value <= 1]
        for (int i = 0; i < scrollPageValues.Length; ++i)
        {
            scrollPageValues[i] = valueDistance * i;
        }

        // 최대 페이지의 수
        //maxPage = transform.childCount;
        maxPage = spawnedContents.Count;

        // 최초 시작할 때 0번 페이지를 볼 수 있도록 설정
        SetScrollBarValue(maxPage, 0);

        //next content timer
        if (isTimer)
        {
            SetTimer();
        }
        else
        {
            SetStopwatch();
        }
    }

    public void DeleteContents()
    {
        warningText.SetActive(false);

        sw.Stop();

        StopAllCoroutines(); // 애니메이션 코루틴 강제 종료

        isUsingStepButtons = false;
        currentPage = 0;

        // step 버튼 제거 및 리셋
        for (int i = 0; i < spawnedContents.Count; i++)
        {
            Destroy(spawnedContents[i].gameObject);
            Destroy(spawnedStepButton[i].gameObject);
        }
        spawnedContents.Clear();
        spawnedStepButton.Clear();

        // json data reset
        JsonData jsonData = GameManager.instance.jsonData;
        jsonData.temp_urgentNotice_List.Clear();
        jsonData.urgentNotice_List.Clear();

        sideButtons.SetActive(false);

        Debug.Log("[urgent notice] 삭제 완료");
    }

    // Todo : 긴급공지 테스트 관련 삭제 예정
    //public void TEST_Coroutine()
    //{
    //    sw.Stop();
    //}
    #endregion
}
