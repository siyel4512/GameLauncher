using RenderHeads.Media.AVProVideo;
using RenderHeads.Media.AVProVideo.Demos;
using System.Collections;
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
