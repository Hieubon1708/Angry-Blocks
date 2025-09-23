using DG.Tweening;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

public class FoodQueue : MonoBehaviour
{
    public List<Transform> points;
    public List<Transform> adsPoints;

    List<Slot> slots = new List<Slot>();

    public GameObject buttonAds;

    public MeshRenderer line;

    Color[] lineStartColors = new Color[2];

    private void Awake()
    {
        for (int i = 0; i < points.Count; i++)
        {
            GameObject col = points[i].gameObject;

            col.SetActive(false);

            slots.Add(new Slot(true, col));
        }

        for (int i = 0; i < line.materials.Length; i++)
        {
            lineStartColors[i] = line.materials[i].color;
            line.materials[i].color = line.materials[i].color * 0.65f;
        }
    }

    public void AddSlot()
    {
        AudioController.instance.PlaySoundNVibrate(AudioController.instance.button, 50);
        AudioController.instance.PlaySoundNVibrate(AudioController.instance.addBox);

        UnityEvent eReward = new UnityEvent();
        eReward.AddListener(() =>
        {
            for (int i = 0; i < line.materials.Length; i++)
            {
                line.materials[i].color = lineStartColors[i];
            }

            buttonAds.SetActive(false);

            while (adsPoints.Count > 0)
            {
                Transform p = adsPoints[adsPoints.Count - 1].transform;

                points.Insert(8, p);

                adsPoints.Remove(p);

                GameObject col = p.gameObject;

                col.SetActive(false);

                slots.Add(new Slot(true, col));
            }
        });
        ACEPlay.Bridge.BridgeController.instance.ShowRewarded("add_slot", eReward, null);
    }

    public void AddFood(Food food)
    {
        int slotIndex = GetSlotEmpty();

        if (slotIndex == -1)
        {
            if (LevelController.instance.gameState == LevelController.GameState.Win
         || LevelController.instance.gameState == LevelController.GameState.Lose) return;

            UIController.instance.Lose();

            return;
        }

        slots[slotIndex].isEmpty = false;

        Vector3 eulers = food.transform.eulerAngles;
        eulers.y += 1080;

        food.transform.DORotate(eulers, 0.5f, RotateMode.FastBeyond360);
        food.transform.DOJump(points[slotIndex].position, 15, 1, 0.5f).OnComplete(delegate
        {
            slots[slotIndex].AddFood(food);
            food.AniSale();

            AudioController.instance.PlaySoundNVibrate(AudioController.instance.onQueue, 25);
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

        AudioController.instance.PlaySoundNVibrate(null, 50);

        List<int> indexes = new List<int>();

        int start = 0;

        if (slotIndex >= 4) start = 4;
        if (slotIndex >= 8) start = 8;

        for (int i = start; i < start + 4; i++)
        {
            if (slots[i].col.activeSelf) indexes.Add(i);
        }

        for (int i = 0; i < indexes.Count; i++)
        {
            int index = i;

            Vector3 pos = LevelController.instance.conveyorBelt.cachedPathPoints[0];

            FoodOnConveyorBelt.instance.SetParent(slots[indexes[index]].food);

            float mul = 1f / indexes.Count;

            float time = index * 0.15f + Mathf.Clamp(mul, mul, 0.25f);

            slots[indexes[index]].food.transform.DOJump(pos, 15, 1, time).OnComplete(delegate
            {
                AudioController.instance.PlaySoundNVibrate(AudioController.instance.onDropConveyorBelt, 25);

                FoodOnConveyorBelt.instance.AddFood(slots[indexes[index]].food);
            });
            slots[indexes[index]].RemoveFood();
        }
    }

    int GetIndexByCol(GameObject col)
    {
        for (int i = 0; i < slots.Count; i++)
        {
            if (slots[i].col.gameObject == col) return i;
        }

        return -1;
    }

    int GetSlotEmpty()
    {
        for (int i = 0; i < slots.Count; i++)
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
