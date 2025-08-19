using System.Collections.Generic;
using UnityEngine;

public class ConveyorBelt : MonoBehaviour
{
    public float speed;

    public Transform[] points;

    public GameObject preArrow;

    public int amountArrow;

    int segmentsPerCurve = 10;

    [HideInInspector]
    public List<Vector3> cachedPathPoints = new List<Vector3>();

    ConveyorBeltArrow[] conveyorBeltArrows;

    private void Awake()
    {
        GenerateSmoothPath();

        conveyorBeltArrows = GetComponentsInChildren<ConveyorBeltArrow>();

        float totalDistance = 0;

        for (int i = 0; i < cachedPathPoints.Count - 1; i++)
        {
            totalDistance += Vector3.Distance(cachedPathPoints[i], cachedPathPoints[i + 1]);
        }

        float distance = totalDistance / (conveyorBeltArrows.Length - 1);

        float tempTotalDistance = 0;
        float tempPointDistance = 0;

        Debug.Log("a = " + totalDistance);
        Debug.Log("b = " + (conveyorBeltArrows.Length - 1));
        Debug.Log("Distance = " + distance);

        conveyorBeltArrows[0].transform.position = cachedPathPoints[0];

        int pointIndex = 0;
        int arrowIndex = 1;

        while (arrowIndex < conveyorBeltArrows.Length)
        {
            Debug.Log("Arrow Index = " + arrowIndex);
            if (arrowIndex == 2) break;
            tempTotalDistance += distance;

            while (true)
            {
                tempPointDistance += Vector3.Distance(cachedPathPoints[pointIndex], cachedPathPoints[pointIndex + 1]);

                if (tempTotalDistance < tempPointDistance)
                {
                    Debug.LogError("Point Index = " + pointIndex);
                    Debug.LogError("Distance Arrow = " + tempTotalDistance);
                    Debug.LogError("Distance Point = " + tempPointDistance);
                    break;
                }
                Debug.LogError("Point Index = " + pointIndex);
                Debug.LogError("Distance Arrow = " + tempTotalDistance);
                Debug.LogError("Distance Point = " + tempPointDistance);
                // lớn hơn khoảng cách thì tăng point lên 1
                pointIndex++;

                if (pointIndex == cachedPathPoints.Count - 1) break;

                // cộng khoảng cách các point đằng sau lại
            }

            Vector3 dir = Vector3.zero;

            if (pointIndex == cachedPathPoints.Count - 1)
            {
                dir = (cachedPathPoints[0] - cachedPathPoints[pointIndex]).normalized;
                conveyorBeltArrows[arrowIndex].transform.position = cachedPathPoints[pointIndex];
            }
            else
            {
                dir = (cachedPathPoints[pointIndex + 1] - cachedPathPoints[pointIndex]).normalized;
                conveyorBeltArrows[arrowIndex].transform.position = cachedPathPoints[pointIndex] + dir * (tempTotalDistance - tempPointDistance);
                Debug.Log(tempTotalDistance - tempPointDistance);
                Debug.Log(Vector3.Distance(conveyorBeltArrows[arrowIndex].transform.position, conveyorBeltArrows[arrowIndex - 1].transform.position));
            }

            //conveyorBeltArrows[arrowIndex].LoadData(pointIndex);

            arrowIndex++;
        }
    }

    void OnDrawGizmos()
    {
        if (points == null || points.Length < 2) return;

        Gizmos.color = Color.red;
        foreach (Transform t in points)
        {
            if (t != null) Gizmos.DrawSphere(t.position, 0.15f);
        }

        if (cachedPathPoints != null && cachedPathPoints.Count > 1)
        {
            Gizmos.color = Color.blue;
            for (int i = 0; i < cachedPathPoints.Count - 1; i++)
            {
                Gizmos.DrawLine(cachedPathPoints[i], cachedPathPoints[i + 1]);
            }
        }
    }

    public void GenerateSmoothPath()
    {
        List<Vector3> controlPoints = new List<Vector3>();

        foreach (Transform t in points)
        {
            controlPoints.Add(t.position);
        }

        if (controlPoints.Count > 0)
        {
            controlPoints.Insert(0, controlPoints[0]);
            controlPoints.Add(controlPoints[controlPoints.Count - 1]);
        }

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
                cachedPathPoints.Add(point);

                Instantiate(preArrow, point, Quaternion.identity);
            }
        }

        cachedPathPoints.Add(controlPoints[controlPoints.Count - 2]);
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
