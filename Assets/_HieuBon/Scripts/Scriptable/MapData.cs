using UnityEngine;

[CreateAssetMenu(fileName = "MapData", menuName = "Scriptable Objects/MapData")]
public class MapData : ScriptableObject
{
    public FoodArea foodArea;
    public ShipperArea shipperArea;
}

[System.Serializable]
public class FoodArea
{
    public FoodRow[] foodRows;
}

[System.Serializable]
public class FoodRow
{
    GameController.FoodType foodType;
}

[System.Serializable]
public class ShipperArea
{
    public ShipperLine[] shipperLines;
}

[System.Serializable]
public class ShipperLine
{
    GameController.FoodType foodDelivery;
}
