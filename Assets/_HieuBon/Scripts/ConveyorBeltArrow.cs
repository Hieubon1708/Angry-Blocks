using System.Collections.Generic;
using UnityEngine;

public class ConveyorBeltArrow : MonoBehaviour
{
    ConveyorBelt conveyorBelt;

    int currentSegmentIndex = 0;

    private void Awake()
    {
        conveyorBelt = GetComponentInParent<ConveyorBelt>();
    }

    public void LoadData(int currentSegmentIndex)
    {
        this.currentSegmentIndex = currentSegmentIndex;
    }

    private void Update()
    {
        return;
        Vector3 targetPoint = conveyorBelt.cachedPathPoints[currentSegmentIndex];
        Vector3 direction = targetPoint - transform.position;

        float step = conveyorBelt.speed * Time.deltaTime;

        Quaternion targetRotation = Quaternion.LookRotation(direction);

        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 10 * Time.deltaTime);
        transform.position = Vector3.MoveTowards(transform.position, targetPoint, step);

        if (Vector3.Distance(transform.position, targetPoint) < 0.01f)
        {
            currentSegmentIndex++;
            if (currentSegmentIndex >= conveyorBelt.cachedPathPoints.Count)
            {
                currentSegmentIndex = 0;
            }
        }
    }
}
