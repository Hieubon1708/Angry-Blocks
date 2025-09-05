using System;
using System.Reflection;
using UnityEngine;

public class TrayTool : MonoBehaviour
{
    public GameObject[] preFood;
    [HideInInspector]
    public GameObject[] food = new GameObject[4];
    [HideInInspector]
    public int[] foodTypes;
    public GameObject[] points;

    private void Start()
    {
        foodTypes = new int[4];
    }

    public void SetFood(GameObject point, GameController.FoodType foodType)
    {
        int index = GetIndex(point);

        if (food[index] != null) Destroy(food[index]);
        Debug.Log(foodTypes.Length);
        food[index] = Instantiate(preFood[(int)foodType], points[index].transform.position, points[index].transform.rotation, transform);
        foodTypes[index] = (int)foodType;
    }

    public void SetAll(GameController.FoodType foodType)
    {
        for (int i = 0; i < food.Length; i++)
        {
            if (food[i] != null) Destroy(food[i]);
            food[i] = Instantiate(preFood[(int)foodType], points[i].transform.position, points[i].transform.rotation, transform);
            foodTypes[i] = (int)foodType;
        }
    }

    int GetIndex(GameObject point)
    {
        for (int i = 0; i < points.Length; i++)
        {
            if (points[i] == point) return i;
        }

        return -1;
    }

    public void SetFoodAll()
    {
        Tool.instance.OnClickFoodAll(gameObject);
    }
}
