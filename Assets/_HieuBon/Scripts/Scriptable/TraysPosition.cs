using UnityEngine;

[CreateAssetMenu(fileName = "TraysPosition", menuName = "Scriptable Objects/TraysPosition")]
public class TraysPosition : ScriptableObject
{
    public TrayPosition[] trayAmount;
}

[System.Serializable]
public class TrayPosition
{
    public Vector3[] pos;
}