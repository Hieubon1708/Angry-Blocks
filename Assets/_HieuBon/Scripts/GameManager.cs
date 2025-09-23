using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public int gold;
    public int level;

    private void Awake()
    {
        instance = this;

#if UNITY_EDITOR

        if (level == -1) return;

        PlayerPrefs.DeleteAll();

        Gold = gold;
        Level = level;
        CurrentLevel = level;
#endif
    }

    private void Start()
    {
        UIController.instance.UpdateGold();

        ACEPlay.Bridge.BridgeController.instance.ShowBanner();
    }

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
    
    public int CurrentLevel
    {
        get
        {
            return PlayerPrefs.GetInt("CurrentLevel", 1);
        }
        set
        {
            PlayerPrefs.SetInt("CurrentLevel", value);
        }
    }

    public int Gold
    {
        get
        {
            return PlayerPrefs.GetInt("Gold", 0);
        }
        set
        {
            PlayerPrefs.SetInt("Gold", value);
        }
    }

    public bool IsAtiveSound
    {
        get
        {
            return PlayerPrefs.GetInt("Sound", 1) == 1;
        }
        set
        {
            AudioController.instance.PlayMusic(!value);

            PlayerPrefs.SetInt("Sound", value ? 1 : 0);
        }
    }

    public bool IsActiveVibrate
    {
        get
        {
            return PlayerPrefs.GetInt("Vibrate", 1) == 1;
        }
        set
        {
            PlayerPrefs.SetInt("Vibrate", value ? 1 : 0);
        }
    }
}
