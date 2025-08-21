using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Tool : MonoBehaviour
{
    public Button deleteButton;

    public GameObject preBox;

    public TMP_InputField rowText;
    public TMP_InputField colText;

    public GridLayoutGroup gridLayoutGroup;

    public void DeleteActive()
    {
        ColorBlock cb = deleteButton.colors;

        if (cb.selectedColor == Color.red)
        {
            cb.selectedColor = Color.white;
        }
        else
        {
            cb.selectedColor = Color.red;
        }

        deleteButton.colors = cb;
    }

    public void UpdateRowCol()
    {
        int row = 0, col = 0;

        if (!int.TryParse(rowText.text, out row) || !int.TryParse(colText.text, out col)) return;

        row = Mathf.Clamp(row, 0, 20);
        col = Mathf.Clamp(col, 0, 20);

        for (int i = 0; i < gridLayoutGroup.transform.childCount; i++)
        {
            Destroy(gridLayoutGroup.transform.GetChild(i).gameObject);

        }

        float cellSize = (Screen.width - ((col - 1)) * 10) / col;

        gridLayoutGroup.cellSize = new Vector2(cellSize, cellSize);
        gridLayoutGroup.constraintCount = col;

        for (int i = 0; i < col * row; i++)
        {
            Instantiate(preBox, gridLayoutGroup.transform);
        }
    }
}
