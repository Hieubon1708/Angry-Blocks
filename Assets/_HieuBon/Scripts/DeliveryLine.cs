using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static GameController;

public class DeliveryLine : MonoBehaviour
{
    public GameObject preDeliveryMan;

    [HideInInspector]
    public Vector3[] startPoints;
    [HideInInspector]
    public Vector3[] endPoints;

    [HideInInspector]
    public List<DeliveryMan> deliveryMen = new List<DeliveryMan>();

    int indexMan;

    [HideInInspector]
    public int indexManDelivered;

    public void GenerateDeliveryMan(int[] foodTypes, Vector3[] startPoints, Vector3[] endPoints)
    {
        this.startPoints = startPoints;
        this.endPoints = endPoints;

        for (int i = 0; i < foodTypes.Length; i++)
        {
            GameObject e = Instantiate(preDeliveryMan, transform);

            DeliveryMan deliveryMan = e.GetComponent<DeliveryMan>();

            deliveryMan.SetFoodType(GameController.instance.GetFoodTypeByIndex(foodTypes[i]));

            deliveryMen.Add(deliveryMan);

            e.SetActive(false);
        }

        NextDeliveryMan();
    }

    public void NextDeliveryMan()
    {
        if (indexMan == deliveryMen.Count) return;

        StartCoroutine(deliveryMen[indexMan].MoveIn(startPoints));
        indexMan++;
    }

    public bool IsOk()
    {
        return indexManDelivered == deliveryMen.Count;
    }
}
