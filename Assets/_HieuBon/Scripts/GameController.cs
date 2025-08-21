using UnityEngine;

public class GameController : MonoBehaviour
{
    public static GameController instance;

    public TraysPosition traysPosition;

    public GameObject preConveyorBelt;

    GameObject conveyorBeltContainer;

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

        //LoadLevel();
    }

    public enum FoodType
    {
        T1, T2, T3, T4, T5, None
    }

    public void LoadLevel()
    {
        if (conveyorBeltContainer != null) Destroy(conveyorBeltContainer);

        conveyorBeltContainer = Instantiate(preConveyorBelt, transform);
    }
}

