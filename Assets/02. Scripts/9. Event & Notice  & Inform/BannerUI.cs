using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BannerUI : SwipeUI
{
    // Start is called before the first frame update
    void Start()
    {
        AddContents();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    #region Create & delete
    IEnumerator AddContents()
    {
        yield return null;

        spawnedContents = new List<NoticeInfo>();
        spawnedStepButton = new List<StepButton>();

        // create contents & step buttons
        for (int i = 0; i < TEST_Contents_Count; i++)
        {
            // contents
            GameObject content = Instantiate(conents_prefab);
            content.transform.SetParent(spawnContentsPos, false);
            NoticeInfo noticeInfo = content.GetComponent<NoticeInfo>();
            noticeInfo.SetContents("Title_" + (i + 1), "Content Content Content Content Content", "https://www.youtube.com/");
            //noticeInfo.noticeUI = this;
            spawnedContents.Add(noticeInfo);

            // step button
            GameObject stepButton = Instantiate(stepButton_prefab, spawnStepButtonsPos.position, Quaternion.identity);
            StepButton _stepButton = stepButton.GetComponent<StepButton>();
            _stepButton.transform.SetParent(spawnStepButtonsPos, false);
            _stepButton.indexNum = i;
            _stepButton.swipeUI = this;
            spawnedStepButton.Add(_stepButton);
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
        SetScrollBarValue(0);
    }
    #endregion
}
