using UnityEngine;
using UnityEngine.UI;

public class GuideInfo : MonoBehaviour
{
    public string downloadLinkURL;
    private Button downloadButton;

    // Start is called before the first frame update
    void Start()
    {
        downloadButton.onClick.AddListener(OnDownloadLink);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnDownloadLink()
    {

    }
}
