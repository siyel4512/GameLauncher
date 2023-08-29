using UnityEngine;
using UnityEngine.UI;

public class GuideInfo : MonoBehaviour
{
    [Header("[ Info Values ]")]
    public string downloadLinkURL;

    [Space(10)]
    [Header("[ UI ]")]
    private Button downloadButton;

    // Todo : 추후 삭제 필요
    public int buttonNum = 1;

    // Start is called before the first frame update
    void Start()
    {
        downloadButton = GetComponent<Button>();
        downloadButton.onClick.AddListener(OnDownloadLink);
    }

    public void OnDownloadLink()
    {
        if (buttonNum == 1)
        {
            Application.OpenURL(downloadLinkURL);
        }
        else
        {
            GameManager.instance.popupManager.popups[(int)PopupType.FunctionUpdate].SetActive(true);
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
