using UnityEngine;

public class IceBreak : MonoBehaviour
{
    public Rigidbody[] rbs;

    private void Start()
    {
        foreach (var rb in rbs)
        {
            rb.AddExplosionForce(150, transform.position, 150);
        }
    }
}
