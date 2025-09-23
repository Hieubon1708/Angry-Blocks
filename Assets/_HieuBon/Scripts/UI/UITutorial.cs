using DG.Tweening;
using System;
using UnityEngine;

public class UITutorial : MonoBehaviour
{
    FoodTray[] foodTrays;
    RectTransform rect;
    Tween delayCall;

    public RectTransform canvas;

    private void Awake()
    {
        rect = GetComponent<RectTransform>();
    }

    public void ShowTut()
    {
        if (GameManager.instance.CurrentLevel != 1) return;

        DOVirtual.DelayedCall(0.1f, delegate
        {
            if (foodTrays == null) foodTrays = LevelController.instance.foodTrays.foodTrays;

            int index = -1;

            for (int i = 0; i < foodTrays.Length; i++)
            {
                if (foodTrays[i].boxCollider != null && foodTrays[i].boxCollider.enabled && foodTrays[i].meshRenderer.material.color == Color.white)
                {
                    index = i; break;
                }
            }

            if (index == -1) return;

            gameObject.SetActive(true);

            Vector3 pos = foodTrays[index].transform.position;

            Vector3 screenPos = GameController.instance.cameraMain.WorldToScreenPoint(pos);

            screenPos.z = transform.position.z;

            Vector2 localPoint;

            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas, screenPos, UIController.instance.uICamera, out localPoint))
            {
                rect.anchoredPosition = localPoint;
            }
        });
    }

    public void ShowBoosterTut(Vector2 pos)
    {
        pos.y -= 50;
        pos.x += 50;

        gameObject.SetActive(true);

        Vector2 localPoint;

        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas, pos, UIController.instance.uICamera, out localPoint))
        {
            rect.anchoredPosition = localPoint;
        }
    }

    public void HideBoosterTut()
    {
        gameObject.SetActive(false);
    }

    public void HideTut()
    {
        if (GameManager.instance.CurrentLevel != 1) return;

        gameObject.SetActive(false);

        delayCall.Kill();

        delayCall = DOVirtual.DelayedCall(2.5f, delegate
        {
            ShowTut();
        });
    }
}
