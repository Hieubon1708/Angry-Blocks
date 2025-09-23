using ACEPlay.Bridge;
using ACEPlay.Native;
using DG.Tweening;
using System;
using System.Drawing;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class UIController : MonoBehaviour
{
    public static UIController instance;

    public GameObject panelWin;
    public GameObject panelLose;

    public Camera uICamera;

    [HideInInspector]
    public UIInGame uIInGame;
    [HideInInspector]
    public UIFade uIFade;
    [HideInInspector]
    public UISetting uISetting;
    [HideInInspector]
    public UIShop uIShop;
    [HideInInspector]
    public UITutorial uITutorial;
    [HideInInspector]
    public UIPanelRemoveAds uIPanelRemoveAds;

    public GameObject buttonAds;

    public TextMeshProUGUI textGold;

    [Header("Gold Fly")]
    public RectTransform[] goldsFly;
    int goldFlyIndex;
    public Transform iconTarget;

    public bool isRewardedPurchase
    {
        get { return PlayerPrefs.GetInt("isRewardedPurchase", 0) == 1; }
        set { PlayerPrefs.SetInt("isRewardedPurchase", value ? 1 : 0); }
    }

    private void Awake()
    {
        instance = this;

        uIInGame = GetComponent<UIInGame>();
        uIFade = GetComponent<UIFade>();
        uISetting = GetComponentInChildren<UISetting>(true);
        uIShop = GetComponentInChildren<UIShop>(true);
        uITutorial = GetComponentInChildren<UITutorial>(true);
        uIPanelRemoveAds = GetComponentInChildren<UIPanelRemoveAds>(true);
    }

    private void Start()
    {
        buttonAds.SetActive(ACEPlay.Bridge.BridgeController.instance.CanShowAds);

        if (BridgeController.instance.CheckOwnerNonConsumable(Application.identifier + "_removeadsplus"))
        {
            if (!isRewardedPurchase)
            {
                isRewardedPurchase = true;
                GameManager.instance.Gold += uIShop.goldRewardOnRemoveAds;
            }
        }

        UpdateGold();
    }

    public void ShowPanelWin()
    {
        AudioController.instance.PlaySoundNVibrate(AudioController.instance.win, 100);

        ACEPlay.Bridge.BridgeController.instance.ShowMRECs();
        NativeAds.instance.DisplayNativeAds(true);

        uIInGame.PlayVfxWin();

        LevelController.instance.gameState = LevelController.GameState.Win;
        panelWin.SetActive(true);
    }

    public void UpdateGold()
    {
        textGold.text = GameManager.instance.Gold.ToString();
    }

    public void IncreaseGold(int goldIncrease)
    {
        int from = GameManager.instance.Gold - goldIncrease;
        int to = GameManager.instance.Gold;

        DOVirtual.Int(from, to, 0.5f, (v) =>
        {
            textGold.text = v.ToString();
        });
    }

    public void MinusGold(int goldMinus)
    {
        int from = GameManager.instance.Gold + goldMinus;
        int to = GameManager.instance.Gold;

        DOVirtual.Int(from, to, 0.5f, (v) =>
        {
            textGold.text = v.ToString();
        });
    }

    public void HidePanelWin()
    {
        NativeAds.instance.DisplayNativeAds(false);
        ACEPlay.Bridge.BridgeController.instance.HideMRECs();

        uIInGame.StopVfxWin();

        panelWin.SetActive(false);

        uIInGame.isReward = false;
    }

    public void ShowPanelLose()
    {
        AudioController.instance.PlaySoundNVibrate(AudioController.instance.lose, 100);

        ACEPlay.Bridge.BridgeController.instance.ShowMRECs();
        NativeAds.instance.DisplayNativeAds(true);

        LevelController.instance.gameState = LevelController.GameState.Lose;
        panelLose.SetActive(true);
    }

    public void HidePanelLose()
    {
        NativeAds.instance.DisplayNativeAds(false);
        ACEPlay.Bridge.BridgeController.instance.HideMRECs();

        panelLose.SetActive(false);
    }

    public void Win()
    {
        if (LevelController.instance.gameState == LevelController.GameState.Pause) return;

        UnityEvent e = new UnityEvent();
        e.AddListener(() =>
        {
            uIInGame.Win();

            ShowPanelWin();
        });

        UnityEvent onDone = new UnityEvent();
        onDone.AddListener(() =>
        {
            BridgeController.instance.PlayCount = 0;
        });
        ACEPlay.Bridge.BridgeController.instance.ShowInterstitial("win", e, onDone);
    }

    public void Lose()
    {
        if (LevelController.instance.gameState == LevelController.GameState.Pause) return;

        UnityEvent e = new UnityEvent();
        e.AddListener(() =>
        {
            ShowPanelLose();
        });

        UnityEvent onDone = new UnityEvent();
        onDone.AddListener(() =>
        {
            BridgeController.instance.PlayCount = 0;
        });
        ACEPlay.Bridge.BridgeController.instance.ShowInterstitial("lose", e, onDone);
    }

    public void NextLevel(bool isX)
    {
        if (uIInGame.isReward) return;

        AudioController.instance.PlaySoundNVibrate(AudioController.instance.button, 50);

        uIInGame.NextLevel(delegate
        {
            UnityEvent e = new UnityEvent();
            e.AddListener(() =>
            {
                uIFade.FadeIn(delegate
                {
                    HidePanelWin();

                    GameManager.instance.Level++;
                    GameManager.instance.CurrentLevel++;

                    GameController.instance.LoadLevel();

                    uIFade.FadeOut(null);
                });
            });
            ACEPlay.Bridge.BridgeController.instance.ShowInterstitial("next_level", e);
        }, isX);
    }

    public void RemoveAds()
    {
        AudioController.instance.PlaySoundNVibrate(AudioController.instance.button, 50);

        UnityStringEvent e = new UnityStringEvent();
        e.AddListener((result) =>
        {
            ACEPlay.Bridge.BridgeController.instance.CanShowAds = false;

            buttonAds.SetActive(false);
        });
        ACEPlay.Bridge.BridgeController.instance.PurchaseProduct(Application.identifier + "_removeads", e);
    }

    public void SkipLevel()
    {
        AudioController.instance.PlaySoundNVibrate(AudioController.instance.button, 50);

        UnityEvent eReward = new UnityEvent();
        eReward.AddListener(() =>
        {
            uIFade.FadeIn(delegate
            {
                HidePanelLose();

                GameManager.instance.Level++;
                GameManager.instance.CurrentLevel++;

                GameController.instance.LoadLevel();

                uIFade.FadeOut(null);
            });
        });
        ACEPlay.Bridge.BridgeController.instance.ShowRewarded("skip", eReward, null);
    }

    public void Replay()
    {
        AudioController.instance.PlaySoundNVibrate(AudioController.instance.button, 50);

        LevelController.instance.gameState = LevelController.GameState.Pause;

        UnityEvent e = new UnityEvent();
        e.AddListener(() =>
        {
            uISetting.Hide();

            uIFade.FadeIn(delegate
            {
                HidePanelLose();

                GameController.instance.LoadLevel();

                uIFade.FadeOut(null);
            });
        });

        UnityEvent onDone = new UnityEvent();
        onDone.AddListener(() =>
        {
            BridgeController.instance.PlayCount = 0;
        });

        ACEPlay.Bridge.BridgeController.instance.ShowInterstitial("replay", e, onDone);
    }

    public void GoldFly(Vector3 startPosition, int gold, Action onLengthDone = null)
    {
        startPosition.z = goldsFly[0].position.z;

        int count = 0;

        for (int i = 0; i < 10; i++)
        {
            Vector3 random = UnityEngine.Random.insideUnitSphere * 1;

            int index = goldFlyIndex;

            Vector3 targetPos = new Vector3(startPosition.x + random.x, startPosition.y + random.y, startPosition.z);

            goldsFly[index].gameObject.SetActive(true);
            goldsFly[index].position = startPosition;
            goldsFly[index].DOMove(targetPos, 0.35f).OnComplete(delegate
            {
                goldsFly[index].DOMove(iconTarget.position, 1f).SetDelay(UnityEngine.Random.Range(0.15f, 0.75f)).SetEase(Ease.InBack).OnComplete(delegate
                {
                    goldsFly[index].gameObject.SetActive(false);

                    iconTarget.DOKill();
                    iconTarget.DOScale(0.8f, 0.15f).OnComplete(delegate { iconTarget.DOScale(0.7f, 0.15f); });

                    count++;
                    if (count == 1)
                    {
                        IncreaseGold(gold);
                    }
                    if (count == 9)
                    {
                        if (onLengthDone != null) onLengthDone.Invoke();
                    }
                    AudioController.instance.PlaySoundNVibrate(AudioController.instance.goldReward, 50);
                });
            });

            goldFlyIndex++;

            if (goldFlyIndex == 30) goldFlyIndex = 0;
        }
    }
}
