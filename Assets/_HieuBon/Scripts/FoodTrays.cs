using UnityEditor;
using UnityEngine;

public class FoodTrays : MonoBehaviour
{
    public GameObject preFoodTray;

    public FoodTray[] foodTrays;

    public float yDistance;

    public void GenerateFoodTrays(TrayData[] trayDatas)
    {
        foodTrays = new FoodTray[trayDatas.Length];

        for (int i = 0; i < trayDatas.Length; i++)
        {
            GameObject e = Instantiate(preFoodTray, transform);

            e.transform.position = trayDatas[i].position;
            e.transform.rotation = trayDatas[i].direction;

            foodTrays[i] = e.GetComponent<FoodTray>();
            foodTrays[i].GenerateFood(trayDatas[i].foodType);
        }

        CheckSphereCast();
    }

    public void CheckSphereCast()
    {
        foreach (var e in foodTrays)
        {
            e.CheckSphereCast();
        }
    }
}
