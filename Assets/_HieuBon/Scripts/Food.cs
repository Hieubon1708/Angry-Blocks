using DG.Tweening;
using UnityEngine;

public class Food : MonoBehaviour
{
    public GameController.FoodType foodType;

    [HideInInspector]
    public MeshRenderer meshRenderer;

    public FoodPoint foodPoint;

    Animation ani;

    private void Awake()
    {
        meshRenderer = GetComponentInChildren<MeshRenderer>();

        ani = GetComponent<Animation>();
    }

    public void AniSale()
    {
        //ani.Play();
    }

    private void OnDestroy()
    {
        transform.DOKill();
    }
}
