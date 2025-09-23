using DG.Tweening;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class UIInGame : MonoBehaviour
{
    Animation aniMove;

    public TextMeshProUGUI txtMoveCount;
    public TextMeshProUGUI txtLevel;

    [HideInInspector]
    public UIBooster uIBooster;

    [Header("Panel Win")]
    public ParticleSystem vfxWin;
    public int totalCoinEarn;
    public TextMeshProUGUI txtX;
    public TextMeshProUGUI txtTotalEarn;
    [HideInInspector]
    public bool isReward;

    private void Awake()
    {
        aniMove = txtMoveCount.GetComponent<Animation>();

        uIBooster = GetComponentInChildren<UIBooster>(true);
    }

    public void Win()
    {
        txtTotalEarn.text = totalCoinEarn + "<sprite=0>";
        txtX.text = totalCoinEarn * 3 + "<sprite=0>";
    }

    public void PlayVfxWin()
    {
        vfxWin.gameObject.SetActive(true);

        vfxWin.Play();
    }

    public void StopVfxWin()
    {
        vfxWin.gameObject.SetActive(false);
    }

    public void CheckBoosterTut()
    {
        uIBooster.CheckBoosterTut();
    }

    public void UpdateLevel()
    {
        txtLevel.text = "Level " + GameManager.instance.CurrentLevel;
    }

    public void UpdateMove(string text)
    {
        txtMoveCount.text = "Move " + text;

        ScaleTextMove();
    }

    public void ActiveMove(bool isActive)
    {
        aniMove.transform.parent.gameObject.SetActive(isActive);
    }

    public bool IsActiveMove()
    {
        return aniMove.transform.parent.gameObject.activeSelf;
    }

    public void ScaleTextMove()
    {
        aniMove.Play();
    }

    public void NextLevel(Action onLenghtDone, bool isX)
    {
        if (isReward) return;

        isReward = true;

        int gold = isX ? totalCoinEarn * 3 : totalCoinEarn;

        GameManager.instance.Gold += gold;

        UIController.instance.GoldFly(isX ? txtX.transform.position : txtTotalEarn.transform.position, gold, delegate
        {
            DOVirtual.DelayedCall(0.75f, delegate
            {
                if (onLenghtDone != null) onLenghtDone.Invoke();
            });
        });
    }

    public void BossterAddMove(int index)
    {
        AudioController.instance.PlaySoundNVibrate(AudioController.instance.button, 50);
        AudioController.instance.PlaySoundNVibrate(AudioController.instance.addMove);

        if (uIBooster.AmountMove > 0)
        {
            uIBooster.AmountMove--;

            LevelController.instance.BoosterAddMove();
            uIBooster.CheckAmoutBooster();
        }
        else if (uIBooster.IsEnough(index))
        {
            GameManager.instance.Gold -= uIBooster.boosterPrices[index];

            UIController.instance.MinusGold(uIBooster.boosterPrices[index]);

            LevelController.instance.BoosterAddMove();
            uIBooster.CheckAmoutBooster();
        }
        else
        {
            UnityEvent eReward = new UnityEvent();
            eReward.AddListener(() =>
            {
                LevelController.instance.BoosterAddMove();
            });
            ACEPlay.Bridge.BridgeController.instance.ShowRewarded("booster", eReward, null);
        }
    }

    public void BossterDisturbance(int index)
    {
        AudioController.instance.PlaySoundNVibrate(AudioController.instance.button, 50);
        AudioController.instance.PlaySoundNVibrate(AudioController.instance.shuffle);

        if (uIBooster.AmountShuffle > 0)
        {
            uIBooster.AmountShuffle--;

            LevelController.instance.BoosterDisturbance();
            uIBooster.CheckAmoutBooster();
        }
        else if (uIBooster.IsEnough(index))
        {
            GameManager.instance.Gold -= uIBooster.boosterPrices[index];

            UIController.instance.MinusGold(uIBooster.boosterPrices[index]);

            LevelController.instance.BoosterDisturbance();
            uIBooster.CheckAmoutBooster();
        }
        else
        {
            UnityEvent eReward = new UnityEvent();
            eReward.AddListener(() =>
            {
                LevelController.instance.BoosterDisturbance();
            });
            ACEPlay.Bridge.BridgeController.instance.ShowRewarded("booster", eReward, null);
        }
    }

    public void BossterMagnet(int index)
    {
        AudioController.instance.PlaySoundNVibrate(AudioController.instance.button, 50);
        AudioController.instance.PlaySoundNVibrate(AudioController.instance.magnet);

        if (uIBooster.AmountMagnet > 0)
        {
            uIBooster.AmountMagnet--;

            LevelController.instance.BoosterMagnet();
            uIBooster.CheckAmoutBooster();
        }
        else if(uIBooster.IsEnough(index))
        {
            GameManager.instance.Gold -= uIBooster.boosterPrices[index];

            UIController.instance.MinusGold(uIBooster.boosterPrices[index]);

            LevelController.instance.BoosterMagnet();
            uIBooster.CheckAmoutBooster();
        }
        else
        {
            UnityEvent eReward = new UnityEvent();
            eReward.AddListener(() =>
            {
                LevelController.instance.BoosterMagnet();
            });
            ACEPlay.Bridge.BridgeController.instance.ShowRewarded("booster", eReward, null);
        }
    }

    public void BossterBreakIce(int index)
    {
        AudioController.instance.PlaySoundNVibrate(AudioController.instance.button, 50);

        if (uIBooster.AmountHammer > 0)
        {
            uIBooster.AmountHammer--;

            LevelController.instance.BoosterBreakIce();
            uIBooster.CheckAmoutBooster();
        }
        else if (uIBooster.IsEnough(index))
        {
            GameManager.instance.Gold -= uIBooster.boosterPrices[index];

            UIController.instance.MinusGold(uIBooster.boosterPrices[index]);

            LevelController.instance.BoosterBreakIce();
            uIBooster.CheckAmoutBooster();
        }
        else
        {
            UnityEvent eReward = new UnityEvent();
            eReward.AddListener(() =>
            {
                LevelController.instance.BoosterBreakIce();
            });
            ACEPlay.Bridge.BridgeController.instance.ShowRewarded("booster", eReward, null);
        }
    }
}
