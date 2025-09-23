using TMPro;
using UnityEngine;

public class UIShopPack : MonoBehaviour
{
    public TextMeshProUGUI value;
    public TextMeshProUGUI price;

    public void SetValueAndPrice(int value, float price)
    {
        this.value.text = value.ToString();
        this.price.text = "$" + price.ToString();
    }
}
