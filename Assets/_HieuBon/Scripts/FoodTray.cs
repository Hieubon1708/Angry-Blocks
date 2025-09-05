using DG.Tweening;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FoodTray : MonoBehaviour
{
    [HideInInspector]
    public List<Food> foods = new List<Food>();
    [HideInInspector]
    public MeshRenderer meshRenderer;
    [HideInInspector]
    public BoxCollider boxCollider;

    Animator animator;

    LayerMask foodTrayMask;

    public Transform[] points;

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();

        foodTrayMask = LayerMask.GetMask("FoodTray");

        boxCollider = GetComponent<BoxCollider>();

        meshRenderer = GetComponentInChildren<MeshRenderer>();
    }

    public void Blur()
    {
        meshRenderer.material.color = new Color(0.5f, 0.5f, 0.5f);
        meshRenderer.material.SetColor("_EmissionColor", new Color(0.75f, 0.75f, 0.75f));

        foreach (var e in foods)
        {
            e.Blur();
        }
    }

    public void GenerateFood(int[] foodTypes)
    {
        for (int i = 0; i < foodTypes.Length; i++)
        {
            GameObject pre = GameController.instance.GetPrefab(foodTypes[i]);

            Transform container = transform.GetChild(0);

            GameObject e = Instantiate(pre, points[i].position, points[i].rotation, container);

            Food food = e.GetComponent<Food>();

            food.foodType = GameController.instance.GetFoodTypeByIndex(foodTypes[i]);

            foods.Add(food);
        }
    }

    public bool IsHighestLayer()
    {
        return meshRenderer.material.color == Color.white;
    }

    public void Holding()
    {
        animator.SetTrigger("Holding");
    }

    public void Toss()
    {
        boxCollider.enabled = false;

        animator.SetTrigger("Toss");
    }

    public void AfterToss()
    {
        Vector3 startPoint = LevelController.instance.conveyorBelt.cachedPathPoints[0];

        for (int i = 0; i < foods.Count - 1; i++)
        {
            float distance1 = Vector3.Distance(startPoint, foods[i].transform.position);

            for (int j = i + 1; j < foods.Count; j++)
            {
                float distance2 = Vector3.Distance(startPoint, foods[j].transform.position);

                if (distance1 > distance2)
                {
                    Food temp = foods[j];
                    foods[j] = foods[i];
                    foods[i] = temp;
                }
            }
        }

        for (int i = 0; i < foods.Count; i++)
        {
            int index = i;

            FoodOnConveyorBelt.instance.SetParent(foods[index]);

            float time = index * 0.15f + 0.25f;

            foods[index].meshRenderer.transform.DOScale(0.5f, time);

            foods[index].transform.DOJump(startPoint, 25, 1, time).OnComplete(delegate
            {
                FoodOnConveyorBelt.instance.AddFood(foods[index]);
            });
        }
    }

    Collider[] result = new Collider[4];

    public void CheckSphereCast()
    {
        int amount = Physics.OverlapSphereNonAlloc(transform.position + transform.up * 0.5f, 0.35f, result, foodTrayMask);

        if (amount == 1)
        {
            meshRenderer.material.color = Color.white;
            meshRenderer.material.SetColor("_EmissionColor", Color.white);

            foreach (var e in foods)
            {
                e.meshRenderer.material.color = Color.white;
                e.meshRenderer.material.SetColor("_EmissionColor", Color.white);
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;

        Debug.DrawLine(transform.position, transform.position + transform.up * 0.5f, Color.yellow);
        Gizmos.DrawWireSphere(transform.position + transform.up * 0.5f, 0.35f);
    }
}
