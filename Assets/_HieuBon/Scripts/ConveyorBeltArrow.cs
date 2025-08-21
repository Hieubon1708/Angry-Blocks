using UnityEngine;

public class ConveyorBeltArrow : MonoBehaviour
{
    ConveyorBelt conveyorBelt;

    int currentSegmentIndex = -1;

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
        if (currentSegmentIndex == -1) return;

        Vector3 targetPoint = conveyorBelt.cachedPathPoints[currentSegmentIndex];

        targetPoint.y += 0.01f;

        Vector3 dir = targetPoint - transform.position;

        transform.position = Vector3.MoveTowards(transform.position, targetPoint, Time.deltaTime * 1);
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(dir) * Quaternion.Euler(90, -90, 0), Time.deltaTime * 10);

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
