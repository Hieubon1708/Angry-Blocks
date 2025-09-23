using UnityEngine;

public class ConveyorBeltArrow : MonoBehaviour
{
    int currentSegmentIndex = -1;

    public void SetSegmentIndex(int currentSegmentIndex)
    {
        this.currentSegmentIndex = currentSegmentIndex;
    }

    private void FixedUpdate()
    {
        if (currentSegmentIndex == -1 || currentSegmentIndex >= LevelController.instance.conveyorBelt.cachedPathPoints.Count) return;

        Vector3 targetPoint = LevelController.instance.conveyorBelt.cachedPathPoints[currentSegmentIndex];
        Vector3 dir = targetPoint - transform.position;

        transform.position = Vector3.MoveTowards(transform.position, targetPoint, GameController.instance.Speed);
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(dir) * Quaternion.Euler(-90, -90, 0), GameController.instance.Speed);

        if (Vector3.Distance(transform.position, targetPoint) < 0.1f)
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
