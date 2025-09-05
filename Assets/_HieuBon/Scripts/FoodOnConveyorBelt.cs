using System.Collections.Generic;
using UnityEngine;

public class FoodOnConveyorBelt : MonoBehaviour
{
    public static FoodOnConveyorBelt instance;

    [HideInInspector]
    public FoodQueue foodQueue;

    public List<Food> foods = new List<Food>();
    public List<Food> foodsOnConveyorBelt = new List<Food>();

    public int FoodLength
    {
        get
        {
            return foods.Count;
        }
    }

    private void Awake()
    {
        instance = this;

        foodQueue = GetComponentInChildren<FoodQueue>();
    }

    public bool IsOnConveyorBelt(Food food)
    {
        return foodsOnConveyorBelt.Contains(food);
    }

    public void AddFood(Food food)
    {
        foods.Add(food);
    }

    public void RemoveFood()
    {
        foods.RemoveAt(0);
    }

    public void AddFoodOnConveyorBelt(Food food)
    {
        food.foodPoint.isEmpty = false;
        foodsOnConveyorBelt.Add(food);
    }

    public void RemoveFoodOnConveyorBelt(Food food)
    {
        food.foodPoint.isEmpty = true;
        foodsOnConveyorBelt.Remove(food);
    }

    public Food GetFood(FoodPoint foodPoint)
    {
        foods[0].foodPoint = foodPoint;

        return foods[0];
    }

    public void SetParent(Food food)
    {
        food.transform.SetParent(transform);
    }

    public Food GetFood(GameObject obj)
    {
        foreach (var e in foodsOnConveyorBelt)
        {
            if (e.gameObject == obj) return e;
        }

        return null;
    }
}
