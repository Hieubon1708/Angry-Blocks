using System.Collections.Generic;
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

    public GameObject preIn;
    public GameObject preOut;

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
        GameObject e = Instantiate(preIn, new Vector3(-16f, 0f, -28f), Quaternion.identity, transform);
        inn.Add(e.transform);
    }

    public void AddOut()
    {
        GameObject e = Instantiate(preOut, new Vector3(16f, 0f, -28f), Quaternion.identity, transform);
        outt.Add(e.transform);
    }

    public Vector3[] GetInPoints()
    {
        Vector3 nextPoint = inn.Count == 1 ? transform.position : inn[1].position;

        Vector3 dir = inn[0].position - nextPoint;

        inn[0].position += dir.normalized * 10;

        Vector3[] results = new Vector3[inn.Count];

        for (int i = 0; i < results.Length; i++)
        {
            results[i] = inn[i].position;
        }

        return results;
    }

    public Vector3[] GetOutPoints()
    {
        Vector3 nextPoint = outt.Count == 1 ? transform.position : outt[1].position;

        Vector3 dir = outt[0].position - nextPoint;

        outt[0].position += dir.normalized * 10;

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
