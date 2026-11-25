using UnityEngine;

public class GardenFruit : MonoBehaviour
{
    public Plant plant;
    public Rigidbody body;
    public bool turnBlack;
    public Color rottenColor = Color.black;
    public AudioClip impactSound;

    private bool landed;

    private void OnCollisionEnter(Collision collision)
    {
        if (landed || !collision.gameObject.CompareTag("Ground")) return;

        landed = true;
        AudioSource.PlayClipAtPoint(impactSound, transform.position);
        if (turnBlack)
            GetComponent<Renderer>().material.color = rottenColor;

        plant.FruitLanded();
    }
}
