using UnityEngine;
using UnityEngine.UI;

public class GuideInfo : MonoBehaviour
{
    [Header("[ Info Values ]")]
    public string downloadLinkURL;

    [Space(10)]
    [Header("[ UI ]")]
    private Button downloadButton;

    public int buttonNum = 1;

    // Start is called before the first frame update
    void Start()
    {
        downloadButton = GetComponent<Button>();
        downloadButton.onClick.AddListener(OnDownloadLink);
    }

    public void OnDownloadLink()
    {
        if (DEV.instance.isUsingTestServer)
        {
            if (buttonNum == 1)
            {
                //GameManager.instance.popupManager.popups[(int)PopupType.FunctionUpdate].SetActive(true);
                Application.OpenURL(downloadLinkURL);
            }
            else
            {
                //GameManager.instance.popupManager.popups[(int)PopupType.FunctionUpdate].SetActive(true);
                Application.OpenURL(downloadLinkURL);
            }
        }
        else
        {
            if (buttonNum == 1)
            {
                //GameManager.instance.popupManager.popups[(int)PopupType.FunctionUpdate].SetActive(true);
                Application.OpenURL(downloadLinkURL);
            }
            else
            {
                //GameManager.instance.popupManager.popups[(int)PopupType.FunctionUpdate].SetActive(true);
                Application.OpenURL(downloadLinkURL);
            }
        }
    }

    public void SetLinkURL(string linkURL)
    {
        downloadLinkURL = linkURL;
    }

    public void ResetLinkURL()
    {
        downloadLinkURL = null;
    }
}
