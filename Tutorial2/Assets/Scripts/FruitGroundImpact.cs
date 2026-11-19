using UnityEngine;

public class FruitGroundImpact : MonoBehaviour
{
    private Collider ground;
    private AudioClip clip;
    private bool played;

    public void Initialize(Collider groundCollider, AudioClip impactClip)
    {
        ground = groundCollider;
        clip = impactClip;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (played || collision.collider != ground) return;

        played = true;
        AudioSource.PlayClipAtPoint(clip, transform.position);
    }
}
