using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ACEPlay.Native
{
    public class NativeAds : MonoBehaviour
    {
        public static NativeAds instance;

#if UNITY_ANDROID
        [SerializeField]
        string adUnitId = "ca-app-pub-7053040887429247/9708643054";
        string idTest = "ca-app-pub-3940256099942544/2247696110";
#elif UNITY_IOS
        string adUnitId = "ca-app-pub-6632644511740977/2493351992";
        string idTest = "ca-app-pub-3940256099942544/3986624511";
#endif
        public enum Position
        {
            Top, Bottom
        }

        [Header("UI Native")]
        [SerializeField] private bool isIconNative;
        [SerializeField] private GameObject nativeAds;
        [SerializeField] private RawImage rawMainImage, rawIconImage;
        [SerializeField] private TextMeshProUGUI txtTitle;
        [SerializeField] private TextMeshProUGUI txtDescription;
        [SerializeField] private TextMeshProUGUI txtButton;

        public float maxWidthMain = 200, maxHeghtMain = 200;
        public float maxWidthIcon = 200, maxHeghtIcon = 200;
        
        bool nativeAdLoaded;
        bool isShowNativeAds;
        int retryAttemptNative;

        private void Awake()
        {
            instance = this;

            DisplayNativeAds(false);
        }

        public void DisplayNativeAds(bool enable)
        {
            nativeAds.SetActive(enable);
        }
    }
}
