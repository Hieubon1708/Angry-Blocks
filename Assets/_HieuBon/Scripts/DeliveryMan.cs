using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class DeliveryMan : MonoBehaviour
{
    public GameController.FoodType foodType;

    [HideInInspector]
    public DeliveryLine deliveryLine;

    Collider[] colliders;

    LayerMask mask;

    public float distance;

    public Transform[] points;

    public Transform foodContainer;

    [HideInInspector]
    public int indexPoint;

    [HideInInspector]
    public DeliveryManAnimator deliveryManAnimator;

    public float speed = 15;

    public MeshRenderer meshBoxBody;
    public MeshRenderer meshBoxUpper;

    [HideInInspector]
    public bool isMovedIn;

    public Vector3 pivot;

    public Image foodIcon;

    public ParticleSystem smoke;
    public ParticleSystem star;

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

    public void SetFoodTypeAndPivot(GameController.FoodType foodType, Vector3 pivot)
    {
        this.foodType = foodType;
        this.pivot = pivot;

        foodIcon.sprite = GameController.instance.GetFoodIcon(foodType);

        //meshBoxBody.material.color = GameController.instance.GetBoxBodyColor(foodType);
        //meshBoxUpper.material.color = GameController.instance.GetBoxUpperColor(foodType);
    }

    private void Update()
    {
        FoodIconRotate();

        if (!isMovedIn) return;

        if (LevelController.instance.gameState == LevelController.GameState.Win
         || LevelController.instance.gameState == LevelController.GameState.Lose) return;

        int count = Physics.OverlapSphereNonAlloc(pivot, distance, colliders, mask);

        if (count > 0 && indexPoint < points.Length)
        {
            for (int i = 0; i < count; i++)
            {
                Food food = FoodOnConveyorBelt.instance.GetFood(colliders[i].gameObject);

                if (food != null && food.foodType == foodType)
                {
                    if (indexPoint == 0) deliveryManAnimator.OpenBox();

                    int j = indexPoint;

                    Vector3 eulers = food.transform.eulerAngles;
                    eulers.y += 1080;

                    food.transform.DORotate(eulers, 0.5f, RotateMode.FastBeyond360);
                    food.transform.DOJump(points[indexPoint].position, 10, 1, 0.5f).OnComplete(delegate
                    {
                        AudioController.instance.PlaySoundNVibrate(AudioController.instance.onDropShippersBox, 25);

                        if (j == points.Length - 1)
                        {
                            star.Play();

                            deliveryManAnimator.CloseBox();

                            DOVirtual.DelayedCall(0.25f, delegate
                            {
                                StartCoroutine(MoveOut(deliveryLine.endPoints));
                            });
                        }
                    });
                    food.transform.DOScale(3f, 0.25f);
                    food.transform.SetParent(foodContainer);

                    indexPoint++;

                    FoodOnConveyorBelt.instance.RemoveFoodOnConveyorBelt(food);
                }
            }
        }
    }

    public void Magnet(List<FoodTray> f)
    {
        if (isMovedIn && indexPoint < points.Length)
        {
            FoodTray foodTray = null;

            for (int i = 0; i < f.Count; i++)
            {
                if (f[i].IsContainFood(foodType))
                {
                    foodTray = f[i];

                    break;
                }
            }

            if (foodTray != null)
            {
                f.Remove(foodTray);
                foodTray.Toss(this);
            }
        }
    }

    public IEnumerator Waiting(Vector3[] startPoints)
    {
        foodContainer.gameObject.SetActive(true);
        deliveryManAnimator.Wheelie(true);

        while (Vector3.Distance(transform.position, startPoints[1]) >= 0.01f)
        {
            transform.position = Vector3.MoveTowards(transform.position, startPoints[1], Time.deltaTime * speed);

            yield return new WaitForEndOfFrame();
        }

        Vector3 target = startPoints.Length == 1 ? deliveryLine.transform.position : startPoints[1];
        Vector3 dir = target - transform.position;

        float angle = Vector3.Angle(transform.forward, dir);

        while (angle > 3 && Vector3.Distance(transform.position, target) >= 0.01f)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(dir), Time.deltaTime * speed);

            angle = Vector3.Angle(transform.forward, dir);

            yield return new WaitForEndOfFrame();
        }

        deliveryManAnimator.Wheelie(false);
    }

    void FoodIconRotate()
    {
        foodIcon.transform.localRotation = Quaternion.Euler(90, -transform.eulerAngles.y, 0);
    }

    public IEnumerator MoveIn(bool isStart, Vector3[] startPoints, DeliveryMan behind)
    {
        if (startPoints.Length == 0) yield break;

        foodContainer.gameObject.SetActive(true);

        deliveryManAnimator.Wheelie(true);

        int index = isStart ? 0 : 1;

        float angle;

        while (index < startPoints.Length + 1)
        {
            Vector3 target = index < startPoints.Length ? startPoints[index] : deliveryLine.transform.position;
            Vector3 dir = target - transform.position;

            angle = Vector3.Angle(transform.forward, dir);

            while (angle > 3 && Vector3.Distance(transform.position, target) >= 0.01f)
            {
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(dir), Time.deltaTime * speed);

                angle = Vector3.Angle(transform.forward, dir);

                yield return new WaitForEndOfFrame();
            }

            while (Vector3.Distance(transform.position, target) >= 0.01f)
            {
                transform.position = Vector3.MoveTowards(transform.position, target, Time.deltaTime * speed);

                yield return new WaitForEndOfFrame();
            }

            if (index == 1 && behind != null) StartCoroutine(behind.Waiting(startPoints));
            index++;
        }

        angle = Vector3.Angle(transform.forward, deliveryLine.transform.forward);

        while (angle > 3)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(deliveryLine.transform.forward), Time.deltaTime * speed);

            angle = Vector3.Angle(transform.forward, deliveryLine.transform.forward);

            yield return new WaitForEndOfFrame();
        }

        deliveryManAnimator.Wheelie(false);

        AudioController.instance.PlaySoundNVibrate(AudioController.instance.shipperArrives);

        yield return new WaitForSeconds(0.1f);

        isMovedIn = true;
    }

    public IEnumerator MoveOut(Vector3[] endPoints)
    {
        if (endPoints.Length == 0) yield break;

        smoke.Play();

        AudioController.instance.PlaySoundNVibrate(AudioController.instance.shipperGo);

        deliveryLine.NextDeliveryMan();

        deliveryManAnimator.Wheelie(true);

        int index = endPoints.Length - 1;

        while (index >= 0)
        {
            Vector3 target = index < endPoints.Length ? endPoints[index] : deliveryLine.transform.position;
            Vector3 dir = target - transform.position;

            float angle = Vector3.Angle(transform.forward, dir);

            while (angle > 3)
            {
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(dir), Time.deltaTime * speed);

                angle = Vector3.Angle(transform.forward, dir);

                yield return new WaitForEndOfFrame();
            }

            while (Vector3.Distance(transform.position, target) >= 0.01f)
            {
                transform.position = Vector3.MoveTowards(transform.position, target, Time.deltaTime * speed);

                yield return new WaitForEndOfFrame();
            }

            index--;
        }

        foodContainer.gameObject.SetActive(false);

        smoke.Stop();

        deliveryLine.indexManDelivered++;

        LevelController.instance.deliveryController.IsWin();

        isMovedIn = false;
    }
}
