using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupManager : MonoBehaviour
{
    public GameObject[] popups;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    #region Login & Logout
    public void BTN_ConfirmLoginFail()
    {
        popups[0].SetActive(false);
    }

    public void ShowLogoutPage()
    {
        popups[1].SetActive(true);
    }

    public void BTN_ConfirmLogout()
    {
        popups[1].SetActive(false);
        GameManager.instance.ResetLauncher();
    }

    public void BTN_CancelLogout()
    {
        popups[1].SetActive(false);
    }
    #endregion

    #region Friend List

    #endregion
}
