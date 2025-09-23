using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class UISettingOption : MonoBehaviour
{
    public UISetting.TypeSetting type;
    public GameObject buttonActive;

    public void SwitchStateHandle(bool isActive)
    {
        buttonActive.SetActive(isActive);
    }

    public void OnClick()
    {
        UIController.instance.uISetting.ChangeSettingOption(this, !buttonActive.activeSelf);
    }
}
