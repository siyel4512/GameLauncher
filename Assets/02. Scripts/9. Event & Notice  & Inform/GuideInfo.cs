using UnityEngine;
using UnityEngine.UI;

public class GuideInfo : MonoBehaviour
{
    public string downloadLinkURL;
    private Button downloadButton;

    // Start is called before the first frame update
    void Start()
    {
        downloadButton = GetComponent<Button>();
        SetLinkURL("https://www.google.com/");
    }

    // Update is called once per frame
    void Update()
    {
        
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
