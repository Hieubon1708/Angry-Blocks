using DG.Tweening;
using TMPro;
using UnityEngine;

public class IceTray : MonoBehaviour
{
    public int amount;

    TextMeshProUGUI textAmount;

    FoodTray foodTray;

    public Rigidbody rb;
    public ParticleSystem smoke;

    public void SetAmount(int amount)
    {
        if (textAmount == null) textAmount = GetComponentInChildren<TextMeshProUGUI>();
        if (foodTray == null) foodTray = GetComponentInParent<FoodTray>();

        if (amount > 0)
        {
            gameObject.SetActive(true);

            foodTray.ActiveFoods(false);
        }
        else
        {
            gameObject.SetActive(false);

            return;
        }

        this.amount = amount;
        textAmount.text = amount.ToString();

        textAmount.transform.rotation = Quaternion.LookRotation(new Vector3(0, -1, 0));
    }

    public void Break()
    {
        if (amount == 0) return;

        foodTray.ActiveFoods(true);

        textAmount.text = "";

        rb.isKinematic = false;
        rb.AddForce(Vector3.up * 333, ForceMode.Impulse);

        smoke.Play();

        DOVirtual.DelayedCall(2.5f, delegate
        {
            gameObject.SetActive(false);
        });
    }

    public void MinusFreeze()
    {
        if (amount == 0) return;

        amount--;
        textAmount.text = amount.ToString();

        if (amount == 0)
        {
            foodTray.ActiveFoods(true);

            gameObject.SetActive(false);

            textAmount.text = "";
        }
    }
}
