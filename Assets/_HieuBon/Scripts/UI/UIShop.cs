using DG.Tweening;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIShop : MonoBehaviour
{
    public List<PurchasePack> packs;
    public List<PurchaseItemPack> itemPacks;

    public int goldRewardOnRemoveAds;

    public TextMeshProUGUI textGoldRewardOnRemoveAds;

    public GameObject adsInShop;

    VerticalLayoutGroup verticalLayoutGroup;

    UIShopPack[] uIShopPacks;
    UIShopItemPack[] uIShopItemPacks;

    Animation ani;

    public GameObject iconPlus;

    public Transform iconGoldOnRemoveAds;

    private void Awake()
    {
        textGoldRewardOnRemoveAds.text = goldRewardOnRemoveAds.ToString();

        ani = GetComponent<Animation>();

        uIShopPacks = GetComponentsInChildren<UIShopPack>();

        for (int i = 0; i < uIShopPacks.Length; i++)
        {
            uIShopPacks[i].SetValueAndPrice(packs[i].value, packs[i].price);
        }

        uIShopItemPacks = GetComponentsInChildren<UIShopItemPack>();

        for (int i = 0; i < uIShopItemPacks.Length; i++)
        {
            uIShopItemPacks[uIShopItemPacks.Length - i - 1].SetValueAndPrice(i + 1, itemPacks[i].magnet, itemPacks[i].shuffle
                , itemPacks[i].move, itemPacks[i].hammer, itemPacks[i].gold, itemPacks[i].price);
        }
    }

    private void Start()
    {
        int topPadding = ACEPlay.Bridge.BridgeController.instance.CanShowAds ? 230 : 330;

        adsInShop.SetActive(ACEPlay.Bridge.BridgeController.instance.CanShowAds);

        if (verticalLayoutGroup == null) verticalLayoutGroup = GetComponentInChildren<VerticalLayoutGroup>();

        verticalLayoutGroup.padding = new RectOffset(verticalLayoutGroup.padding.left,
            verticalLayoutGroup.padding.right,
            topPadding,
            verticalLayoutGroup.padding.bottom);
    }

    public void RemoveAdsPlus(bool isInShop)
    {
        AudioController.instance.PlaySoundNVibrate(AudioController.instance.button, 50);

        UnityStringEvent e = new UnityStringEvent();
        e.AddListener((result) =>
        {
            adsInShop.SetActive(false);
            UIController.instance.buttonAds.SetActive(false);

            ACEPlay.Bridge.BridgeController.instance.CanShowAds = false;

            GameManager.instance.Gold += goldRewardOnRemoveAds;

            UIController.instance.isRewardedPurchase = true;

            if (verticalLayoutGroup == null) verticalLayoutGroup = GetComponentInChildren<VerticalLayoutGroup>();

            verticalLayoutGroup.padding = new RectOffset(verticalLayoutGroup.padding.left,
            verticalLayoutGroup.padding.right,
            330,
            verticalLayoutGroup.padding.bottom);

            if (!isInShop)
            {
                UIController.instance.uIPanelRemoveAds.HideAds();

                UIController.instance.GoldFly(Vector2.zero, goldRewardOnRemoveAds);
            }
            else
            {
                UIController.instance.GoldFly(iconGoldOnRemoveAds.position, goldRewardOnRemoveAds);
            }
        });
        ACEPlay.Bridge.BridgeController.instance.PurchaseProduct(Application.identifier + "_removeadsplus", e);
    }

    public enum PurchaseType
    {
        Coin
    }

    public void Purchase(int index)
    {
        AudioController.instance.PlaySoundNVibrate(AudioController.instance.button, 50);

        PurchasePack purchasePack = packs[index];

        UnityStringEvent e = new UnityStringEvent();
        e.AddListener((result) =>
        {
            if (purchasePack.type == PurchaseType.Coin)
            {
                GameManager.instance.Gold += purchasePack.value;

                UIController.instance.GoldFly(uIShopPacks[index].transform.position, purchasePack.value);
            }
        });
        ACEPlay.Bridge.BridgeController.instance.PurchaseProduct(Application.identifier + "_pack" + Mathf.Round(purchasePack.price), e);
    }

    public void ItemPurchase(int index)
    {
        AudioController.instance.PlaySoundNVibrate(AudioController.instance.button, 50);

        PurchaseItemPack purchaseItemPack = itemPacks[index];

        UnityStringEvent e = new UnityStringEvent();
        e.AddListener((result) =>
        {
            if (purchaseItemPack.magnet > 0)
            {
                UIController.instance.uIInGame.uIBooster.AmountMagnet += purchaseItemPack.magnet;
            }
            if (purchaseItemPack.shuffle > 0)
            {
                UIController.instance.uIInGame.uIBooster.AmountShuffle += purchaseItemPack.shuffle;
            }
            if (purchaseItemPack.move > 0)
            {
                UIController.instance.uIInGame.uIBooster.AmountMove += purchaseItemPack.move;
            }
            if (purchaseItemPack.hammer > 0)
            {
                UIController.instance.uIInGame.uIBooster.AmountHammer += purchaseItemPack.hammer;
            }

            UIController.instance.uIInGame.uIBooster.CheckAmoutBooster();

            GameManager.instance.Gold += purchaseItemPack.gold;

            UIController.instance.GoldFly(uIShopItemPacks[uIShopItemPacks.Length - index - 1].t.transform.position, purchaseItemPack.gold);
        });
        ACEPlay.Bridge.BridgeController.instance.PurchaseProduct(Application.identifier + "_item_pack" + Mathf.Round(purchaseItemPack.price), e);
    }

    public void Show()
    {
        if (!iconPlus.activeSelf) return;

        LevelController.instance.gameState = LevelController.GameState.Pause;

        AudioController.instance.PlaySoundNVibrate(AudioController.instance.button, 50);

        gameObject.SetActive(true);
        iconPlus.SetActive(false);

        ani.Play("ShowPanelFade");
    }

    public void Hide()
    {
        ani.Play("HidePanelFade");

        UIController.instance.uIInGame.uIBooster.CheckAmoutBooster();

        LevelController.instance.gameState = LevelController.GameState.Playing;

        AudioController.instance.PlaySoundNVibrate(AudioController.instance.button, 50);

        DOVirtual.DelayedCall(ani.clip.length, delegate
        {
            gameObject.SetActive(false);
            iconPlus.SetActive(true);
        });
    }
}

[System.Serializable]
public class PurchasePack
{
    public UIShop.PurchaseType type;
    public int value;
    public float price;

    public PurchasePack(UIShop.PurchaseType type, int value, float price)
    {
        this.type = type;
        this.value = value;
        this.price = price;
    }
}

[System.Serializable]
public class PurchaseItemPack
{
    public int magnet;
    public int shuffle;
    public int move;
    public int hammer;
    public int gold;
    public float price;

}
