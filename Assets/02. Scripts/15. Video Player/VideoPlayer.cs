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
        // Todo : link �������� �׽�Ʈ �Ϸ� �� ���� ����
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
            // ��� ǥ�� ����
            warningText.SetActive(false);

            videoLink = await API.instance.Request_EnglishVideoLink("en"); // ���� �Ұ� ���� ��� ��û

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
            // ����� �� �ִ� ���� ���
            if (videoLink != null && videoLink != "")
            {
                // ��� ǥ�� ����
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

        // ����� �� �ִ� ���� ���
        if (videoLink != null && videoLink != "")
        {
            // ��� ǥ�� ����
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
