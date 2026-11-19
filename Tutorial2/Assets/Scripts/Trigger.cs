using UnityEngine;

public class Trigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Fruit"))
        {
            TreeManager.instance.Count();
        }
    }
}
