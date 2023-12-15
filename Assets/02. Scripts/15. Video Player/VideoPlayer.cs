using RenderHeads.Media.AVProVideo;
using UnityEngine;

public class VideoPlayer : MonoBehaviour
{
    public MediaPlayer Player;
    private string videoLink;

    public void Awake()
    {
        // link setting
        // test server
        if (DEV.instance.isUsingTestServer)
        {
            videoLink = "https://ejrdejzsaflk20717940.cdn.ntruss.com/onlinemuseum/video/ENG_METAPLY_VIDEO.mp4";

        }
        // live server
        else
        {
            videoLink = "https://yhbdymjatqnq20869625.cdn.ntruss.com/onlinemuseum/video/ENG_METAPLY_VIDEO.mp4";
        }
    }

    public void OnEnable()
    {
        // start video
        Player.enabled = true;
        Player.OpenMedia(new MediaPath(videoLink, MediaPathType.AbsolutePathOrURL), autoPlay: true);
    }

    public void OnDisable()
    {
        // stop video
        Player.CloseMedia();
    }
}
