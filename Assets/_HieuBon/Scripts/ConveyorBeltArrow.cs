using UnityEngine;

public class ConveyorBeltArrow : MonoBehaviour
{
    [HideInInspector]
    public int currentSegmentIndex = -1;

    public void SetSegmentIndex(int currentSegmentIndex)
    {
        this.currentSegmentIndex = currentSegmentIndex;
    }

    private void FixedUpdate()
    {
        if (currentSegmentIndex == -1) return;

        Vector3 targetPoint = LevelController.instance.conveyorBelt.cachedPathPoints[currentSegmentIndex];
        Vector3 dir = targetPoint - transform.position;

        transform.position = Vector3.MoveTowards(transform.position, targetPoint, LevelController.instance.conveyorBelt.speed);
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(dir) * Quaternion.Euler(-90, -90, 0), LevelController.instance.conveyorBelt.speed * 10);

        if (Vector3.Distance(transform.position, targetPoint) < 0.01f)
        {
            currentSegmentIndex++;

            if (currentSegmentIndex >= LevelController.instance.conveyorBelt.cachedPathPoints.Count)
            {
                transform.position = LevelController.instance.conveyorBelt.cachedPathPoints[0];
                transform.rotation = Quaternion.LookRotation(LevelController.instance.conveyorBelt.cachedPathPoints[1] - transform.position) * Quaternion.Euler(-90, -90, 0);

                currentSegmentIndex = 1;
            }
        }
    }
}
