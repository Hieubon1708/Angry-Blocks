using DG.Tweening;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class FoodQueue : MonoBehaviour
{
    public Transform[] points;

    Slot[] slots;

    private void Awake()
    {
        slots = new Slot[points.Length];

        for (int i = 0; i < points.Length; i++)
        {
            GameObject col = points[i].gameObject;

            col.SetActive(false);

            slots[i] = new Slot(true, col);
        }
    }

    public void AddFood(Food food)
    {
        int slotIndex = GetSlotEmpty();

        if (slotIndex == -1)
        {
            UIController.instance.ShowPaneLose();

            return;
        }

        slots[slotIndex].isEmpty = false;

        food.transform.DOJump(points[slotIndex].position, 15, 1, 0.25f).OnComplete(delegate
        {
            slots[slotIndex].AddFood(food);
            food.AniSale();
        });
    }

    public void OnConveyorBelt(GameObject col)
    {
        int slotIndex = GetIndexByCol(col);

        if (slotIndex == -1)
        {
            Debug.LogError("-1");

            return;
        }

        GameController.FoodType type = slots[slotIndex].food.foodType;

        List<int> indexes = new List<int>() { slotIndex };

        for (int i = slotIndex; i < slots.Length - 1; i++)
        {
            if (slots[i + 1].col.activeSelf && slots[i + 1].food.foodType == type)
            {
                indexes.Add(i + 1);
            }
            else break;
        }

        for (int i = slotIndex; i > 0; i--)
        {
            if (slots[i - 1].col.activeSelf && slots[i - 1].food.foodType == type)
            {
                indexes.Add(i - 1);
            }
            else break;
        }

        indexes.Sort();

        for (int i = 0; i < indexes.Count; i++)
        {
            int index = i;

            Vector3 pos = LevelController.instance.conveyorBelt.cachedPathPoints[0];

            FoodOnConveyorBelt.instance.SetParent(slots[indexes[index]].food);

            float time = index * 0.15f + 0.25f;

            slots[indexes[index]].food.transform.DOJump(pos, 15, 1, time).OnComplete(delegate
            {
                FoodOnConveyorBelt.instance.AddFood(slots[indexes[index]].food);

            });
            slots[indexes[index]].RemoveFood();
        }
    }

    int GetIndexByCol(GameObject col)
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].col.gameObject == col) return i;
        }

        return -1;
    }

    int GetSlotEmpty()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].isEmpty) return i;
        }

        return -1;
    }
}

public class Slot
{
    public Food food;
    public bool isEmpty;
    public GameObject col;

    public Slot(bool isEmpty, GameObject col)
    {
        this.isEmpty = isEmpty;
        this.col = col;
    }

    public void AddFood(Food food)
    {
        this.food = food;
        col.SetActive(true);
    }

    public void RemoveFood()
    {
        isEmpty = true;
        col.SetActive(false);
    }
}
