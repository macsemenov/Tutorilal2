using UnityEngine;

public class TreeManager : MonoBehaviour
{
    public GameObject[] fruits;
    public Transform[] spawnPoints;
    int currentFuit;
    int currentSpawnPoint;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    public void SpawnFruit()
    {
        if(currentSpawnPoint<spawnPoints.Length)
        {
            Instantiate(fruits[currentFuit], spawnPoints[currentSpawnPoint].position, spawnPoints[currentSpawnPoint].rotation);

            if (currentFuit < fruits.Length)
            {
                currentFuit++;
            }
            else
            {
                currentFuit = 0;
            }

            currentSpawnPoint++;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
