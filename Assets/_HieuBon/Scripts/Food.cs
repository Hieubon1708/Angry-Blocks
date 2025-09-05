using DG.Tweening;
using UnityEngine;

public class Food : MonoBehaviour
{
    public GameController.FoodType foodType;

    [HideInInspector]
    public MeshRenderer meshRenderer;

    public FoodPoint foodPoint;

    private void Awake()
    {
        meshRenderer = GetComponentInChildren<MeshRenderer>();
    }

    public void Blur()
    {
        meshRenderer.material.color = new Color(0.5f, 0.5f, 0.5f);
        meshRenderer.material.SetColor("_EmissionColor", new Color(0.75f, 0.75f, 0.75f));
    }

    private void OnDestroy()
    {
        transform.DOKill();
    }
}
