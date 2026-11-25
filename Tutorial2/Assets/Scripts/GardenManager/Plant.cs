using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plant : MonoBehaviour
{
    [Header("Growth")]
    public Transform foliage;
    public float growTime = 2f;

    [Header("Fruits")]
    public GameObject fruitPrefab;
    public Transform[] spawnPoints;
    public float fruitGrowTime = 0.5f;
    public float fruitInterval = 0.5f;
    public float dropInterval = 0.5f;

    protected List<GardenFruit> fruits = new();

    protected IEnumerator Grow(Transform part, float duration)
    {
        yield return Grow(part, duration, part.localScale);
    }

    protected IEnumerator Grow(Transform part, float duration, Vector3 targetScale)
    {
        part.localScale = Vector3.zero;
        float time = 0;

        while (time < duration)
        {
            time += Time.deltaTime;
            part.localScale = Vector3.Lerp(Vector3.zero, targetScale, time / duration);
            yield return null;
        }

        part.localScale = targetScale;
    }

    protected IEnumerator SpawnFruits(bool turnBlack = false, Color rottenColor = default)
    {
        foreach (Transform point in spawnPoints)
        {
            GameObject obj = Instantiate(fruitPrefab, point.position, point.rotation, point);
            GardenFruit fruit = obj.GetComponent<GardenFruit>();
            fruit.plant = this;
            fruit.turnBlack = turnBlack;
            fruit.rottenColor = rottenColor;
            fruit.body.isKinematic = true;
            fruits.Add(fruit);

            yield return Grow(obj.transform, fruitGrowTime);
            yield return new WaitForSeconds(fruitInterval);
        }
    }

    protected IEnumerator DropFruits()
    {
        foreach (GardenFruit fruit in fruits)
        {
            fruit.transform.SetParent(null, true);
            fruit.body.isKinematic = false;
            yield return new WaitForSeconds(dropInterval);
        }
    }

    public virtual void FruitLanded() { }
}
