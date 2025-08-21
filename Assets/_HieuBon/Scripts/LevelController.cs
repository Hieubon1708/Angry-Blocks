using UnityEngine;

public class LevelController : MonoBehaviour
{
    public int amount;

    public GameObject preFoodTray;

    private void Awake()
    {
        TrayPosition traysPosition = GameController.instance.traysPosition.trayAmount[amount - 1];

        for (int i = 0; i < amount; i++)
        {
            GameObject e = Instantiate(preFoodTray, transform);

            e.transform.localPosition = traysPosition.pos[i];
        }
    }
}
