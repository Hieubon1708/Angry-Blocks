using ACEPlay.Native;
using UnityEngine;

public class NativeAdsController : MonoBehaviour
{
    public static NativeAdsController instance;
    public NativeAds NativeAdsTop;
    public NativeAds NativeAdsBottom;

    private void Awake()
    {
        if (instance == null) instance = this;
    }

    public void SetNative(bool isShowMrec, bool isShowNativeTop, bool isShowNativeBottom)
    {
        if (isShowMrec) ACEPlay.Bridge.BridgeController.instance.ShowMRECs();
        else ACEPlay.Bridge.BridgeController.instance.HideMRECs();

        NativeAdsBottom.DisplayNativeAds(isShowNativeBottom);
        NativeAdsTop.DisplayNativeAds(isShowNativeTop);
    }
}
