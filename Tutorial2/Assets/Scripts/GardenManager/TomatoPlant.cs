using System.Collections;
using UnityEngine;

public class TomatoPlant : Plant
{
    [Header("Tomatoes")]
    public float ripenTime = 2f;
    public Color unripeColor = Color.green;
    public Color ripeColor = Color.red;
    public Color rottenColor = Color.black;

    private int landedFruits;

    private IEnumerator Start()
    {
        yield return Grow(foliage, growTime);
        yield return SpawnFruits(true, rottenColor);

        float time = 0;
        while (time < ripenTime)
        {
            time += Time.deltaTime;
            Color color = Color.Lerp(unripeColor, ripeColor, time / ripenTime);

            foreach (GardenFruit fruit in fruits)
                fruit.GetComponent<Renderer>().material.color = color;

            yield return null;
        }

        yield return DropFruits();
    }

    public override void FruitLanded()
    {
        landedFruits++;
        if (landedFruits == fruits.Count)
            foliage.gameObject.SetActive(false);
    }
}
