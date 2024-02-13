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
