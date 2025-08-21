using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ConveyorBelt : MonoBehaviour
{
    public int amoutArrow;

    public GameObject preArrow;

    int segmentsPerCurve = 10;

    [HideInInspector]
    public List<Vector3> cachedPathPoints = new List<Vector3>();

    List<Transform> pathTransforms;

    GameObject[] arrows;

    private void Awake()
    {
        arrows = new GameObject[amoutArrow];

        for (int i = 0; i < amoutArrow; i++)
        {
            arrows[i] = Instantiate(preArrow, transform);
        }

        GenerateSmoothPath();

        float totalDistance = 0;

        for (int i = 0; i < cachedPathPoints.Count - 1; i++)
        {
            totalDistance += Vector3.Distance(cachedPathPoints[i], cachedPathPoints[i + 1]);
        }

        float distance = totalDistance / amoutArrow;
        Debug.Log(totalDistance);
        Debug.Log(distance);

        float a = 0;
        float b = distance;

        int arrowIndex = 1;

        Vector3 dir = (cachedPathPoints[1] - cachedPathPoints[0]).normalized;

        arrows[0].transform.position = cachedPathPoints[0];
        arrows[0].transform.rotation = Quaternion.LookRotation(dir) * Quaternion.Euler(90, -90, 0);
        arrows[0].GetComponent<ConveyorBeltArrow>().LoadData(0);

        for (int i = 0; i < cachedPathPoints.Count - 1; i++)
        {
            a += Vector3.Distance(cachedPathPoints[i], cachedPathPoints[i + 1]);

            if (a >= b && arrowIndex < arrows.Length)
            {
                float c = b - a;

                dir = (cachedPathPoints[i + 1] - cachedPathPoints[i]).normalized;

                arrows[arrowIndex].transform.position = cachedPathPoints[i + 1] + dir * c;
                arrows[arrowIndex].transform.rotation = Quaternion.LookRotation(dir) * Quaternion.Euler(90, -90, 0);

                int currentSegment = arrowIndex == arrows.Length - 1 ? 0 : i + 1;

                arrows[arrowIndex].GetComponent<ConveyorBeltArrow>().LoadData(currentSegment);

                b += distance;

                arrowIndex++;
            }
        }
    }

    void OnDrawGizmos()
    {
        if (pathTransforms == null || pathTransforms.Count < 2) return;

        /*Gizmos.color = Color.red;
        foreach (Transform t in pathTransforms)
        {
            if (t != null) Gizmos.DrawSphere(t.position, 0.15f);
        }*/

        if (cachedPathPoints != null && cachedPathPoints.Count > 1)
        {
            Gizmos.color = Color.blue;
            for (int i = 0; i < cachedPathPoints.Count - 1; i++)
            {
                Gizmos.DrawLine(cachedPathPoints[i], cachedPathPoints[i + 1]);
                Gizmos.DrawSphere(cachedPathPoints[i], 0.01f);
            }
        }
    }
    public void GenerateSmoothPath()
    {
        Transform pointParent = transform.Find("Points");

        pathTransforms = new List<Transform>();

        for (int i = 0; i < pointParent.childCount; i++)
        {
            pathTransforms.Add(pointParent.GetChild(i));
        }

        List<Vector3> controlPoints = new List<Vector3>();

        foreach (Transform t in pathTransforms)
        {
            controlPoints.Add(t.position);
        }

        controlPoints.Add(controlPoints[0]);
        controlPoints.Add(controlPoints[1]);
        controlPoints.Add(controlPoints[2]);


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
