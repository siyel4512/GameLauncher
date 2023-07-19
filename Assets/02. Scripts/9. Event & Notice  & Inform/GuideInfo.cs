using UnityEngine;
using UnityEngine.UI;

public class GuideInfo : MonoBehaviour
{
    [Header("[ Info Values ]")]
    public string downloadLinkURL;

    [Space(10)]
    [Header("[ UI ]")]
    private Button downloadButton;

    // Start is called before the first frame update
    void Start()
    {
        downloadButton = GetComponent<Button>();
    }

    public void SetLinkURL(string linkURL)
    {
        downloadLinkURL = linkURL;
        downloadButton.onClick.AddListener(OnDownloadLink);
    }
    public void OnDownloadLink()
    {
        Application.OpenURL(downloadLinkURL);
    }
}
