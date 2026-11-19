using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class FruitGroundImpact : MonoBehaviour
{
    [SerializeField] private AudioClip impactClip;
    [SerializeField] private Collider groundCollider;

    private bool hasPlayed;

    public void Initialize(Collider ground, AudioClip clip)
    {
        groundCollider = ground;
        impactClip = clip;
        hasPlayed = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (hasPlayed || impactClip == null || groundCollider == null)
            return;

        // Звук только при первом ударе о Plane, а не о соседний фрукт.
        if (collision.collider != groundCollider)
            return;

        hasPlayed = true;
        AudioSource.PlayClipAtPoint(impactClip, transform.position);
    }
}
