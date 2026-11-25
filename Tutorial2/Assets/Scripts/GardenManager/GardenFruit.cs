using System.Collections;
using UnityEngine;

public class GardenFruit : MonoBehaviour
{
    public Plant plant;
    public Rigidbody body;
    public bool turnBlack;
    public Color rottenColor = Color.black;
    public AudioClip impactSound;
    public AudioSource audioSource;

    public bool ripe;
    private bool landed;

    public void StartRipening(float duration, Color from, Color to)
    {
        StartCoroutine(Ripen(duration, from, to));
    }

    private IEnumerator Ripen(float duration, Color from, Color to)
    {
        Renderer renderer = GetComponent<Renderer>();
        float time = 0f;
        renderer.material.color = from;

        while (time < duration)
        {
            time += Time.deltaTime;
            renderer.material.color = Color.Lerp(from, to, time / duration);
            yield return null;
        }

        renderer.material.color = to;
        ripe = true;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (landed || !collision.gameObject.CompareTag("Ground")) return;

        landed = true;
        audioSource.PlayOneShot(impactSound);
        if (turnBlack)
            GetComponent<Renderer>().material.color = rottenColor;

        plant.FruitLanded();
    }
}
