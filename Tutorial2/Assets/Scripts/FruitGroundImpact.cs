using UnityEngine;

public class FruitGroundImpact : MonoBehaviour
{
    public bool played;
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Ground")&&!played)
        {
            TreeManager.instance.PlaySound();
            played = true;
        }
    }
}
