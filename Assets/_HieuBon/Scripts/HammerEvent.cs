using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HammerEvent : MonoBehaviour
{
    public void AfterDam()
    {
        LevelController.instance.AfterHammer();
    }
}
