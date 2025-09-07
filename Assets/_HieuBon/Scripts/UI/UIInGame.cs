using TMPro;
using UnityEngine;

public class UIInGame : MonoBehaviour
{
    private void Awake()
    {
    }
    public TextMeshProUGUI txtMoveCount;

    public void UpdateMove(string text)
    {
        txtMoveCount.text = text;
    }
}
