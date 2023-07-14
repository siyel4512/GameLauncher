using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BannerUI : SwipeUI
{
    private List<BannerInfo> spawnedContents;

    //// Start is called before the first frame update
    //void Start()
    //{
        
    //}

    private void Update()
    {
        if (!isTimer)
        {
            ChangeContent();
        }
    }

    public void TryAddContents(int contentCount)
    {
        StartCoroutine(AddContents(contentCount));
    }

    #region Create & delete
    IEnumerator AddContents(int contentCount)
    {
        yield return null;

        if (contentCount <= 0)
            yield break;

        spawnedContents = new List<BannerInfo>();
        spawnedStepButton = new List<StepButton>();

        // create contents & step buttons
        for (int i = 0; i < contentCount; i++)
        {
            // contents
            GameObject content = Instantiate(conents_prefab);
            content.transform.SetParent(spawnContentsPos, false);
            BannerInfo bannerInfo = content.GetComponent<BannerInfo>();

            bannerInfo.SetContents("Banner Image_" + (i + 1), "https://www.naver.com/");
            bannerInfo.bannerUI = this;
            spawnedContents.Add(bannerInfo);

            // step button
            GameObject stepButton = Instantiate(stepButton_prefab, spawnStepButtonsPos.position, Quaternion.identity);
            StepButton _stepButton = stepButton.GetComponent<StepButton>();
            _stepButton.transform.SetParent(spawnStepButtonsPos, false);
            _stepButton.indexNum = i;
            _stepButton.swipeUI = this;
            spawnedStepButton.Add(_stepButton);
        }

        // If you have more than one content
        if (spawnedContents.Count > 0)
        {
            isExistContents = true;
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
        sw.Stop();
        isExistContents = false;
        currentPage = 0;

        for (int i = 0; i < spawnedContents.Count; i++)
        {
            Destroy(spawnedContents[i].gameObject);
            Destroy(spawnedStepButton[i].gameObject);
        }

        spawnedContents.Clear();
        spawnedStepButton.Clear();
    }
    #endregion
}
