using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using System;

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
        Playing, Win, Lose, Pause
    }

    private void Awake()
    {
        instance = this;

        TextAsset textAsset = Resources.Load<TextAsset>(GameManager.instance.Level.ToString());

        levelData = JsonConvert.DeserializeObject<LevelData>(textAsset.text);

        conveyorBelt = GetComponentInChildren<ConveyorBelt>();
        deliveryController = GetComponentInChildren<DeliveryController>();
        foodTrays = GetComponentInChildren<FoodTrays>();

        deliveryController.GenerateDeliveryLines(levelData.shipperDatas);
        foodTrays.GenerateFoodTrays(levelData.trayDatas);
        conveyorBelt.GenerateConveyorBelt(levelData.conveyorBeltDatas, levelData.barData);

        moveCount = levelData.moveAmount;
        bool isActiveMove = moveCount > 0;

        UIController.instance.uIInGame.ActiveMove(isActiveMove);

        if (isActiveMove) UIController.instance.uIInGame.UpdateMove(moveCount.ToString());

        UIController.instance.uIInGame.UpdateLevel();
        //UIController.instance.uIInGame.CheckBoosterTut();
        UIController.instance.uIInGame.uIBooster.CheckAmoutBooster();
        UIController.instance.uIPanelRemoveAds.CheckToDisplay();

        AudioController.instance.StartOnFire();
    }

    private void Start()
    {
        UIController.instance.uITutorial.ShowTut();
    }

    public void MinusMove()
    {
        moveCount--;

        foodTrays.MinusFreeze();

        if (!UIController.instance.uIInGame.IsActiveMove()) return;

        if (moveCount == -1)
        {
            UIController.instance.Lose();
        }
        else
        {
            UIController.instance.uIInGame.UpdateMove(moveCount.ToString());
        }
    }

    public void BoosterAddMove()
    {
        moveCount += 10;

        UIController.instance.uIInGame.UpdateMove(moveCount.ToString());

        UIController.instance.uIInGame.uIBooster.HideBoosterTut();
    }

    public void BoosterDisturbance()
    {
        foodTrays.BossterDisturbance();

        UIController.instance.uIInGame.uIBooster.HideBoosterTut();
    }

    public void BoosterMagnet()
    {
        foodTrays.MinusFreeze();

        deliveryController.BossterMagnet();

        UIController.instance.uIInGame.uIBooster.HideBoosterTut();
    }

    public void BoosterBreakIce()
    {
        gameState = GameState.Pause;

        GameController.instance.PlayHammer();

        UIController.instance.uIInGame.uIBooster.HideBoosterTut();
    }

    public void AfterHammer()
    {
        gameState = GameState.Playing;

        GameController.instance.ShakeCamera();

        foodTrays.BoosterBreakFreeze();
    }
}
