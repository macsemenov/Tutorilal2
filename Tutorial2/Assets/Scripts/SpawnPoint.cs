using UnityEngine;
/// <summary>Marker used for bush placement on the ground and fruit placement on branches.</summary>
public sealed class SpawnPoint : MonoBehaviour
{
    [Tooltip("Optional readable name for students inspecting the Hierarchy.")]
    public string pointName = "Spawn Point";
    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(1f, 0.78f, 0.18f, 1f);
        Gizmos.DrawWireSphere(transform.position, 0.12f);
    }
}