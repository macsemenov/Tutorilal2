using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlantType { Apple, Tomato, Potato }

public class Plant : MonoBehaviour
{
    [Header("Plant")]
    public PlantType plantType;
    public float growTime = 2f;
    public Transform foliage;

    [Header("Fruits")]
    public GameObject fruitPrefab;
    public Transform[] spawnPoints;
    public float fruitGrowTime = 0.5f;
    public float fruitInterval = 0.5f;
    public float ripenTime = 2f;
    public float dropInterval = 0.5f;
    public Color unripeColor = Color.green;
    public Color ripeColor = Color.red;
    public Color rottenColor = Color.black;

    [Header("Potato")]
    public Transform mound;
    public float moundGrowTime = 3f;

    private readonly List<GardenFruit> fruits = new();
    private int landedFruits;

    private IEnumerator Start()
    {
        yield return Grow(foliage, growTime);

        if (plantType == PlantType.Potato)
        {
            Vector3 moundSize = mound.localScale;
            mound.localScale = Vector3.zero;
            mound.gameObject.SetActive(true);
            yield return Grow(mound, moundGrowTime, moundSize);
            foliage.gameObject.SetActive(false);
            yield break;
        }

        foreach (Transform point in spawnPoints)
        {
            GameObject obj = Instantiate(fruitPrefab, point.position, point.rotation, point);
            GardenFruit fruit = obj.GetComponent<GardenFruit>();
            fruit.plant = this;
            fruit.turnBlack = plantType == PlantType.Tomato;
            fruit.rottenColor = rottenColor;
            fruit.body.isKinematic = true;
            fruits.Add(fruit);

            yield return Grow(obj.transform, fruitGrowTime);
            yield return new WaitForSeconds(fruitInterval);
        }

        if (plantType == PlantType.Tomato)
        {
            float time = 0;
            while (time < ripenTime)
            {
                time += Time.deltaTime;
                Color color = Color.Lerp(unripeColor, ripeColor, time / ripenTime);
                foreach (GardenFruit fruit in fruits)
                    fruit.GetComponent<Renderer>().material.color = color;
                yield return null;
            }
        }

        foreach (GardenFruit fruit in fruits)
        {
            fruit.transform.SetParent(null, true);
            fruit.body.isKinematic = false;
            yield return new WaitForSeconds(dropInterval);
        }
    }

    public void FruitLanded()
    {
        landedFruits++;
        if (plantType == PlantType.Tomato && landedFruits == fruits.Count)
            foliage.gameObject.SetActive(false);
    }

    private IEnumerator Grow(Transform part, float duration)
    {
        yield return Grow(part, duration, part.localScale);
    }

    private IEnumerator Grow(Transform part, float duration, Vector3 targetScale)
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
}
