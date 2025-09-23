using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UIFade : MonoBehaviour
{
    public Image layerFade;

    private void Start()
    {
        layerFade.gameObject.SetActive(true);

        FadeOut(null);
    }

    public void FadeIn(Action action)
    {
        layerFade.gameObject.SetActive(true);

        layerFade.DOFade(1f, 0.5f).SetUpdate(true).OnComplete(delegate
        {
            if (action != null) action.Invoke();
        });
    }

    public void FadeOut(Action action)
    {
        layerFade.DOFade(0f, 0.5f).SetUpdate(true).OnComplete(delegate
        {
            if (action != null) action.Invoke();

            layerFade.gameObject.SetActive(false);
        });
    }
}
