using UnityEngine;

public class FoodPoint : MonoBehaviour
{
    public bool isEmpty;

    int currentSegmentIndex = -1;

    Food food;

    public void SetSegmentIndex(int currentSegmentIndex)
    {
        this.currentSegmentIndex = currentSegmentIndex;
    }

    private void FixedUpdate()
    {
        if (currentSegmentIndex == -1 || currentSegmentIndex >= LevelController.instance.conveyorBelt.cachedPathPoints.Count) return;

        Vector3 targetPoint = LevelController.instance.conveyorBelt.cachedPathPoints[currentSegmentIndex];

        transform.position = Vector3.MoveTowards(transform.position, targetPoint, GameController.instance.Speed);

        if (Vector3.Distance(transform.position, targetPoint) < 0.1f)
        {
            currentSegmentIndex++;

            if (currentSegmentIndex == LevelController.instance.conveyorBelt.cachedPathPoints.Count - 1 && !isEmpty
                && FoodOnConveyorBelt.instance.IsOnConveyorBelt(food))
            {
                FoodOnConveyorBelt.instance.SetParent(food);
                
                FoodOnConveyorBelt.instance.foodQueue.AddFood(food);
                FoodOnConveyorBelt.instance.RemoveFoodOnConveyorBelt(food);
            }
            else if (currentSegmentIndex >= LevelController.instance.conveyorBelt.cachedPathPoints.Count)
            {
                transform.position = LevelController.instance.conveyorBelt.cachedPathPoints[0];

                currentSegmentIndex = 1;

                if (FoodOnConveyorBelt.instance.FoodLength > 0 && isEmpty)
                {
                    food = FoodOnConveyorBelt.instance.GetFood(this);

                    FoodOnConveyorBelt.instance.RemoveFood();
                    FoodOnConveyorBelt.instance.AddFoodOnConveyorBelt(food);

                    food.transform.SetParent(transform);
                }
            }
        }
    }
}
