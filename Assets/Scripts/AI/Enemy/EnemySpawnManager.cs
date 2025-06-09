using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EnemyPrefabs
{
    public Transform prefab;
    public UnitType type;
}
public class EnemySpawnManager : MonoBehaviour
{
    [Header("Enemy spawn settings")]
    public int maxEnemyQuantity = 1;
    public int maxEnemyQuantityOnMap = 5;

    [HideInInspector] public int currentEnemyQuantity = 0;
    [HideInInspector] public int wasEnemyCreated = 0;

    [HideInInspector] public Dictionary<UnitType, Transform> typeEnemyPairs;

    public EnemyPrefabs[] enemyPrefabs;
    public EnemySpawnPoint[] enemySpawnPoints;

    public bool allowSpawn = true;

    // Start is called before the first frame update
    void Start()
    {
        FindFirstObjectByType<UIStatisticManager>().SetEnemiesLeft(maxEnemyQuantity);

        typeEnemyPairs = new Dictionary<UnitType, Transform>();
        
        foreach(EnemyPrefabs enemyPrefab in enemyPrefabs)
        {
            typeEnemyPairs.Add(enemyPrefab.type, enemyPrefab.prefab);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!allowSpawn)
            return;

        for(int i = currentEnemyQuantity; i < maxEnemyQuantityOnMap; i++)
        {
            if (wasEnemyCreated >= maxEnemyQuantity)
            {
                break;
            }

            int index = Random.Range(0, enemySpawnPoints.Length);

            if (enemySpawnPoints[index].CheckInNear())
            {
                enemySpawnPoints[index].Spawn(typeEnemyPairs[enemySpawnPoints[index].type]);

                currentEnemyQuantity++;
                wasEnemyCreated++;
            }
        }
    }
}
