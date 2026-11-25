using System.Collections;
using UnityEngine;

public class PotatoPlant : Plant
{
    [Header("Potato")]
    public Transform mound;
    public float moundGrowTime = 3f;

    private IEnumerator Start()
    {
        yield return Grow(foliage, growTime);

        Vector3 moundSize = mound.localScale;
        mound.localScale = Vector3.zero;
        mound.gameObject.SetActive(true);

        yield return Grow(mound, moundGrowTime, moundSize);
        foliage.gameObject.SetActive(false);
    }
}
