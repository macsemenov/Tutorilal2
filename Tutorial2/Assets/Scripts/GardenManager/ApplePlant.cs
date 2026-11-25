using System.Collections;
using UnityEngine;

public class ApplePlant : Plant
{
    private IEnumerator Start()
    {
        yield return Grow(foliage, growTime);
        yield return SpawnFruits();
        yield return DropFruits();
    }
}
