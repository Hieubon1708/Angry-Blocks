using DG.Tweening;
using System.Collections;
using UnityEngine;
using TA;

public class UIConnectionError : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject pnlConnectionError;
    //[SerializeField] private Transform dialog;

    private void Awake()
    {
        DontDestroyOnLoad(this);
    }

    IEnumerator Start()
    {
        yield return new WaitUntil(() => ConnectInternetManager.instance != null);

        ConnectInternetManager.instance.onConnectInternet += () =>
        {
            DisplayPanelConnectionError(false);
        };
        ConnectInternetManager.instance.onDisconnectInternet += () =>
        {
            DisplayPanelConnectionError(true);
        };
    }

    public void DisplayPanelConnectionError(bool enable)
    {
        if (enable)
        {
            if (ACEPlay.Bridge.BridgeController.instance.InternetRequire)
            {
                pnlConnectionError.SetActive(true);
               // dialog.DOPunchScale(Vector3.one * 0.03f, 0.2f, 20, 1);
                Time.timeScale = 0f;
            }
        }
        else
        {
            pnlConnectionError.SetActive(false);
            Time.timeScale = 1f;
        }
    }

    public void OnClickButtonTryAgain()
    {
        ConnectInternetManager.instance.Check();
    }
}
