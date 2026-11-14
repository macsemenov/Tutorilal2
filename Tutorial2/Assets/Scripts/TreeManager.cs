using UnityEngine;

public class TreeManager : MonoBehaviour
{
    public GameObject[] fruits;
    public Transform[] spawnPoints;

    private int currentFruit;
    private int currentSpawnPoint;

    public void SpawnFruit()
    {
        if (fruits == null || fruits.Length == 0 ||
            spawnPoints == null || spawnPoints.Length == 0)
        {
            return;
        }

        if (currentSpawnPoint >= spawnPoints.Length)
        {
            print("Все точки спавна использованы.");
            return;
        }

        Transform point = spawnPoints[currentSpawnPoint];

        if (point == null || fruits[currentFruit] == null)
            return;

        Instantiate(fruits[currentFruit], point.position, point.rotation);

        currentSpawnPoint++;
        currentFruit++;

        if (currentFruit >= fruits.Length)
        {
            currentFruit = 0;
        }
    
    }
}