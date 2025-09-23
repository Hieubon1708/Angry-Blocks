using UnityEngine;

[System.Serializable]
public class LevelData
{
    public ShipperData[] shipperDatas;
    public TrayData[] trayDatas;
    public ConveyorBeltData[] conveyorBeltDatas;
    public BarData barData;
    public int moveAmount;

    public LevelData(ShipperData[] shipperDatas, TrayData[] trayDatas, ConveyorBeltData[] conveyorBeltDatas, BarData barData)
    {
        this.shipperDatas = shipperDatas;
        this.trayDatas = trayDatas;
        this.conveyorBeltDatas = conveyorBeltDatas;
        this.barData = barData;
    }

    public enum ConveyorBeltType
    {
        Corner, Inout, Straight
    }
}

[System.Serializable]
public class ShipperData
{
    public Vector3 pivot;
    public Vector3 position;
    public Quaternion direction;
    public int[] foodType;

    public Vector3[] endPoints;
    public Vector3[] startPoints;

    public ShipperData(Vector3 pivot, Vector3 position, Quaternion direction, int[] foodType, Vector3[] endPoints, Vector3[] startPoints)
    {
        this.pivot = pivot;
        this.position = position;
        this.direction = direction;
        this.foodType = foodType;
        this.endPoints = endPoints;
        this.startPoints = startPoints;
    }
}

[System.Serializable]
public class TrayData
{
    public Vector3 position;
    public Quaternion direction;
    public int[] foodType = new int[4];
    public int amountFreeze;

    public TrayData(Vector3 position, Quaternion direction, int[] foodType, int amountFreeze)
    {
        this.position = position;
        this.direction = direction;
        this.foodType = foodType;
        this.amountFreeze = amountFreeze;
    }
}

[System.Serializable]
public class ConveyorBeltData
{
    public LevelData.ConveyorBeltType type;
    public Vector3 position;
    public Quaternion direction;

    public ConveyorBeltData(LevelData.ConveyorBeltType type, Vector3 position, Quaternion direction)
    {
        this.type = type;
        this.position = position;
        this.direction = direction;
    }
}

[System.Serializable]
public class BarData
{
    public Vector3 position;
    public Quaternion direction;

    public BarData(Vector3 position, Quaternion direction)
    {
        this.position = position;
        this.direction = direction;
    }
}
