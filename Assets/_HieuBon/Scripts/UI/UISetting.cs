using ACEPlay.Native;
using DG.Tweening;
using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UISetting : MonoBehaviour
{
    public UISettingOption[] settingOptions;

    public GameObject buttonRestore;
    public GameObject popup;

    CanvasGroup canvasGroup;

    Animation ani;

    private void Awake()
    {
#if UNITY_IOS
        buttonRestore.SetActive(true);
#endif

        GetComponent<CanvasGroup>().alpha = 0;

        ani = GetComponent<Animation>();

        popup.transform.localScale = Vector3.zero;
    }

    private void Start()
    {
        settingOptions[0].SwitchStateHandle(GameManager.instance.IsActiveVibrate);
        settingOptions[1].SwitchStateHandle(GameManager.instance.IsAtiveSound);
    }

    public void ChangeSettingOption(UISettingOption settingOption, bool isActive)
    {
        AudioController.instance.PlaySoundNVibrate(AudioController.instance.button, 50);

        if (settingOption.type == TypeSetting.Sound) GameManager.instance.IsAtiveSound = isActive;
        if (settingOption.type == TypeSetting.Vibrate) GameManager.instance.IsActiveVibrate = isActive;

        settingOption.SwitchStateHandle(isActive);

    }

    public enum TypeSetting
    {
        None, Sound, Vibrate
    }

    public void Show()
    {
        gameObject.SetActive(true);

        ani.Play("ShowPanelScale");

        ACEPlay.Bridge.BridgeController.instance.ShowMRECs();
        NativeAds.instance.DisplayNativeAds(true);

        AudioController.instance.PlaySoundNVibrate(AudioController.instance.button, 50);
    }

    public void Hide()
    {
        if (!gameObject.activeSelf) return;

        ani.Play("HidePanelScale");

        DOVirtual.DelayedCall(ani.clip.length, delegate
        {
            gameObject.SetActive(false);
        });

        NativeAds.instance.DisplayNativeAds(false);
        ACEPlay.Bridge.BridgeController.instance.HideMRECs();

        AudioController.instance.PlaySoundNVibrate(AudioController.instance.button, 50);
    }

    public void Youtube()
    {
        AudioController.instance.PlaySoundNVibrate(AudioController.instance.button, 50);

        ACEPlay.Bridge.BridgeController.instance.SubcribeYoutube(null);
    }

    public void RestorePurchase()
    {
        AudioController.instance.PlaySoundNVibrate(AudioController.instance.button, 50);

        ACEPlay.Bridge.BridgeController.instance.RestorePurchase();
    }
}
