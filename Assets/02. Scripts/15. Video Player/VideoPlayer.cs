using RenderHeads.Media.AVProVideo;
using RenderHeads.Media.AVProVideo.Demos;
using System.Collections;
using Unity.VisualScripting;
using UnityEditor.UIElements;
using UnityEngine;

public class VideoPlayer : MonoBehaviour
{
    public MediaPlayer Player;
    private string videoLink;

    public MediaPlayerUI mediaPlayerUI;
    public GameObject warningText;

    public bool isFirstCheck;
    public bool isLoadedLink;

    public async void Awake()
    {
        // Todo : link 가져오기 테스트 완료 후 삭제 예정
        if (!DEV.instance.isLinkLoadTEST)
        {
            // link setting
            // test server
            if (DEV.instance.isUsingTestServer)
            {
                //videoLink = "https://ejrdejzsaflk20717940.cdn.ntruss.com/onlinemuseum/video/ENG_METAPLY_VIDEO.mp4";
                //videoLink = "https://iyvmhtukulmj22048635.cdn.ntruss.com/CDNResources/video/ENG_METAPLY_VIDEO.mp4";

                videoLink = "https://iyvmhtukulmj22048635.cdn.ntruss.com/CDNResources/video/ENG_METAPLY_VIDEO.mp4";
            }
            // live server
            else
            {
                //videoLink = "https://yhbdymjatqnq20869625.cdn.ntruss.com/onlinemuseum/video/ENG_METAPLY_VIDEO.mp4";
                //videoLink = "https://metaply.go.kr/CDNResources/video/ENG_METAPLY_VIDEO.mp4";

                videoLink = "https://yhbdymjatqnq20869625.cdn.ntruss.com/onlinemuseum/video/ENG_METAPLY_VIDEO.mp4";
            }
        }
        else
        {
            // 경고 표시 숨김
            warningText.SetActive(false);

            videoLink = await API.instance.Request_EnglishVideoLink("en"); // 영어 소개 영상 경로 요청

            if (videoLink != null && videoLink != "")
            {
                // start video (first setting play)
                Player.enabled = true;
                Player.OpenMedia(new MediaPath(videoLink, MediaPathType.AbsolutePathOrURL), autoPlay: true);
            }

            isLoadedLink = true;
        }
    }

    public void OnEnable()
    {
        if (!isFirstCheck)
        {
            StartCoroutine(Co_CheckLink());
        }
        else
        {
            // 재생할 수 있는 비디오 경로
            if (videoLink != null && videoLink != "")
            {
                // 경고 표시 숨김
                warningText.SetActive(false);

                // start video
                Player.enabled = true;
                Player.OpenMedia(new MediaPath(videoLink, MediaPathType.AbsolutePathOrURL), autoPlay: true);
                //mediaPlayerUI.TogglePlayPause();
                Debug.Log("[videlo link Check] video play 2");
            }
            else
            {
                warningText.SetActive(true);
                Debug.Log("[videlo link Check] no video 2");
            }
        }
    }

    IEnumerator Co_CheckLink()
    {
        while (!isLoadedLink)
        {
            yield return null;
        }

        isFirstCheck = true;

        // 재생할 수 있는 비디오 경로
        if (videoLink != null && videoLink != "")
        {
            // 경고 표시 숨김
            warningText.SetActive(false);

            // start video
            Player.enabled = true;
            Player.OpenMedia(new MediaPath(videoLink, MediaPathType.AbsolutePathOrURL), autoPlay: true);
            //mediaPlayerUI.TogglePlayPause();
            Debug.Log("[videlo link Check] video play 1");
        }
        else
        {
            warningText.SetActive(true);
            Debug.Log("[videlo link Check] no video 1");
        }
    }

    public void OnDisable()
    {
        // stop video
        Player.CloseMedia();
        warningText.SetActive(false);
    }
}
