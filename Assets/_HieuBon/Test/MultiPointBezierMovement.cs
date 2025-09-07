using UnityEngine;
using System.Collections.Generic;

public class MultiPointBezierMovement : MonoBehaviour
{
    // Public list of points to set in the Unity Inspector
    public List<Transform> waypoints;
    public float speed = 1f;

    private float t = 0f;
    private int currentWaypointIndex = 0;

    void Update()
    {
        // Ensure there are at least 2 waypoints
        if (waypoints == null || waypoints.Count < 2)
        {
            Debug.LogWarning("Please assign at least two waypoints to the list.");
            return;
        }

        // Check if we've reached the end of the path
        if (currentWaypointIndex >= waypoints.Count - 1)
        {
            // Optional: Loop the path by resetting
            currentWaypointIndex = 0;
            t = 0f;
            return; // Exit the Update loop for this frame
        }

        // Get the current segment's points
        Vector3 p0 = waypoints[currentWaypointIndex].position;
        Vector3 p1 = waypoints[currentWaypointIndex + 1].position;

        // Calculate a simple control point
        // For a more advanced, smoother curve, you'd calculate control points
        // based on the surrounding waypoints. This example uses a midpoint.
        Vector3 controlPoint = (p0 + p1) / 2f;

        // Calculate the new position using the Bézier curve formula
        // P(t) = (1-t)^2 * P0 + 2*(1-t)*t*P1 + t^2*P2
        // Note: We use the midpoint as a simple control point for this example
        Vector3 newPosition = Mathf.Pow(1 - t, 2) * p0 +
                              2 * (1 - t) * t * controlPoint +
                              Mathf.Pow(t, 2) * p1;

        // Update the object's position
        transform.position = newPosition;

        // Increase t based on speed
        t += speed * Time.deltaTime;

        // If t reaches 1, move to the next waypoint
        if (t >= 1f)
        {
            t = 0f;
            currentWaypointIndex++;
        }
    }
}