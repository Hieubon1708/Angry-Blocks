using DG.Tweening;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static GameController;

public class FoodTray : MonoBehaviour
{
    [HideInInspector]
    public List<Food> foods = new List<Food>();
    [HideInInspector]
    public MeshRenderer meshRenderer;
    [HideInInspector]
    public BoxCollider boxCollider;
    [HideInInspector]
    public Animator animator;
    IceTray iceTray;

    LayerMask foodTrayMask;

    public Transform[] points;

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        meshRenderer = GetComponentInChildren<MeshRenderer>();
        boxCollider = GetComponent<BoxCollider>();
        iceTray = GetComponentInChildren<IceTray>(true);

        foodTrayMask = LayerMask.GetMask("FoodTray");

        DOVirtual.DelayedCall(0.05f, delegate
        {
            CheckSphereCast();
        });
    }

    public void ActiveFoods(bool isActive)
    {
        foreach (var e in foods)
        {
            e.gameObject.SetActive(isActive);
        }
    }

    public bool IsContainFood(GameController.FoodType foodType)
    {
        foreach (var e in foods)
        {
            if (e.foodType == foodType) return true;
        }

        return false;
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

    public bool IsFreeze()
    {
        return iceTray.gameObject.activeSelf;
    }

    public void Holding()
    {
        animator.SetTrigger("Holding");
    }

    public void Toss(DeliveryMan deliveryMan = null)
    {
        this.deliveryMan = deliveryMan;

        boxCollider.enabled = false;

        animator.SetTrigger("Toss");
    }

    DeliveryMan deliveryMan;

    public void AfterToss()
    {
        if (LevelController.instance.foodTrays.IsOutOfTray()) AudioController.instance.StopOnFire();

        LevelController.instance.foodTrays.MinusFreeze();

        if (deliveryMan != null)
        {
            if (meshRenderer.material.color != Color.white)
            {
                meshRenderer.material.color = Color.white;
                //meshRenderer.material.SetColor("_EmissionColor", new Color(0.25f, 0.25f, 0.25f));

                foreach (var e in foods)
                {
                    e.meshRenderer.material.color = Color.white;
                    //e.meshRenderer.material.SetColor("_EmissionColor", new Color(0.25f, 0.25f, 0.25f));
                }
            }

            Vector3 p = deliveryMan.transform.position;

            float headDistance = Vector3.Distance(foods[0].transform.position, p);
            float tailDistance = Vector3.Distance(foods[3].transform.position, p);

            if (tailDistance < headDistance) foods.Reverse();

            for (int j = 0; j < foods.Count; j++)
            {
                Food food = foods[j];

                if (food.foodType == deliveryMan.foodType && deliveryMan.indexPoint < deliveryMan.points.Length)
                {
                    if (deliveryMan.indexPoint == 0) deliveryMan.deliveryManAnimator.OpenBox();

                    int k = deliveryMan.indexPoint;

                    Vector3 eulers = food.transform.eulerAngles;
                    eulers.y += 1080;

                    float time = j * 0.15f + 0.3f;

                    food.transform.DORotate(eulers, time, RotateMode.FastBeyond360);

                    food.transform.DOJump(deliveryMan.points[k].position, 25, 1, time).OnComplete(delegate
                    {
                        AudioController.instance.PlaySoundNVibrate(AudioController.instance.onDropShippersBox, 25);

                        if (k == deliveryMan.points.Length - 1)
                        {
                            deliveryMan.star.Play();

                            deliveryMan.deliveryManAnimator.CloseBox();

                            DOVirtual.DelayedCall(0.15f, delegate
                            {
                                StartCoroutine(deliveryMan.MoveOut(deliveryMan.deliveryLine.endPoints));
                            });
                        }
                    });

                    food.transform.DOScale(1.5f, 0.25f);
                    food.transform.SetParent(deliveryMan.foodContainer);

                    deliveryMan.indexPoint++;
                }
                else
                {
                    Vector3 tossPoint = LevelController.instance.conveyorBelt.cachedPathPoints[0];

                    int index = j;

                    FoodOnConveyorBelt.instance.SetParent(foods[index]);

                    float time = index * 0.15f + 0.25f;

                    foods[index].meshRenderer.transform.DOScale(0.5f, time);

                    foods[index].transform.DOJump(tossPoint, 25, 1, time).OnComplete(delegate
                    {
                        AudioController.instance.PlaySoundNVibrate(AudioController.instance.onDropConveyorBeltByMagnet, 25);

                        FoodOnConveyorBelt.instance.AddFood(foods[index]);
                    });
                }
            }
        }
        else
        {
            Vector3 tossPoint = LevelController.instance.conveyorBelt.cachedPathPoints[0];

            float headDistance = Vector3.Distance(foods[0].transform.position, tossPoint);
            float tailDistance = Vector3.Distance(foods[3].transform.position, tossPoint);

            if (tailDistance < headDistance) foods.Reverse();

            for (int i = 0; i < foods.Count; i++)
            {
                int index = i;

                FoodOnConveyorBelt.instance.SetParent(foods[index]);

                float time = index * 0.15f + 0.25f;

                Quaternion q = Quaternion.Euler(0, 45, 0);

                foods[index].meshRenderer.transform.DOScale(0.55f, time);
                foods[index].transform.DORotate(new Vector3(0, 45, 0), time);
                foods[index].transform.DOJump(tossPoint, 25, 1, time).OnComplete(delegate
                {
                    AudioController.instance.PlaySoundNVibrate(AudioController.instance.onDropConveyorBelt, 25);

                    FoodOnConveyorBelt.instance.AddFood(foods[index]);
                });
            }
        }
    }

    Collider[] result = new Collider[4];

    public void CheckSphereCast()
    {
        if (!boxCollider.enabled) return;

        Vector3 actualBoxCenter = transform.position + transform.up * 12.15f;

        int numColliders = Physics.OverlapBoxNonAlloc(actualBoxCenter, boxHalfExtents, result, transform.rotation, foodTrayMask);

        Color targetColor = numColliders == 0 ? Color.white : new Color(0.5f, 0.5f, 0.5f);
        //Color targetEmisionColor = numColliders == 0 ? new Color(0.25f, 0.25f, 0.25f) : new Color(0.25f, 0.25f, 0.25f);

        if (numColliders == 0 && meshRenderer.material.color == targetColor) return;

        meshRenderer.material.color = targetColor;
        //meshRenderer.material.SetColor("_EmissionColor", targetEmisionColor);

        foreach (var e in foods)
        {
            e.meshRenderer.material.color = targetColor;
            //e.meshRenderer.material.SetColor("_EmissionColor", targetEmisionColor);
        }
    }

    Vector3 boxHalfExtents = new Vector3(4.85f, 10f, 2.075f);

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        Vector3 actualBoxCenter = transform.position + transform.up * 12.15f;

        Matrix4x4 originalMatrix = Gizmos.matrix;

        Gizmos.matrix = Matrix4x4.TRS(actualBoxCenter, transform.rotation, Vector3.one);

        Gizmos.DrawWireCube(Vector3.zero, boxHalfExtents * 2);

        Gizmos.matrix = originalMatrix;
    }
}
