using UnityEngine;

public class FruitGroundImpact : MonoBehaviour
{



    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Ground"))
        {
            TreeManager.instance.PlaySound();
        }
    }
}
