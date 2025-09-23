using DG.Tweening;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class FoodTrays : MonoBehaviour
{
    public GameObject preFoodTray;

    public FoodTray[] foodTrays;

    public float yDistance;

    [HideInInspector]
    public List<IceTray> iceTrays = new List<IceTray>();

    public void GenerateFoodTrays(TrayData[] trayDatas)
    {
        foodTrays = new FoodTray[trayDatas.Length];

        for (int i = 0; i < trayDatas.Length; i++)
        {
            GameObject e = Instantiate(preFoodTray, transform);

            IceTray iceTray = e.GetComponentInChildren<IceTray>(true);

            e.transform.position = trayDatas[i].position;
            e.transform.rotation = trayDatas[i].direction;

            foodTrays[i] = e.GetComponent<FoodTray>();
            foodTrays[i].GenerateFood(trayDatas[i].foodType);

            if (iceTray != null)
            {
                iceTrays.Add(iceTray);
                iceTray.SetAmount(trayDatas[i].amountFreeze);
            }
        }
    }

    public void CheckSphereCast()
    {
        foreach (var e in foodTrays)
        {
            e.CheckSphereCast();
        }
    }

    public bool IsOutOfTray()
    {
        foreach (var e in foodTrays)
        {
            if (e.boxCollider.enabled) return false;
        }

        return true;
    }

    public List<FoodTray> result;

    public List<FoodTray> GetSortFoodTrays()
    {
        result = new List<FoodTray>();

        for (int i = 0; i < foodTrays.Length; i++)
        {
            if (foodTrays[i].boxCollider.enabled)
            {
                result.Add(foodTrays[i]);
            }
        }

        for (int i = 0; i < result.Count - 1; i++)
        {
            for (int j = i + 1; j < result.Count; j++)
            {
                if (result[i].transform.position.y < result[j].transform.position.y)
                {
                    FoodTray temp = result[i];
                    result[i] = result[j];
                    result[j] = temp;
                }
            }
        }

        return result;
    }

    public void BossterDisturbance()
    {
        List<Transform> t = new List<Transform>();
        List<Quaternion> r = new List<Quaternion>();
        List<Vector3> p = new List<Vector3>();

        for (int i = 0; i < foodTrays.Length; i++)
        {
            if (foodTrays[i].boxCollider.enabled)
            {
                foodTrays[i].Holding();

                t.Add(foodTrays[i].transform);
                p.Add(foodTrays[i].transform.position);
                r.Add(foodTrays[i].transform.rotation);
            }
        }

        List<Transform> random = new List<Transform>();

        while (t.Count > 0)
        {
            int index = Random.Range(0, t.Count);

            random.Add(t[index]);

            t.RemoveAt(index);
        }

        bool isNotSame = false;

        for (int i = 0; i < random.Count; i++)
        {
            if (random[i].position != p[i]) isNotSame = true;
        }

        if (!isNotSame) random.Reverse();

        for (int i = 0; i < random.Count; i++)
        {
            random[i].transform.position = p[i];
            random[i].transform.rotation = r[i];
        }

        CheckSphereCast();
    }

    public void BoosterBreakFreeze()
    {
        float yHighest = -1;

        foreach (var e in iceTrays)
        {
            if (e.transform.position.y > yHighest && e.amount > 0) yHighest = e.transform.position.y;
        }
        
        foreach (var e in iceTrays)
        {
            if (e.transform.position.y == yHighest && e.amount > 0) e.Break();
        }
    }

    public void MinusFreeze()
    {
        foreach (var e in iceTrays)
        {
            e.MinusFreeze();
        }
    }
}
