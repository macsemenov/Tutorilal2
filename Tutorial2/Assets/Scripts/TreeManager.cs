using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeManager : MonoBehaviour
{
    public GameObject[] fruits;
    public Transform[] spawnPoints;

    [Header("Dropping")]
    [Min(0f)] public float dropInterval = 0.5f;
    public Collider groundCollider;
    public AudioClip groundImpactSound;

    private readonly List<Rigidbody> spawnedFruits = new List<Rigidbody>();
    private int currentFruit;
    private int currentSpawnPoint;

    private bool drop;
    private bool isDropping;

    public void SpawnFruit()
    {
        // После заполнения куста та же кнопка запускает сброс.
        if (drop)
        {
            DropFruit();
            return;
        }

        if (fruits == null || fruits.Length == 0 ||
            spawnPoints == null || spawnPoints.Length == 0)
            return;

        if (currentSpawnPoint >= spawnPoints.Length)
        {
            drop = true;
            DropFruit();
            return;
        }

        Transform point = spawnPoints[currentSpawnPoint];
        GameObject prefab = fruits[currentFruit];

        if (point == null || prefab == null)
        {
            Debug.LogWarning("Префаб фрукта или точка спавна не назначены.", this);
            return;
        }

        // Объект становится дочерним к конкретной точке появления.
        GameObject fruit = Instantiate(prefab, point.position, point.rotation, point);

        Rigidbody body = fruit.GetComponent<Rigidbody>();
        if (body == null)
        {
            Debug.LogWarning("На префабе фрукта нужен Rigidbody.", fruit);
            Destroy(fruit);
            return;
        }

        body.isKinematic = true;
        spawnedFruits.Add(body);

        // Звук есть только у тех фруктов, которым назначен клип.
        if (groundImpactSound != null && groundCollider != null)
        {
            FruitGroundImpact impact = fruit.GetComponent<FruitGroundImpact>();
            if (impact == null)
                impact = fruit.AddComponent<FruitGroundImpact>();

            impact.Initialize(groundCollider, groundImpactSound);
        }

        currentSpawnPoint++;
        currentFruit = (currentFruit + 1) % fruits.Length;

        // После заполнения всех точек следующая кнопка уже сбрасывает плоды.
        if (currentSpawnPoint >= spawnPoints.Length)
            drop = true;
    }

    public void DropFruit()
    {
        if (!drop || isDropping)
            return;

        StartCoroutine(DropFruitsSequentially());
    }

    private IEnumerator DropFruitsSequentially()
    {
        isDropping = true;

        foreach (Rigidbody body in spawnedFruits)
        {
            if (body != null)
            {
                // Отвязываем от ветки: физика больше не зависит от Transform куста.
                body.transform.SetParent(null, true);
                body.isKinematic = false;
            }

            yield return new WaitForSeconds(dropInterval);
        }

        spawnedFruits.Clear();
        currentSpawnPoint = 0;
        currentFruit = 0;
        drop = false;
        isDropping = false;
    }
}
