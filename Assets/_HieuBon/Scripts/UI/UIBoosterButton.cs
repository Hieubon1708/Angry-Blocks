using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIBoosterButton : MonoBehaviour
{
    public GameObject amount;
    public GameObject iconAds;
    public GameObject gold;

    public TextMeshProUGUI txtGold;
    public TextMeshProUGUI txtAmount;

    public RectTransform icon;

    float scale;

    private void Awake()
    {
        scale = icon.localScale.x;
    }

    public void SetGold(int gold)
    {
        txtGold.text = gold.ToString() + "<sprite=0>";
    }

    public void CheckButton(int amount, bool isEnough)
    {
        if (amount > 0)
        {
            this.amount.SetActive(true);
            iconAds.SetActive(false);
            gold.SetActive(false);

            txtAmount.text = amount.ToString();

            icon.localScale = Vector3.one * scale * 1.25f;

            icon.anchoredPosition = new Vector2(icon.anchoredPosition.x, 10f);
        }
        else
        {
            this.amount.SetActive(false);
            iconAds.SetActive(!isEnough);
            gold.SetActive(isEnough);

            icon.localScale = Vector3.one * scale;

            icon.anchoredPosition = new Vector2(icon.anchoredPosition.x, 35f);
        }
    }
}
