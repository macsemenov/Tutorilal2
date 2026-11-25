using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlantType { Apple, Tomato, Potato }

public class Plant : MonoBehaviour
{
    [Header("Plant")]
    public PlantType plantType;
    public float growTime = 2f;

    [Header("Fruits")]
    public int fruitCount = 5;
    public float fruitGrowTime = 0.5f;
    public float fruitInterval = 0.5f;
    public float ripenTime = 2f;
    public float dropInterval = 0.5f;
    public float fruitSize = 0.3f;
    public Vector3[] fruitPositions =
    {
        new(-0.6f, 1.8f, 0), new(0.6f, 1.9f, 0.1f),
        new(0.1f, 2.2f, -0.6f), new(-0.2f, 1.6f, 0.7f),
        new(0.7f, 1.55f, -0.45f), new(-0.7f, 2.2f, -0.35f)
    };
    public Color unripeColor = Color.green;
    public Color ripeColor = Color.red;
    public Color rottenColor = Color.black;

    [Header("Potato")]
    public float moundGrowTime = 3f;

    private Transform foliage;
    private readonly List<GardenFruit> fruits = new();
    private int landedFruits;

    private IEnumerator Start()
    {
        BuildPlant();
        yield return Grow(foliage, growTime, Vector3.one);

        if (plantType == PlantType.Potato)
        {
            Vector3 moundSize = new(1.7f, 0.65f, 1.7f);
            Transform mound = CreatePart(PrimitiveType.Sphere, "Soil Mound", transform,
                Vector3.zero, moundSize, new Color(0.4f, 0.27f, 0.14f));

            yield return Grow(mound, moundGrowTime, moundSize);
            Destroy(foliage.gameObject);
            yield break;
        }

        float height = plantType == PlantType.Tomato ? 0.55f : 1f;
        float size = plantType == PlantType.Tomato ? fruitSize * 0.8f : fruitSize;

        for (int i = 0; i < fruitCount; i++)
        {
            Transform shape = CreatePart(PrimitiveType.Sphere, "Fruit", foliage,
                fruitPositions[i % fruitPositions.Length] * height,
                Vector3.one * size, plantType == PlantType.Tomato ? unripeColor : ripeColor);

            GardenFruit fruit = shape.gameObject.AddComponent<GardenFruit>();
            fruit.plant = this;
            fruit.turnBlack = plantType == PlantType.Tomato;
            fruit.rottenColor = rottenColor;

            Rigidbody body = shape.gameObject.AddComponent<Rigidbody>();
            body.isKinematic = true;
            fruit.body = body;
            fruits.Add(fruit);

            yield return Grow(shape, fruitGrowTime, Vector3.one * size);
            yield return new WaitForSeconds(fruitInterval);
        }

        if (plantType == PlantType.Tomato)
        {
            float elapsed = 0f;
            while (elapsed < ripenTime)
            {
                elapsed += Time.deltaTime;
                Color color = Color.Lerp(unripeColor, ripeColor, elapsed / ripenTime);
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
        if (plantType == PlantType.Tomato && landedFruits == fruitCount)
            Destroy(foliage.gameObject);
    }

    private void BuildPlant()
    {
        foliage = new GameObject("Plant Visual").transform;
        foliage.SetParent(transform, false);

        bool apple = plantType == PlantType.Apple;
        float height = apple ? 1.15f : 0.5f;

        CreatePart(PrimitiveType.Cylinder, "Stem", foliage,
            new Vector3(0, height, 0),
            new Vector3(apple ? 0.25f : 0.12f, height, apple ? 0.25f : 0.12f),
            apple ? new Color(0.4f, 0.24f, 0.1f) : Color.green);

        CreatePart(PrimitiveType.Sphere, "Leaves", foliage,
            new Vector3(0, height * 2, 0),
            apple ? new Vector3(2.1f, 1.25f, 2.1f) : new Vector3(1.15f, 0.95f, 1.15f),
            new Color(0.15f, 0.5f, 0.18f));

        foliage.localScale = Vector3.zero;
    }

    private Transform CreatePart(PrimitiveType primitive, string name, Transform parent,
        Vector3 position, Vector3 size, Color color)
    {
        GameObject part = GameObject.CreatePrimitive(primitive);
        part.name = name;
        part.transform.SetParent(parent, false);
        part.transform.localPosition = position;
        part.transform.localScale = size;
        part.GetComponent<Renderer>().material.color = color;

        if (name != "Fruit")
            Destroy(part.GetComponent<Collider>());

        return part.transform;
    }

    private IEnumerator Grow(Transform item, float duration, Vector3 targetScale)
    {
        item.localScale = Vector3.zero;
        float time = 0f;
        while (time < duration)
        {
            time += Time.deltaTime;
            item.localScale = Vector3.Lerp(Vector3.zero, targetScale, time / duration);
            yield return null;
        }
        item.localScale = targetScale;
    }
}
