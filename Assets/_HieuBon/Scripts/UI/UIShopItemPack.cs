using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIShopItemPack : MonoBehaviour
{
    public TextMeshProUGUI amountMagnet;
    public TextMeshProUGUI amountShuffle;
    public TextMeshProUGUI amountMove;
    public TextMeshProUGUI amountHammer;
    public TextMeshProUGUI gold;
    public TextMeshProUGUI price;
    public TextMeshProUGUI indexPack;

    public Transform t;

    public void SetValueAndPrice(int indexPack, int amountMagnet, int amountShuffle, int amountMove, int amountHammer, int gold, float price)
    {
        this.amountMagnet.text = amountMagnet.ToString();
        this.amountShuffle.text = amountShuffle.ToString();
        this.amountMove.text = amountMove.ToString();
        if (this.amountHammer != null) this.amountHammer.text = amountHammer.ToString();
        this.gold.text = gold.ToString();
        this.price.text = "$" + price.ToString();
        this.indexPack.text = "Pack " + indexPack.ToString();
    }
}
