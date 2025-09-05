using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static GameController;
using static LevelData;

public class Tool : MonoBehaviour
{
    public static Tool instance;

    public GameObject preTray;
    public GameObject preShipper;
    public GameObject preCorner;
    public GameObject preStraight;
    public GameObject preInOut;

    public GameObject bar;

    public GameObject submitQuestion;

    Dictionary<GameObject, List<Transform>> snapPoints = new Dictionary<GameObject, List<Transform>>();

    public bool isGroup;

    private void Awake()
    {
        instance = this;

        gridMask = LayerMask.GetMask("GridTool");
        objMask = LayerMask.GetMask("ObjTool");
        trashCanMask = LayerMask.GetMask("TrashCan");
    }

    public Transform conveyorBeltContainer;

    Vector3 gridPos;

    LayerMask gridMask;
    LayerMask objMask;
    LayerMask trashCanMask;

    GameObject objDraging;

    public GameObject listFood;
    public GameObject listFoodForShipper;
    GameObject foodSelected;
    bool isTrayLanded;

    Vector3 snapPoint;

    public TMP_InputField moveAmount;

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && isTrayLanded)
        {
            if (!EventSystem.current.IsPointerOverGameObject())
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                RaycastHit hit;

                if (Physics.Raycast(ray, out hit, 100, objMask))
                {
                    if (hit.collider.CompareTag("FoodPoint"))
                    {
                        foodSelected = hit.collider.gameObject;
                        listFood.SetActive(true);
                    }
                    else
                    {
                        isTrayLanded = false;
                        objDraging = hit.collider.gameObject;
                    }
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.A) && objDraging != null)
        {
            objDraging.transform.rotation = Quaternion.Euler(0f, objDraging.transform.eulerAngles.y + 90, 0f);
        }

        if (Input.GetKeyDown(KeyCode.D) && objDraging != null)
        {
            objDraging.transform.rotation = Quaternion.Euler(0f, objDraging.transform.eulerAngles.y - 90, 0f);
        }

        if (Input.GetKeyDown(KeyCode.Z) && objDraging != null)
        {
            objDraging.transform.rotation = Quaternion.Euler(0f, objDraging.transform.eulerAngles.y + 15, 0f);
        }

        if (Input.GetKeyDown(KeyCode.C) && objDraging != null)
        {
            objDraging.transform.rotation = Quaternion.Euler(0f, objDraging.transform.eulerAngles.y - 15, 0f);
        }

        if (Input.GetKeyDown(KeyCode.G))
        {
            Group();
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            if (objDraging != null)
            {
                isTrayLanded = true;

                snapPoints.Remove(objDraging);

                Destroy(objDraging);

                objDraging = null;
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            if (!EventSystem.current.IsPointerOverGameObject())
            {
                if (objDraging != null)
                {
                    objDraging.transform.position = gridPos;

                    objDraging = null;

                    isTrayLanded = true;
                }
            }
            else
            {
                if (objDraging != null)
                {
                    PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
                    pointerEventData.position = Input.mousePosition;

                    List<RaycastResult> results = new List<RaycastResult>();

                    EventSystem.current.RaycastAll(pointerEventData, results);

                    if (results.Count > 0)
                    {
                        foreach (RaycastResult result in results)
                        {
                            if (result.gameObject.name == "Trash Can")
                            {
                                isTrayLanded = true;

                                snapPoints.Remove(objDraging);

                                Destroy(objDraging);

                                objDraging = null;

                                break;
                            }
                        }
                    }
                }
            }
        }

        if (objDraging != null)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 100, gridMask))
            {
                bool isUnuseGridPos = false;

                if (objDraging.name.Contains("ConveyorBelt"))
                {
                    if (isGroup)
                    {
                        objDraging = conveyorBeltContainer.gameObject;
                    }
                    else
                    {
                        foreach (var e in snapPoints)
                        {
                            if (e.Key != objDraging)
                            {
                                Vector3 mouse = hit.point;

                                foreach (var i in e.Value)
                                {
                                    mouse.y = i.position.y;

                                    Debug.DrawLine(i.position, mouse, Color.yellow);
                                    if (Vector3.Distance(i.position, mouse) <= 3)
                                    {
                                        float minDistance = 100f;
                                        Vector3 k = Vector3.zero;

                                        foreach (var j in snapPoints[objDraging])
                                        {
                                            float distance = Vector3.Distance(j.position, i.position);

                                            if (distance < minDistance)
                                            {
                                                k = j.position;

                                                minDistance = distance;
                                            }
                                        }

                                        Debug.DrawLine(snapPoint, objDraging.transform.position, Color.red);

                                        isUnuseGridPos = true;

                                        if (k != snapPoint)
                                        {
                                            snapPoint = k;

                                            gridPos = i.position - k + objDraging.transform.position;

                                            objDraging.transform.position = gridPos;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                if (!isUnuseGridPos)
                {
                    float x = Mathf.Round(hit.point.x);
                    float z = Mathf.Round(hit.point.z);

                    gridPos = new Vector3(x, objDraging.transform.position.y, z);

                    objDraging.transform.position = gridPos;
                }
            }
        }
    }

    public FoodType GetFoodTypeByIndex(int index)
    {
        FoodType[] foodTypes = (GameController.FoodType[])Enum.GetValues(typeof(GameController.FoodType));

        for (int i = 0; i < foodTypes.Length; i++)
        {
            if (i == index) return foodTypes[i];
        }

        return FoodType.None;
    }

    public void HideFoodOptions()
    {
        listFood.SetActive(false);
    }

    public void ShipperHideFoodOptions()
    {
        listFoodForShipper.SetActive(false);
    }

    public void ShipperResetFood()
    {
        listFoodForShipper.SetActive(false);

        foodSelected.GetComponent<ShipperTool>().Rs();
    }

    public void ShipperShowFood(GameObject e)
    {
        if (!isTrayLanded) return;

        foodSelected = e;
        listFoodForShipper.SetActive(true);
    }

    public void FoodSelect(int type)
    {
        listFood.SetActive(false);

        if (foodSelected.GetComponent<TrayTool>() != null)
        {
            foodSelected.GetComponent<TrayTool>().SetAll(GetFoodTypeByIndex(type));
        }
        else
        {
            foodSelected.GetComponentInParent<TrayTool>().SetFood(foodSelected, GetFoodTypeByIndex(type));
        }
    }

    public void ShipperFoodSelect(int type)
    {
        listFoodForShipper.SetActive(false);

        foodSelected.GetComponent<ShipperTool>().AddFood(type);
    }

    public void OnClickReset()
    {
        SceneManager.LoadScene(0);
    }

    public GameObject listTrayLayer;

    public void OnClickTray()
    {
        isTrayLanded = false;

        listTrayLayer.SetActive(!listTrayLayer.activeSelf);
    }

    public void OnClickTrayLayer(int layer)
    {
        listTrayLayer.SetActive(false);

        objDraging = Instantiate(preTray, new Vector3(0, layer * 3, 0), Quaternion.identity, transform);

        objDraging.GetComponentInChildren<TextMeshProUGUI>().text = (layer + 1).ToString();
    }

    public void OnClickShipper()
    {
        objDraging = Instantiate(preShipper, transform);
    }

    public void ShowSubmit()
    {
        submitQuestion.SetActive(true);
    }

    public void HideSubmit()
    {
        submitQuestion.SetActive(false);
    }

    public TMP_InputField levelName;

    public void OnClickSubmit()
    {
        submitQuestion.SetActive(false);

        TrayTool[] trayTools = GetComponentsInChildren<TrayTool>();

        TrayData[] trayDatas = new TrayData[trayTools.Length];

        for (int i = 0; i < trayDatas.Length; i++)
        {
            trayDatas[i] = new TrayData(trayTools[i].transform.position, trayTools[i].transform.rotation, trayTools[i].foodTypes);
        }

        ShipperTool[] shipperTools = GetComponentsInChildren<ShipperTool>();

        ShipperData[] shipperDatas = new ShipperData[shipperTools.Length];

        for (int i = 0; i < shipperDatas.Length; i++)
        {
            shipperDatas[i] = new ShipperData(shipperTools[i].transform.position, shipperTools[i].transform.rotation,
                shipperTools[i].foodTypes.ToArray(), shipperTools[i].GetOutPoints(), shipperTools[i].GetInPoints());
        }

        ConveyorBeltData[] conveyorBeltDatas = new ConveyorBeltData[conveyorBeltContainer.childCount];

        for (int i = 0; i < conveyorBeltDatas.Length; i++)
        {
            GameObject e = conveyorBeltContainer.GetChild(i).gameObject;

            LevelData.ConveyorBeltType conveyorBeltType = LevelData.ConveyorBeltType.Straight;

            if (e.name.Contains("Corner")) conveyorBeltType = LevelData.ConveyorBeltType.Corner;
            else if (e.name.Contains("InOut")) conveyorBeltType = LevelData.ConveyorBeltType.Inout;

            conveyorBeltDatas[i] = new ConveyorBeltData(conveyorBeltType, e.transform.position, e.transform.rotation);
        }

        ConveyorBeltTool[] a = conveyorBeltContainer.GetComponentsInChildren<ConveyorBeltTool>();

        for (int i = 0; i < a.Length - 1; i++)
        {
            string t1 = a[i].txt.text;

            int n1 = int.Parse(t1);

            for (int j = i + 1; j < conveyorBeltDatas.Length; j++)
            {
                string t2 = a[j].txt.text;

                int n2 = int.Parse(t2);

                if (n1 > n2)
                {
                    ConveyorBeltTool temp1 = a[j];
                    a[j] = a[i];
                    a[i] = temp1;

                    ConveyorBeltData temp2 = conveyorBeltDatas[j];
                    conveyorBeltDatas[j] = conveyorBeltDatas[i];
                    conveyorBeltDatas[i] = temp2;
                }
            }
        }

        BarData barData = new BarData(bar.transform.position, bar.transform.rotation);

        levelData = new LevelData(shipperDatas, trayDatas, conveyorBeltDatas, barData);

        levelData.moveAmount = int.Parse(moveAmount.text);

        string jsonData = JsonUtility.ToJson(levelData);

        string filePath = Path.Combine(Application.persistentDataPath, levelName.text + ".json");

        File.WriteAllText(filePath, jsonData);

        Debug.Log("Đã lưu dữ liệu vào: " + filePath);
    }

    public LevelData levelData;

    public void OnClickCorner()
    {
        isGroup = false;
        buttonGroup.color = Color.white;

        objDraging = Instantiate(preCorner, conveyorBeltContainer);

        GameObject k = objDraging;
        List<Transform> v = new List<Transform>();

        for (int i = 0; i < objDraging.transform.childCount; i++)
        {
            if (objDraging.transform.GetChild(i).name.Contains("Snap"))
            {
                v.Add(objDraging.transform.GetChild(i));
            }
        }

        snapPoints.Add(k, v);
    }

    public void OnClickInOut()
    {
        isGroup = false;
        buttonGroup.color = Color.white;

        objDraging = Instantiate(preInOut, conveyorBeltContainer);

        GameObject k = objDraging;
        List<Transform> v = new List<Transform>();

        for (int i = 0; i < objDraging.transform.childCount; i++)
        {
            if (objDraging.transform.GetChild(i).name.Contains("Snap"))
            {
                v.Add(objDraging.transform.GetChild(i));
            }
        }

        snapPoints.Add(k, v);
    }

    public void OnClickStraight()
    {
        isGroup = false;
        buttonGroup.color = Color.white;

        objDraging = Instantiate(preStraight, conveyorBeltContainer);

        GameObject k = objDraging;
        List<Transform> v = new List<Transform>();

        for (int i = 0; i < objDraging.transform.childCount; i++)
        {
            if (objDraging.transform.GetChild(i).name.Contains("Snap"))
            {
                v.Add(objDraging.transform.GetChild(i));
            }
        }

        snapPoints.Add(k, v);
    }

    public GameObject listConveyorBelt;

    public void OnClickConveyourBelt()
    {
        listConveyorBelt.SetActive(!listConveyorBelt.activeSelf);
    }

    public void OnClickFoodAll(GameObject gameObject)
    {
        listFood.SetActive(true);

        foodSelected = gameObject;
    }

    public Image buttonGroup;

    public void Group()
    {
        buttonGroup.color = isGroup ? Color.white : Color.green;

        isGroup = !isGroup;

        if (!isGroup)
        {
            if (objDraging != null)
            {
                objDraging.transform.position = gridPos;

                objDraging = null;

                isTrayLanded = true;
            }
        }
    }
}
