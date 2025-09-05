using DG.Tweening;
using System.Collections;
using System.Net;
using UnityEngine;

public class DeliveryMan : MonoBehaviour
{
    public GameController.FoodType foodType;

    DeliveryLine deliveryLine;

    Collider[] colliders;

    LayerMask mask;

    public float distance;

    public Transform[] points;

    public Transform foodContainer;

    int indexPoint;

    [HideInInspector]
    public DeliveryManAnimator deliveryManAnimator;

    public float speed = 15;

    public MeshRenderer meshBoxBody;
    public MeshRenderer meshBoxUpper;

    bool isMovedIn;

    private void Awake()
    {
        deliveryLine = GetComponentInParent<DeliveryLine>();
        deliveryManAnimator = GetComponentInChildren<DeliveryManAnimator>();
    }

    private void Start()
    {
        colliders = new Collider[5];

        mask = LayerMask.GetMask("Food");
    }

    public void SetFoodType(GameController.FoodType foodType)
    {
        this.foodType = foodType;

        meshBoxBody.material.color = GameController.instance.GetBoxBodyColor(foodType);
        meshBoxUpper.material.color = GameController.instance.GetBoxUpperColor(foodType);
    }

    private void Update()
    {
        if (!isMovedIn) return;

        int count = Physics.OverlapSphereNonAlloc(transform.position, distance, colliders, mask);

        if (count > 0 && indexPoint < points.Length)
        {
            for (int i = 0; i < count; i++)
            {
                Food food = FoodOnConveyorBelt.instance.GetFood(colliders[i].gameObject);

                if (food != null && food.foodType == foodType)
                {
                    if (indexPoint == 0) deliveryManAnimator.OpenBox();

                    int j = indexPoint;

                    food.transform.DOJump(points[indexPoint].position, 10, 1, 0.5f).OnComplete(delegate
                    {
                        if (j == points.Length - 1)
                        {
                            deliveryManAnimator.CloseBox();

                            DOVirtual.DelayedCall(0.15f, delegate
                            {
                                StartCoroutine(MoveOut(deliveryLine.endPoints));
                            });
                        }
                    });
                    food.transform.DOScale(2f, 0.25f);
                    food.transform.SetParent(foodContainer);

                    indexPoint++;

                    FoodOnConveyorBelt.instance.RemoveFoodOnConveyorBelt(food);
                }
            }
        }
    }

    public IEnumerator MoveIn(Vector3[] startPoints)
    {
        if (startPoints.Length == 0) yield break;

        gameObject.SetActive(true);

        deliveryManAnimator.Wheelie(true);

        int index = 1;

        transform.position = startPoints[0];

        while (index < startPoints.Length + 1)
        {
            Vector3 target = index < startPoints.Length ? startPoints[index] : deliveryLine.transform.position;
            Vector3 dir = target - transform.position;

            transform.rotation = Quaternion.LookRotation(dir);

            while (Vector3.Distance(transform.position, target) >= 0.01f)
            {
                transform.position = Vector3.MoveTowards(transform.position, target, Time.deltaTime * speed);

                yield return new WaitForEndOfFrame();
            }

            index++;
        }
        deliveryManAnimator.Wheelie(false);

        yield return new WaitForSeconds(0.1f);

        isMovedIn = true;
    }

    public IEnumerator MoveOut(Vector3[] endPoints)
    {
        if (endPoints.Length == 0) yield break;

        deliveryLine.NextDeliveryMan();

        deliveryManAnimator.Wheelie(true);

        int index = endPoints.Length - 1;

        while (index >= 0)
        {
            Vector3 target = index < endPoints.Length ? endPoints[index] : deliveryLine.transform.position;
            Vector3 dir = target - transform.position;

            transform.rotation = Quaternion.LookRotation(dir);

            while (Vector3.Distance(transform.position, target) >= 0.01f)
            {
                transform.position = Vector3.MoveTowards(transform.position, target, Time.deltaTime * speed);

                yield return new WaitForEndOfFrame();
            }

            index--;
        }

        gameObject.SetActive(false);

        deliveryLine.indexManDelivered++;

        LevelController.instance.deliveryController.IsWin();
    }
}
