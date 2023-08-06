using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

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

        await PreloadImages(apiUrl, contentCount);

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

            // set banner image
            content.GetComponent<RawImage>().texture = imageCache[i];

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
        isUsingStepButtons = false;
        currentPage = 0;

        for (int i = 0; i < spawnedContents.Count; i++)
        {
            Destroy(spawnedContents[i].gameObject);
            Destroy(spawnedStepButton[i].gameObject);
        }

        imageCache = new List<Texture2D>();
        spawnedContents.Clear();
        spawnedStepButton.Clear();
    }
    #endregion

    #region Banner Image Load
    public string apiUrl = "https://launcherdownload1.s3.ap-northeast-2.amazonaws.com/snowFieldLoadingBackGround_2.jpg"; // 서버 API URL
    public int compressedWidth = 512; // 압축된 이미지 너비
    //public int numberOfImages = 10; // 생성할 이미지 개수

    private List<Texture2D> imageCache = new List<Texture2D>();

    private async UniTask PreloadImages(string url, int count)
    {
        for (int i = 0; i < count; i++)
        {
            using (var www = UnityWebRequestTexture.GetTexture(url))
            {
                await www.SendWebRequest();

                if (www.result == UnityWebRequest.Result.Success)
                {
                    Texture2D texture = DownloadHandlerTexture.GetContent(www);
                    if (texture != null)
                    {
                        imageCache.Add(texture);
                    }
                }
                else
                {
                    Debug.Log("Image download failed: " + www.error);
                }
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
    #endregion
}
