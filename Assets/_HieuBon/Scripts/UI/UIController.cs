using UnityEngine;

public class UIController : MonoBehaviour
{
    public static UIController instance;

    public GameObject panelWin;
    public GameObject panelLose;

    [HideInInspector]
    public UIInGame uIInGame;

    private void Awake()
    {
        instance = this;

        uIInGame = GetComponent<UIInGame>();
    }

    public void ShowPanelWin()
    {
        panelWin.SetActive(true);
    }
    
    public void HidePanelWin()
    {
        panelWin.SetActive(false);

        GameController.instance.LoadLevel();
    }

    public void ShowPaneLose()
    {
        panelLose.SetActive(true);
    }

    public void HidePaneLose()
    {
        panelLose.SetActive(false);
        GameController.instance.LoadLevel();
    }
}
