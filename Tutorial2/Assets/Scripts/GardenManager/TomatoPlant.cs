using System.Collections;
using UnityEngine;

public class TomatoPlant : Plant
{
    [Header("Tomatoes")]
    public float ripenTime = 15f;
    public Color unripeColor = Color.green;
    public Color ripeColor = Color.red;
    public Color rottenColor = Color.black;

    private int landedFruits;

    private IEnumerator Start()
    {
        yield return Grow(foliage, growTime);
        yield return SpawnFruits(true, rottenColor);

        // Each fruit ripens from its own creation time.
        while (!fruits[fruits.Count - 1].ripe)
            yield return null;

        yield return DropFruits();
    }

    protected override void OnFruitSpawned(GardenFruit fruit)
    {
        fruit.StartRipening(ripenTime, unripeColor, ripeColor);
    }

    public override void FruitLanded()
    {
        landedFruits++;
        if (landedFruits == fruits.Count)
            foliage.gameObject.SetActive(false);
    }
}
