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

    public void GenerateDeliveryMan(int[] foodTypes, Vector3[] startPoints, Vector3[] endPoints, Vector3 pivot)
    {
        this.startPoints = startPoints;
        this.endPoints = endPoints;

        for (int i = 0; i < foodTypes.Length; i++)
        {
            GameObject e = Instantiate(preDeliveryMan, transform);

            e.transform.position = startPoints[0];
            e.transform.rotation = Quaternion.LookRotation(startPoints[1] - startPoints[0]);

            DeliveryMan deliveryMan = e.GetComponent<DeliveryMan>();

            deliveryMan.SetFoodTypeAndPivot(GameController.instance.GetFoodTypeByIndex(foodTypes[i]), pivot);

            deliveryMen.Add(deliveryMan);

            deliveryMan.foodContainer.gameObject.SetActive(false);
        }

        NextDeliveryMan(true);
    }

    public void NextDeliveryMan(bool isStart = false)
    {
        if (indexMan == deliveryMen.Count) return;

        StartCoroutine(deliveryMen[indexMan].MoveIn(isStart, startPoints, indexMan < deliveryMen.Count - 1 ? deliveryMen[indexMan + 1] : null));

        indexMan++;
    }

    public bool IsOk()
    {
        return indexManDelivered == deliveryMen.Count;
    }
}
