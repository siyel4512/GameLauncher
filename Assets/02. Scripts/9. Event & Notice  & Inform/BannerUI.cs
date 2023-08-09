using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
//using static SaveData;

public class BannerUI : SwipeUI
{
    private List<BannerInfo> spawnedContents;
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

    public async void TryAddContents(int contentCount)
    {
        if (contentCount == 0)
        {
            warningText.SetActive(true);
        }
        else
        {
            warningText.SetActive(false);
        }

        await PreloadImages(contentCount);

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

        List<SaveData.mainBoard> eventBannerInfoValue = GameManager.instance.jsonData.event_List;

        // create contents & step buttons
        for (int i = 0; i < contentCount; i++)
        {
            // contents
            GameObject content = Instantiate(conents_prefab);
            content.transform.SetParent(spawnContentsPos, false);

            // set banner image
            content.GetComponent<RawImage>().texture = imageCache[i];
            
            

            BannerInfo bannerInfo = content.GetComponent<BannerInfo>();

            //bannerInfo.SetContents("Banner Image_" + (i + 1), "https://www.naver.com/");

            bannerInfo.boardNum = eventBannerInfoValue[i].boardNum;
            bannerInfo.writer = eventBannerInfoValue[i].writer;
            bannerInfo.title = eventBannerInfoValue[i].title;
            bannerInfo.content = eventBannerInfoValue[i].content;
            bannerInfo.webImg = eventBannerInfoValue[i].webImg;
            bannerInfo.lnchrImg = eventBannerInfoValue[i].lnchrImg;
            bannerInfo.boardType = eventBannerInfoValue[i].boardType;
            bannerInfo.openYn = eventBannerInfoValue[i].openYn;
            bannerInfo.exprPeriod = eventBannerInfoValue[i].exprPeriod;
            bannerInfo.regDt = eventBannerInfoValue[i].regDt;
            bannerInfo.upDt = eventBannerInfoValue[i].upDt;
            bannerInfo.linkURL = "https://www.naver.com/";

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

        // 다운로드 에러 체크
        CheckDownloadError();
    }

    public void DeleteContents()
    {
        warningText.SetActive(false);

        sw.Stop();
        isUsingStepButtons = false;
        currentPage = 0;

        for (int i = 0; i < spawnedContents.Count; i++)
        {
            Destroy(spawnedContents[i].gameObject);
            Destroy(spawnedStepButton[i].gameObject);
        }

        imageCache = new List<Texture2D>();
        downloadErrorImageIndexNum = new List<bool>();
        spawnedContents.Clear();
        spawnedStepButton.Clear();
    }
    #endregion

    #region Banner Image Load
    //public string apiUrl = "https://launcherdownload1.s3.ap-northeast-2.amazonaws.com/snowFieldLoadingBackGround_2.jpg"; // 서버 API URL
    public int compressedWidth = 512; // 압축된 이미지 너비
    //public int numberOfImages = 10; // 생성할 이미지 개수

    private List<Texture2D> imageCache = new List<Texture2D>();
    private List<bool> downloadErrorImageIndexNum = new List<bool>();

    //private async UniTask PreloadImages(string url, int count)
    private async UniTask PreloadImages(int count)
    {
        List<SaveData.mainBoard> eventBannerInfoValue = GameManager.instance.jsonData.event_List;

        for (int i = 0; i < count; i++)
        {
            //// 정상적인 URL 인지 확인하기
            //using (var www = UnityWebRequestTexture.GetTexture(eventBannerInfoValue[i].lnchrImg))
            //{
            //    await www.SendWebRequest();

            //    if (www.result == UnityWebRequest.Result.Success)
            //    {
            //        Texture2D texture = DownloadHandlerTexture.GetContent(www);
            //        if (texture != null)
            //        {
            //            imageCache.Add(texture);
            //        }
            //    }
            //    else
            //    {
            //        Debug.Log("Image download failed: " + www.error);
            //    }
            //}

            try
            {
                Uri uri;
                if (Uri.TryCreate(eventBannerInfoValue[i].lnchrImg, UriKind.Absolute, out uri))
                {
                    using (var www = UnityWebRequestTexture.GetTexture(uri))
                    {
                        await www.SendWebRequest();

                        if (www.result == UnityWebRequest.Result.Success)
                        {
                            Texture2D texture = DownloadHandlerTexture.GetContent(www);
                            if (texture != null)
                            {
                                imageCache.Add(texture);
                                downloadErrorImageIndexNum.Add(true);
                            }
                        }
                        else
                        {
                            Debug.Log("[에러]Image download failed: " + www.error);
                        }
                    }
                }
                else
                {
                    Debug.Log("[에러]Invalid URL: " + eventBannerInfoValue[i].lnchrImg);
                    //imageCache.Add(new Texture2D(512, 512));
                    imageCache.Add(new Texture2D(2, 2));
                    downloadErrorImageIndexNum.Add(false);
                }
            }
            catch (UnityWebRequestException ex)
            {
                if (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    Debug.LogWarning("[에러]Image not found: " + eventBannerInfoValue[i].lnchrImg);
                    // 이미지를 찾을 수 없을 때의 처리
                    imageCache.Add(new Texture2D(2, 2));
                    downloadErrorImageIndexNum.Add(false);
                }
                else
                {
                    Debug.LogError("[에러]UnityWebRequestException: " + ex.Message);
                    // 다른 UnityWebRequestException 처리
                    imageCache.Add(new Texture2D(2, 2));
                    downloadErrorImageIndexNum.Add(false);
                }
            }
            catch (Exception e)
            {
                Debug.LogError("[에러] : " + e);
                // 기타 예외 처리
                imageCache.Add(new Texture2D(2, 2));
                downloadErrorImageIndexNum.Add(false);
            }
        }
    }

    private Texture2D CompressTexture(Texture2D originalTexture, int targetWidth)
    {
        int targetHeight = (int)(originalTexture.height * ((float)targetWidth / originalTexture.width));
        Texture2D compressedTexture = new Texture2D(targetWidth, targetHeight, originalTexture.format, false);

        // 픽셀 해상도 조정을 통해 이미지 압축
        Color[] pixels = originalTexture.GetPixels(0, 0, originalTexture.width, originalTexture.height);
        Color[] resizedPixels = compressedTexture.GetPixels(0, 0, targetWidth, targetHeight);

        for (int y = 0; y < targetHeight; y++)
        {
            for (int x = 0; x < targetWidth; x++)
            {
                int sourceX = x * originalTexture.width / targetWidth;
                int sourceY = y * originalTexture.height / targetHeight;
                resizedPixels[y * targetWidth + x] = pixels[sourceY * originalTexture.width + sourceX];
            }
        }

        compressedTexture.SetPixels(resizedPixels);
        compressedTexture.Apply();

        return compressedTexture;
    }

    private void CheckDownloadError()
    {
        for (int i = 0; i < spawnedContents.Count; i++)
        {
            if (!downloadErrorImageIndexNum[i])
            {
                spawnedContents[i].GetComponent<RawImage>().enabled = false;
                spawnedContents[i].downloadErrorText.SetActive(true);
            }
        }
    }
    #endregion
}
