using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerController : MonoBehaviour
{
    LayerMask mask;

    GameObject hitObj;
    FoodTray foodTray;

    bool isDrag;

    private void Awake()
    {
        mask = LayerMask.GetMask("FoodTray", "FoodQueue");
    }

    private void Update()
    {
        if (LevelController.instance.gameState == LevelController.GameState.Win
         || LevelController.instance.gameState == LevelController.GameState.Lose
         || LevelController.instance.gameState == LevelController.GameState.Pause) return;

        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            isDrag = true;
        }

        if (Input.GetMouseButtonUp(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            isDrag = false;

            if (hitObj != null && foodTray != null)
            {
                if (foodTray.IsHighestLayer() && !foodTray.IsFreeze())
                {
                    LevelController.instance.MinusMove();
                    foodTray.Toss();

                    AudioController.instance.PlaySoundNVibrate(AudioController.instance.onClickTray, 50);

                    UIController.instance.uITutorial.HideTut();
                }
            }

            Ray ray = GameController.instance.cameraMain.ScreenPointToRay(Input.mousePosition);

            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 100, mask))
            {
                if (hit.collider.gameObject.layer == LayerMask.NameToLayer("FoodQueue"))
                {
                    LevelController.instance.MinusMove();
                    FoodOnConveyorBelt.instance.foodQueue.OnConveyorBelt(hit.collider.gameObject);
                }
            }

            hitObj = null;
        }

        if (isDrag)
        {
            Ray ray = GameController.instance.cameraMain.ScreenPointToRay(Input.mousePosition);

            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 100, mask))
            {
                if (hit.collider.gameObject.layer == LayerMask.NameToLayer("FoodTray"))
                {
                    if (hitObj != null && hitObj == hit.collider.gameObject) return;

                    hitObj = hit.collider.gameObject;

                    foodTray = hitObj.GetComponent<FoodTray>();

                    if (foodTray != null)
                    {
                        if (foodTray.IsHighestLayer() && !foodTray.IsFreeze())
                        {
                            foodTray.Holding();
                        }
                    }
                    else Debug.LogError("Null");
                }
            }
            else
            {
                hitObj = null;
            }
        }
    }
}
