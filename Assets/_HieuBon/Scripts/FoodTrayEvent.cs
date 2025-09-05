using UnityEngine;

public class FoodTrayEvent : MonoBehaviour
{
    FoodTray foodTray;

    private void Awake()
    {
        foodTray = GetComponentInParent<FoodTray>();
    }

    public void Toss()
    {
        foodTray.AfterToss();
    }

    public void EndToss()
    {
        LevelController.instance.foodTrays.CheckSphereCast();
    }
}
