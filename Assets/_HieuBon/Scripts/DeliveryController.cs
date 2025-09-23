using System.Collections.Generic;
using UnityEngine;

public class DeliveryController : MonoBehaviour
{
    public GameObject preDeliveryLine;

    DeliveryLine[] deliveryLines;

    public void GenerateDeliveryLines(ShipperData[] shipperDatas)
    {
        deliveryLines = new DeliveryLine[shipperDatas.Length];      

        for (int i = 0; i < shipperDatas.Length; i++)
        {
            Vector3 nextEndPoint = shipperDatas[i].endPoints.Length == 1 ? shipperDatas[i].position : shipperDatas[i].endPoints[1];

            Vector3 endDir = shipperDatas[i].endPoints[0] - nextEndPoint;

            shipperDatas[i].endPoints[0] += endDir.normalized * 10;

            deliveryLines[i] = Instantiate(preDeliveryLine, shipperDatas[i].position, shipperDatas[i].direction, transform).GetComponent<DeliveryLine>();

            deliveryLines[i].GenerateDeliveryMan(shipperDatas[i].foodType, shipperDatas[i].startPoints, shipperDatas[i].endPoints, shipperDatas[i].pivot);
        }
    }

    public void IsWin()
    {
        bool isWin = true;

        for (int i = 0; i < deliveryLines.Length; i++)
        {
            if (!deliveryLines[i].IsOk()) isWin = false;
        }

        if (isWin)
        {
            UIController.instance.Win();
        }
    }

    public void BossterMagnet()
    {
        List<FoodTray> f = LevelController.instance.foodTrays.GetSortFoodTrays();

        for (int i = 0; i < deliveryLines.Length; i++)
        {
            if(deliveryLines[i].indexManDelivered < deliveryLines[i].deliveryMen.Count)
            {
                DeliveryMan d = deliveryLines[i].deliveryMen[deliveryLines[i].indexManDelivered];

                d.Magnet(f);
            }
        }
    }
}
