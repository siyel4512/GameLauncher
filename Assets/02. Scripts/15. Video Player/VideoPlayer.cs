using RenderHeads.Media.AVProVideo;
using RenderHeads.Media.AVProVideo.Demos;
using UnityEngine;

public class VideoPlayer : MonoBehaviour
{
    public MediaPlayer Player;
    private string videoLink;

    public MediaPlayerUI mediaPlayerUI;

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
            videoLink = await API.instance.Request_EnglishVideoLink("en"); // 영어 소개 영상 경로 요청

            // start video (first setting play)
            Player.enabled = true;
            Player.OpenMedia(new MediaPath(videoLink, MediaPathType.AbsolutePathOrURL), autoPlay: true);
            //mediaPlayerUI.TogglePlayPause();
        }
    }

    public void OnEnable()
    {
        if (videoLink != null)
        {
            // start video
            Player.enabled = true;
            Player.OpenMedia(new MediaPath(videoLink, MediaPathType.AbsolutePathOrURL), autoPlay: true);
            //mediaPlayerUI.TogglePlayPause();
        }
    }

    public void OnDisable()
    {
        // stop video
        Player.CloseMedia();
    }
}
