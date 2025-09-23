using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using static GameController;
using UnityEngine.Events;
using ACEPlay.Bridge;

public class UIPanelRemoveAds : MonoBehaviour
{
    CanvasGroup canvasGroup;

    public RectTransform rectPopup;

    public CanvasGroup buttonPlayWithAds;
    public RectTransform rectButton;

    public GameObject panel;

    public int LevelJustShowed
    {
        get
        {
            return PlayerPrefs.GetInt("LevelJustShowed");
        }
        set
        {
            PlayerPrefs.SetInt("LevelJustShowed", value);
        }
    }

    public void CheckToDisplay()
    {
        if (LevelJustShowed == GameManager.instance.CurrentLevel) return;

        int level = GameManager.instance.CurrentLevel;

        LevelJustShowed = level;

        int i = 5;
        int count = 0;

        while (i + 5 * count <= level)
        {
            if (level == i + 5 * count)
            {
                ShowAdsHandle();

                return;
            }

            count++;
        }
    }

    void ShowAdsHandle()
    {
        if (canvasGroup == null) canvasGroup = panel.GetComponent<CanvasGroup>();
        
        Time.timeScale = 0;

        panel.SetActive(true);

        canvasGroup.DOFade(1f, 0.25f).SetUpdate(true);

        rectPopup.DOScale(1f, 0.25f).SetEase(Ease.OutBack).SetUpdate(true);

        buttonPlayWithAds.DOFade(1f, 0.25f).SetUpdate(true).SetDelay(3f).OnStart(delegate
        {
            buttonPlayWithAds.gameObject.SetActive(true);
        });

        rectButton.DOScale(1.15f, 0.35f).SetUpdate(true).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.Linear);
    }

    public void ShowAds()
    {
        AudioController.instance.PlaySoundNVibrate(AudioController.instance.button, 50);

        ShowAdsHandle();
    }

    public void HideAds()
    {
        AudioController.instance.PlaySoundNVibrate(AudioController.instance.button, 50);

        Time.timeScale = 1;

        panel.SetActive(false);
        buttonPlayWithAds.gameObject.SetActive(false);

        canvasGroup.alpha = 0;
        buttonPlayWithAds.alpha = 0;

        rectPopup.localScale = Vector3.zero;

        rectButton.DOKill();

        rectButton.localScale = Vector3.one * 1.35f;
    }

    public void HideAdsWithAds()
    {
        AudioController.instance.PlaySoundNVibrate(AudioController.instance.button, 50);

        UnityEvent e = new UnityEvent();
        e.AddListener(() =>
        {
            Time.timeScale = 1;

            panel.SetActive(false);
            buttonPlayWithAds.gameObject.SetActive(false);

            canvasGroup.alpha = 0;
            buttonPlayWithAds.alpha = 0;

            rectPopup.localScale = Vector3.zero;

            rectButton.DOKill();

            rectButton.localScale = Vector3.one * 1.35f;
        });

        ACEPlay.Bridge.BridgeController.instance.ShowInterstitial("playwithads", e);
    }
}
