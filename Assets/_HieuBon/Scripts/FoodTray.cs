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

    private void Start()
    {
        CheckSphereCast();
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
        Vector3 actualBoxCenter = transform.position + transform.up * 3.5f;

        int numColliders = Physics.OverlapBoxNonAlloc(actualBoxCenter, boxHalfExtents, result, transform.rotation, foodTrayMask);

        Debug.Log(numColliders);

        Color targetColor = numColliders == 0 ? Color.white : new Color(0.5f, 0.5f, 0.5f);

        if (numColliders == 0 && meshRenderer.material.color == targetColor) return;

        meshRenderer.material.color = targetColor;
        meshRenderer.material.SetColor("_EmissionColor", targetColor);

        foreach (var e in foods)
        {
            e.meshRenderer.material.color = targetColor;
            e.meshRenderer.material.SetColor("_EmissionColor", targetColor);
        }
    }

    Vector3 boxHalfExtents = new Vector3(7f, 1f, 2f);

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        Vector3 actualBoxCenter = transform.position + transform.up * 3.5f;

        Matrix4x4 originalMatrix = Gizmos.matrix;

        Gizmos.matrix = Matrix4x4.TRS(actualBoxCenter, transform.rotation, Vector3.one);

        Gizmos.DrawWireCube(Vector3.zero, boxHalfExtents * 2);

        Gizmos.matrix = originalMatrix;
    }
}
