using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;
using static GameController;

public class ShipperTool : MonoBehaviour
{
    public RectTransform rect;
    public RectTransform showHideRect;
    public RectTransform inOutRect;
    public RectTransform buttonSet;

    public Sprite[] sprites;

    public GameObject pre;

    List<GameObject> imgs = new List<GameObject>();
    public List<int> foodTypes = new List<int>();

    public Transform grid;

    public List<Transform> inn = new List<Transform>();
    public List<Transform> outt = new List<Transform>();

    public GameObject prePivot;
    public GameObject preIn;
    public GameObject preOut;

    public GameObject buttonPivot;

    public Transform pivot;

    public void Import(ShipperData shipperData)
    {
        transform.position = shipperData.position;
        transform.rotation = shipperData.direction;

        for (int i = 0; i < shipperData.foodType.Length; i++)
        {
            int index = shipperData.foodType[i];

            foodTypes.Add(index);

            GameObject e = Instantiate(pre, grid);

            e.GetComponent<Image>().sprite = sprites[index];

            imgs.Add(e);
        }

        for (int i = 0; i < shipperData.endPoints.Length; i++)
        {
            GameObject e = Instantiate(preOut, shipperData.endPoints[i], Quaternion.identity, transform);
            outt.Add(e.transform);
        }

        for (int i = 0; i < shipperData.startPoints.Length; i++)
        {
            GameObject e = Instantiate(preIn, shipperData.startPoints[i], Quaternion.identity, transform);
            inn.Add(e.transform);
        }

        if (shipperData.pivot != Vector3.zero)
        {
            buttonPivot.SetActive(false);
            pivot = Instantiate(prePivot, shipperData.pivot, Quaternion.identity, transform).transform;
        }
    }

    private void Update()
    {
        rect.rotation = Quaternion.LookRotation(new Vector3(0, -1, 0));
        inOutRect.rotation = Quaternion.LookRotation(new Vector3(0, -1, 0));
        buttonSet.rotation = Quaternion.LookRotation(new Vector3(0, -1, 0));
        showHideRect.rotation = Quaternion.LookRotation(new Vector3(0, -1, 0));
    }

    public void Rs()
    {
        foreach (var e in imgs)
        {
            Destroy(e);
        }

        foodTypes.Clear();
        imgs.Clear();
    }

    public void RemoveLastFood()
    {
        if (foodTypes.Count > 0)
        {
            int index = foodTypes.Count - 1;
            foodTypes.RemoveAt(index);
            Destroy(imgs[index].gameObject);
            imgs.RemoveAt(index);
        }
    }

    public void AddFood(int index)
    {
        foodTypes.Add(index);

        GameObject e = Instantiate(pre, grid);

        e.GetComponent<Image>().sprite = sprites[index];

        imgs.Add(e);
    }

    public void ShowFood()
    {
        Tool.instance.ShipperShowFood(gameObject);
    }

    public void AddIn()
    {
        GameObject e = Instantiate(preIn, new Vector3(-23f, 0f, -17f), Quaternion.identity, transform);
        inn.Add(e.transform);
    }

    public void AddPivot()
    {
        buttonPivot.SetActive(false);
        pivot = Instantiate(prePivot, new Vector3(-23f, 3.5f, -17f), Quaternion.identity, transform).transform;
    }

    public void AddOut()
    {
        GameObject e = Instantiate(preOut, new Vector3(23f, 0f, -17f), Quaternion.identity, transform);
        outt.Add(e.transform);
    }

    public Vector3[] GetInPoints()
    {
        Vector3[] results = new Vector3[inn.Count];

        for (int i = 0; i < results.Length; i++)
        {
            results[i] = inn[i].position;
        }

        return results;
    }

    public Vector3[] GetOutPoints()
    {
        Vector3[] results = new Vector3[outt.Count];

        for (int i = 0; i < results.Length; i++)
        {
            results[i] = outt[i].position;
        }

        return results;
    }

    public void ShowHideMesh()
    {
        transform.GetChild(0).gameObject.SetActive(!transform.GetChild(0).gameObject.activeSelf);
    }
}
