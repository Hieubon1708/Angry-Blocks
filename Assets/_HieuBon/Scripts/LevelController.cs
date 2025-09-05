using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

public class LevelController : MonoBehaviour
{
    public static LevelController instance;

    [HideInInspector]
    public ConveyorBelt conveyorBelt;
    [HideInInspector]
    public FoodTrays foodTrays;
    [HideInInspector]
    public DeliveryController deliveryController;
    [HideInInspector]
    public GameState gameState;
    [HideInInspector]
    public LevelData levelData;


    public int moveCount;

    public enum GameState
    {
        Playing, Win, Lose
    }

    private void Awake()
    {
        instance = this;

        TextAsset textAsset = Resources.Load<TextAsset>("1");

        levelData = JsonConvert.DeserializeObject<LevelData>(textAsset.text);

        conveyorBelt = GetComponentInChildren<ConveyorBelt>();
        deliveryController = GetComponentInChildren<DeliveryController>();
        foodTrays = GetComponentInChildren<FoodTrays>();

        deliveryController.GenerateDeliveryLines(levelData.shipperDatas);
        foodTrays.GenerateFoodTrays(levelData.trayDatas);
        conveyorBelt.GenerateConveyorBelt(levelData.conveyorBeltDatas, levelData.barData);

        moveCount = levelData.moveAmount;

         UIController.instance.uIInGame.UpdateMove(moveCount);
    }

    public void SubtractMove()
    {
        moveCount--;

        UIController.instance.uIInGame.UpdateMove(moveCount);

        if (moveCount == 0)
        {
            UIController.instance.ShowPaneLose();
        }
    }
}
