using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIBooster : MonoBehaviour
{
    public GameObject boosterMagnet;
    public GameObject boosterDisturbance;
    public GameObject boosterAddMove;
    public GameObject boosterBreakIce;

    Animation boosterAni;

    public GameObject note;
    public TextMeshProUGUI txtNote;

    public int[] boosterPrices;

    int freezeLevel = 100;

    UIBoosterButton[] uIBoosterButtons;

    string[] txtNotes = new string[]
    {
        "Use magnet to attract food for shippers, including trays below.",
        "When out of choices, use shuffle to swap the positions of the trays.",
        "When you are running out of moves, use this to get extra.",
        "Use a hammer to break all the ice layers above."
    };

    public int AmountMagnet
    {
        get
        {
            return PlayerPrefs.GetInt("AmountMagnet", 1);
        }
        set
        {
            PlayerPrefs.SetInt("AmountMagnet", value);
        }
    }

    public int AmountShuffle
    {
        get
        {
            return PlayerPrefs.GetInt("AmountShuffle", 1);
        }
        set
        {
            PlayerPrefs.SetInt("AmountShuffle", value);
        }
    }

    public int AmountMove
    {
        get
        {
            return PlayerPrefs.GetInt("AmountMove", 1);
        }
        set
        {
            PlayerPrefs.SetInt("AmountMove", value);
        }
    }

    public int AmountHammer
    {
        get
        {
            return PlayerPrefs.GetInt("AmountHammer", 1);
        }
        set
        {
            PlayerPrefs.SetInt("AmountHammer", value);
        }
    }

    int GetAmountBooster(int index)
    {
        switch (index)
        {
            case 0: return AmountMagnet;
            case 1: return AmountShuffle;
            case 2: return AmountMove;
            case 3: return AmountHammer;
        }
        return -1;
    }

    private void Awake()
    {
        boosterAni = GetComponent<Animation>();

        uIBoosterButtons = GetComponentsInChildren<UIBoosterButton>();

        for (int i = 0; i < uIBoosterButtons.Length; i++)
        {
            uIBoosterButtons[i].SetGold(boosterPrices[i]);
        }
    }

    public void CheckAmoutBooster()
    {
        for (int i = 0; i < uIBoosterButtons.Length; i++)
        {
            uIBoosterButtons[i].CheckButton(GetAmountBooster(i), IsEnough(i));
        }
    }

    public bool IsEnough(int index)
    {
        return GameManager.instance.Gold >= boosterPrices[index];
    }

    public void CheckBoosterTut()
    {
        int level = GameManager.instance.CurrentLevel;

        if (level == 1) ActiveBooster();
        else if (level == 2)
        {
            txtNote.text = txtNotes[0];

            ActiveBooster(true);

            LevelController.instance.gameState = LevelController.GameState.Pause;

            note.SetActive(true);

            DOVirtual.DelayedCall(0.01f, delegate
            {
                UIController.instance.uITutorial.ShowBoosterTut(UIController.instance.uICamera.WorldToScreenPoint(boosterMagnet.transform.position));
            });
        }
        else if (level == 3)
        {
            txtNote.text = txtNotes[1];

            ActiveBooster(true, true);

            LevelController.instance.gameState = LevelController.GameState.Pause;

            note.SetActive(true);

            DOVirtual.DelayedCall(0.01f, delegate
            {
                UIController.instance.uITutorial.ShowBoosterTut(UIController.instance.uICamera.WorldToScreenPoint(boosterDisturbance.transform.position));
            });
        }
        else if (level == 4)
        {
            txtNote.text = txtNotes[2];

            ActiveBooster(true, true, true);

            LevelController.instance.gameState = LevelController.GameState.Pause;

            note.SetActive(true);

            DOVirtual.DelayedCall(0.01f, delegate
            {
                UIController.instance.uITutorial.ShowBoosterTut(UIController.instance.uICamera.WorldToScreenPoint(boosterAddMove.transform.position));
            });
        }
        else if (level == freezeLevel)
        {
            txtNote.text = txtNotes[3];

            ActiveBooster(true, true, true, true);

            LevelController.instance.gameState = LevelController.GameState.Pause;

            note.SetActive(true);

            DOVirtual.DelayedCall(0.01f, delegate
            {
                UIController.instance.uITutorial.ShowBoosterTut(UIController.instance.uICamera.WorldToScreenPoint(boosterBreakIce.transform.position));
            });
        }
        else
        {
            boosterAni.Play();

            bool isFreeze = GameController.instance.GetComponentInChildren<IceTray>() != null;

            ActiveBooster(true, true, true, isFreeze);
        }
    }

    public void HideBoosterTut()
    {
        int level = GameManager.instance.Level;

        if (level != 2 && level != 3 && level != 4 && level != freezeLevel) return;

        LevelController.instance.gameState = LevelController.GameState.Playing;

        note.SetActive(false);

        UIController.instance.uITutorial.HideBoosterTut();
    }

    void ActiveBooster(bool isActiveMagnet = false, bool isActiveDisturbance = false, bool isActiveAddMove = false, bool isActiveBreakIce = false)
    {
        boosterMagnet.SetActive(isActiveMagnet);
        boosterDisturbance.SetActive(isActiveDisturbance);
        boosterAddMove.SetActive(isActiveAddMove);
        boosterBreakIce.SetActive(isActiveBreakIce);
    }
}
