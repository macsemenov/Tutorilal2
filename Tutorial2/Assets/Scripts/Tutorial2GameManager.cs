using System.Collections.Generic;
using UnityEngine;
/// <summary>Instantiates bushes at ground markers and fruit at branch markers in order.</summary>
public sealed class Tutorial2GameManager : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private GameObject[] bushPrefabs;
    [SerializeField] private GameObject[] fruitPrefabs;
    [Header("Ground spawn points")]
    [SerializeField] private Transform[] bushSpawnPoints;
    [Header("Placement")]
    [SerializeField, Min(0.05f)] private float fruitOffset = 0.12f;
    private readonly List<GameObject> spawnedBushes = new List<GameObject>();
    private int nextFruitPoint;
    private int nextFruitType;

    public void SpawnBush()
    {
        if (bushPrefabs == null || bushPrefabs.Length == 0)
        {
            Debug.LogWarning("Assign at least one bush prefab to the GameManager.", this);
            return;
        }
        Transform point = FindFreeBushPoint();
        if (point == null)
        {
            Debug.LogWarning("There are no free bush spawn points in the scene.", this);
            return;
        }
        GameObject prefab = bushPrefabs[Random.Range(0, bushPrefabs.Length)];
        GameObject bush = Instantiate(prefab, point.position, point.rotation);
        bush.name = prefab.name;
        spawnedBushes.Add(bush);
        Debug.Log("Spawned " + bush.name + " at " + point.name, bush);
    }

    public void SpawnFruit()
    {
        if (fruitPrefabs == null || fruitPrefabs.Length == 0)
        {
            Debug.LogWarning("Assign at least one fruit prefab to the GameManager.", this);
            return;
        }
        List<SpawnPoint> branchPoints = CollectBranchPoints();
        if (branchPoints.Count == 0)
        {
            Debug.Log("Spawn a bush first. Its branch markers are the fruit spawn locations.", this);
            return;
        }
        SpawnPoint point = branchPoints[nextFruitPoint % branchPoints.Count];
        GameObject prefab = fruitPrefabs[nextFruitType % fruitPrefabs.Length];
        Vector3 position = point.transform.position + point.transform.up * fruitOffset;
        GameObject fruit = Instantiate(prefab, position, point.transform.rotation);
        fruit.name = prefab.name;
        nextFruitPoint = (nextFruitPoint + 1) % branchPoints.Count;
        nextFruitType = (nextFruitType + 1) % fruitPrefabs.Length;
    }

    private Transform FindFreeBushPoint()
    {
        if (bushSpawnPoints == null || bushSpawnPoints.Length == 0) return null;
        List<Transform> freePoints = new List<Transform>();
        foreach (Transform point in bushSpawnPoints)
        {
            if (point == null) continue;
            bool occupied = false;
            foreach (GameObject bush in spawnedBushes)
            {
                if (bush != null && Vector3.Distance(bush.transform.position, point.position) < 0.25f)
                {
                    occupied = true;
                    break;
                }
            }
            if (!occupied) freePoints.Add(point);
        }
        if (freePoints.Count == 0) return null;
        return freePoints[Random.Range(0, freePoints.Count)];
    }

    private List<SpawnPoint> CollectBranchPoints()
    {
        List<SpawnPoint> result = new List<SpawnPoint>();
        spawnedBushes.RemoveAll(bush => bush == null);
        foreach (GameObject bush in spawnedBushes)
        {
            foreach (SpawnPoint point in bush.GetComponentsInChildren<SpawnPoint>())
            {
                if (point != null && point.transform != bush.transform) result.Add(point);
            }
        }
        return result;
    }
}