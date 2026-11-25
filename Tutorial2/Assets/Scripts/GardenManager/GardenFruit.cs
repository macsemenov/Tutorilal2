using UnityEngine;

public class GardenFruit : MonoBehaviour
{
    public Plant plant;
    public Rigidbody body;
    public bool turnBlack;

    private bool landed;

    private void OnCollisionEnter(Collision collision)
    {
        if (landed || !collision.gameObject.CompareTag("Ground")) return;

        landed = true;
        if (turnBlack)
            GetComponent<Renderer>().material.color = Color.black;

        plant.FruitLanded();
    }
}
