using System.Collections.Generic;
using System;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public static GameController instance;

    public Camera cameraMain;

    GameObject levelObj;

    public GameObject[] preFood;

    public string[] shipperBoxBodyHex;
    public string[] shipperBoxUpperHex;

    public int Level
    {
        get
        {
            return PlayerPrefs.GetInt("Level", 1);
        }
        set
        {
            PlayerPrefs.SetInt("Level", value);
        }
    }

    private void Awake()
    {
        instance = this;

        LoadLevel();
    }

    public enum FoodType
    {
        S1, S2, S3, S4, S5, S6, S7, S8, S9, S10, S11, S12, S13, None
    }

    public void LoadLevel()
    {
        if (levelObj != null) Destroy(levelObj);

        levelObj = Instantiate(Resources.Load<GameObject>("GamePlay"), transform);
    }

    public GameObject GetPrefab(int foodType)
    {
        return preFood[foodType];
    }

    public FoodType GetFoodTypeByIndex(int index)
    {
        FoodType[] foodTypes = (GameController.FoodType[])Enum.GetValues(typeof(GameController.FoodType));

        for (int i = 0; i < foodTypes.Length; i++)
        {
            if (i == index) return foodTypes[i];
        }

        return FoodType.None;
    }

    public Color GetBoxBodyColor(FoodType foodType)
    {
        Color color = Color.white;

        ColorUtility.TryParseHtmlString("#" + shipperBoxBodyHex[(int)foodType], out color);

        return color;
    }

    public Color GetBoxUpperColor(FoodType foodType)
    {
        Color color = Color.white;

        ColorUtility.TryParseHtmlString("#" + shipperBoxUpperHex[(int)foodType], out color);

        return color;
    }
}

