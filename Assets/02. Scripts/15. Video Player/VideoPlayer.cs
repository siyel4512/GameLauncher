using RenderHeads.Media.AVProVideo;
using UnityEngine;

public class VideoPlayer : MonoBehaviour
{
    public MediaPlayer Player;
    private string videoLink;

    public void Awake()
    {
        // link 설정
        // test server
        if (DEV.instance.isUsingTestServer)
        {
            //Debug.Log("[video] 테스트 서버 사용중...");
            videoLink = "https://ejrdejzsaflk20717940.cdn.ntruss.com/onlinemuseum/video/ENG_METAPLY_VIDEO.mp4";

        }
        // live server
        else
        {
            //Debug.Log("[video] 라이브 서버 사용중...");
            videoLink = "https://yhbdymjatqnq20869625.cdn.ntruss.com/onlinemuseum/video/ENG_METAPLY_VIDEO.mp4";
        }
    }

    public void OnEnable()
    {
        // start video
        //Debug.Log("[video] 플레이 시작");
        Player.enabled = true;
        Player.OpenMedia(new MediaPath(videoLink, MediaPathType.AbsolutePathOrURL), autoPlay: true);
    }

    public void OnDisable()
    {
        // stop video
        //Debug.Log("[video] 플레이 종료");
        Player.CloseMedia();
    }
}
