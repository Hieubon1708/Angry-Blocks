using System.Collections.Generic;
using UnityEngine;

public class ConveyorBelt : MonoBehaviour
{
    public float speed;

    Transform[] points;

    List<Vector3> cachedPathPoints = new List<Vector3>();

    LineRenderer lineRenderer;

    float startOffset;

    private void Awake()
    {
        GenerateSmoothPath();

        lineRenderer = GetComponentInChildren<LineRenderer>();

        lineRenderer.positionCount = cachedPathPoints.Count;

        for (int i = 0; i < lineRenderer.positionCount; i++)
        {
            Vector3 pos = cachedPathPoints[i];

            pos.y += 0.01f;

            lineRenderer.SetPosition(i, pos);
        }
    }

    private void Update()
    {
        lineRenderer.material.SetTextureOffset("_MainTex", new Vector2(startOffset -= Time.deltaTime, 0));
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
        int segmentsPerCurve = 100;

        Transform pointParent = transform.Find("Points");

        points = new Transform[pointParent.childCount];

        for (int i = 0; i < points.Length; i++)
        {
            points[i] = pointParent.GetChild(i);
        }

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
