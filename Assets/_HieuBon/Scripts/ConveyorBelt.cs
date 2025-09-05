using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConveyorBelt : MonoBehaviour
{
    public float space;
    public float foodSpace;

    public GameObject preArrow;
    public GameObject preFoodPoint;

    public GameObject preCorner;
    public GameObject preStraight;
    public GameObject preInOut;

    int segmentsPerCurve = 10;

    [Range(0.01f, 0.1f)]
    public float speed;

    public Transform bar;

    [HideInInspector]
    public List<Vector3> cachedPathPoints = new List<Vector3>();

    List<Transform> pathTransforms = new List<Transform>();

    public List<Transform> arrows = new List<Transform>();
    List<ConveyorBeltArrow> arrowScripts = new List<ConveyorBeltArrow>();

    FoodPoint[] foodPoints;

    public void GenerateConveyorBelt(ConveyorBeltData[] conveyorBeltDatas, BarData barData)
    {
        for (int i = 0; i < conveyorBeltDatas.Length; i++)
        {
            GameObject e = null;

            if (conveyorBeltDatas[i].type == LevelData.ConveyorBeltType.Corner) e = Instantiate(preCorner, conveyorBeltDatas[i].position, conveyorBeltDatas[i].direction, transform);
            else if (conveyorBeltDatas[i].type == LevelData.ConveyorBeltType.Inout) e = Instantiate(preInOut, conveyorBeltDatas[i].position, conveyorBeltDatas[i].direction, transform);
            else e = Instantiate(preStraight, conveyorBeltDatas[i].position, conveyorBeltDatas[i].direction, transform);

            int length = e.transform.childCount;

            Transform[] points = new Transform[e.transform.childCount];

            for (int r = 0; r < length; r++)
            {
                Transform t = e.transform.GetChild(r);

                if (t.name.Contains("Sphere"))
                {
                    points[r] = t;
                }
            }

            if (conveyorBeltDatas[i].type == LevelData.ConveyorBeltType.Corner)
            {
                int index = 0;
                float minDistance = 100f;

                for (int k = 0; k < points.Length; k++)
                {
                    float distance = Vector3.Distance(pathTransforms[pathTransforms.Count - 1].position, points[k].position);

                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        index = k;
                    }
                }

                pathTransforms.Add(points[index == 0 ? 0 : 1]);
                pathTransforms.Add(points[index == 0 ? 1 : 0]);

            }
            else
            {
                pathTransforms.Add(points[0]);
            }
        }

        bar.position = new Vector3(0, 0, barData.position.z);

        GenerateSmoothPath();
        GenerateArrows();
        GenerateFoodPoints();
    }

    void GenerateArrows()
    {
        float totalDistance = 0;

        for (int i = 0; i < cachedPathPoints.Count - 1; i++)
        {
            totalDistance += Vector3.Distance(cachedPathPoints[i], cachedPathPoints[i + 1]);
        }

        float f = 0;

        int amountArrow = -1;

        while (f < totalDistance)
        {
            f += space;

            amountArrow++;
        }

        for (int i = 0; i < amountArrow; i++)
        {
            GameObject e = Instantiate(preArrow, transform);

            arrows.Add(e.transform);
            arrowScripts.Add(e.GetComponent<ConveyorBeltArrow>());
        }

        float distance = totalDistance / amountArrow;

        float a = 0;
        float b = distance;

        int arrowIndex = 1;

        Vector3 dir = (cachedPathPoints[1] - cachedPathPoints[0]).normalized;

        arrows[0].position = cachedPathPoints[0];
        arrows[0].rotation = Quaternion.LookRotation(dir) * Quaternion.Euler(-90, -90, 0);
        arrowScripts[0].SetSegmentIndex(0);

        for (int i = 0; i < cachedPathPoints.Count - 1; i++)
        {
            a += Vector3.Distance(cachedPathPoints[i], cachedPathPoints[i + 1]);

            if (a >= b && arrowIndex < arrows.Count)
            {
                float c = b - a;

                dir = (cachedPathPoints[i + 1] - cachedPathPoints[i]).normalized;

                arrows[arrowIndex].position = cachedPathPoints[i + 1] + dir * c;

                arrows[arrowIndex].rotation = Quaternion.LookRotation(dir) * Quaternion.Euler(-90, -90, 0);

                arrowScripts[arrowIndex].SetSegmentIndex(i + 1);

                b += distance;

                arrowIndex++;
            }
        }
    }

    void GenerateFoodPoints()
    {
        float totalDistance = 0;

        for (int i = 0; i < cachedPathPoints.Count - 1; i++)
        {
            totalDistance += Vector3.Distance(cachedPathPoints[i], cachedPathPoints[i + 1]);
        }

        float f = 0;

        int amountPoint = -1;

        while (f < totalDistance)
        {
            f += foodSpace;

            amountPoint++;
        }

        foodPoints = new FoodPoint[amountPoint];

        for (int i = 0; i < foodPoints.Length; i++)
        {
            GameObject e = Instantiate(preFoodPoint, transform);

            foodPoints[i] = e.GetComponent<FoodPoint>();
        }

        float distance = totalDistance / amountPoint;

        float a = 0;
        float b = distance;

        int pointIndex = 1;

        foodPoints[0].transform.position = cachedPathPoints[0];
        foodPoints[0].SetSegmentIndex(0);

        for (int i = 0; i < cachedPathPoints.Count - 1; i++)
        {
            a += Vector3.Distance(cachedPathPoints[i], cachedPathPoints[i + 1]);

            if (a >= b && pointIndex < foodPoints.Length)
            {
                float c = b - a;

                Vector3 dir = (cachedPathPoints[i + 1] - cachedPathPoints[i]).normalized;

                foodPoints[pointIndex].transform.position = cachedPathPoints[i + 1] + dir * c;

                foodPoints[pointIndex].SetSegmentIndex(i + 1);

                b += distance;

                pointIndex++;
            }
        }
    }

    void OnDrawGizmos()
    {
        if (cachedPathPoints != null && cachedPathPoints.Count > 1)
        {
            Gizmos.color = Color.blue;
            for (int i = 0; i < cachedPathPoints.Count - 1; i++)
            {
                Gizmos.DrawLine(cachedPathPoints[i], cachedPathPoints[i + 1]);
                Gizmos.DrawSphere(cachedPathPoints[i], 0.035f);
            }
        }
    }
    public void GenerateSmoothPath()
    {
        List<Vector3> controlPoints = new List<Vector3>();

        foreach (Transform t in pathTransforms)
        {
            controlPoints.Add(t.position);
        }

        controlPoints.Insert(0, controlPoints[0]);
        controlPoints.Add(controlPoints[controlPoints.Count - 1]);

        for (int i = 0; i < controlPoints.Count - 3; i++)
        {
            Vector3 p0 = controlPoints[i];
            Vector3 p1 = controlPoints[i + 1];
            Vector3 p2 = controlPoints[i + 2];
            Vector3 p3 = controlPoints[i + 3];

            for (int j = 0; j < segmentsPerCurve; j++)
            {
                float t = (float)j / segmentsPerCurve;
                Vector3 point = GetCatmullRomPoint(p0, p1, p2, p3, t);

                point.y += 0.01f;

                cachedPathPoints.Add(point);
            }
        }

        //cachedPathPoints.Add(controlPoints[controlPoints.Count - 2]);
    }

    Vector3 GetCatmullRomPoint(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
    {
        t = Mathf.Clamp01(t);

        Vector3 a = 2f * p1;
        Vector3 b = p2 - p0;
        Vector3 c = 2f * p0 - 5f * p1 + 4f * p2 - p3;
        Vector3 d = -p0 + 3f * p1 - 3f * p2 + p3;

        return 0.5f * (a + (b * t) + (c * t * t) + (d * t * t * t));
    }
}
