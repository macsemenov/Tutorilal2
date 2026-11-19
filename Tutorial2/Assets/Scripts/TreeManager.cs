using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeManager : MonoBehaviour
{
    public GameObject[] fruits;
    public Transform[] spawnPoints;
    public Collider groundCollider;
    public AudioClip groundImpactSound;
    public float dropInterval = 0.5f;

    private readonly List<Rigidbody> spawnedFruits = new();
    private int currentFruit;
    private int currentSpawnPoint;
    private bool drop;
    private bool isDropping;

    public void SpawnFruit()
    {
        if (isDropping) return;

        if (drop)
        {
            DropFruit();
            return;
        }

        Transform point = spawnPoints[currentSpawnPoint];
        GameObject fruit = Instantiate(fruits[currentFruit], point.position, point.rotation, point);

        Rigidbody body = fruit.GetComponent<Rigidbody>();
        body.isKinematic = true;
        spawnedFruits.Add(body);

        FruitGroundImpact impact = fruit.AddComponent<FruitGroundImpact>();
        impact.Initialize(groundCollider, groundImpactSound);

        currentFruit = (currentFruit + 1) % fruits.Length;
        currentSpawnPoint++;

        if (currentSpawnPoint == spawnPoints.Length)
            drop = true;
    }

    public void DropFruit()
    {
        StartCoroutine(DropSequence());
    }

    private IEnumerator DropSequence()
    {
        isDropping = true;

        foreach (Rigidbody body in spawnedFruits)
        {
            body.transform.SetParent(null, true);
            body.isKinematic = false;
            yield return new WaitForSeconds(dropInterval);
        }

        spawnedFruits.Clear();
        currentFruit = 0;
        currentSpawnPoint = 0;
        drop = false;
        isDropping = false;
    }
}
