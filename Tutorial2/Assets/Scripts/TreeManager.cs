using UnityEngine;

using System.Diagnostics;
using NUnit.Framework;

public class TreeManager : MonoBehaviour
{
    public GameObject[] fruits;
    public Transform[] spawnPoints;

    private int currentFruit = 0;
    private int currentSpawnPoint;

    

    private void Start()
    {
        //SpawnFruit();
    }

    public void SpawnFruit()
    {
        if (fruits == null || fruits.Length == 0 ||
            spawnPoints == null || spawnPoints.Length == 0)
        {
            return;
        }

        if (currentSpawnPoint >= spawnPoints.Length)
        {
            print("All spawn points had used");
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

    void DropFruit()
    {

    }
}