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
            deliveryLines[i] = Instantiate(preDeliveryLine, shipperDatas[i].position, shipperDatas[i].direction, transform).GetComponent<DeliveryLine>();

            deliveryLines[i].GenerateDeliveryMan(shipperDatas[i].foodType, shipperDatas[i].startPoints, shipperDatas[i].endPoints);
        }
    }

    public void IsWin()
    {
        bool isWin = true;
        
        for (int i = 0; i < deliveryLines.Length; i++)
        {
            if (!deliveryLines[i].IsOk()) isWin = false;
        }

        if(isWin)
        {
            UIController.instance.ShowPanelWin();

            LevelController.instance.gameState = LevelController.GameState.Win;
        }
    }
}
