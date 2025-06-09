using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    public int wave = 1;

    public float enemyDamageMultiplier;
    public float enemyCountMultiplier;

    public float nextWaveDelay = 5;
    public TextMeshProUGUI disclaimer;

    private EnemySpawnManager spawnManager;

    void Start()
    {
        FindFirstObjectByType<UIStatisticManager>().SetWave(wave);
        spawnManager = FindFirstObjectByType<EnemySpawnManager>();
        disclaimer.text = "Хвиля 1";

        Invoke("ShowDisclaimer", 1f);
    }

    public void ShowDisclaimer()
    {
        FindFirstObjectByType<InGameMenu>().GetComponent<Animator>().Play("Show");
    }

    // Update is called once per frame
    void Update()
    {
        if (spawnManager.maxEnemyQuantity == spawnManager.wasEnemyCreated && spawnManager.currentEnemyQuantity == 0)
        {
            disclaimer.text = "Хвиля " + wave;
            ShowDisclaimer();

            spawnManager.allowSpawn = false;
            spawnManager.wasEnemyCreated = 0;
            Invoke("NextWave", nextWaveDelay);
        }
    }

    public void NextWave()
    {
        spawnManager.maxEnemyQuantity = (int)(spawnManager.maxEnemyQuantity * enemyCountMultiplier);
        foreach(var enemyPrefab in spawnManager.enemyPrefabs)
        {
            enemyPrefab.prefab.GetComponent<ShootProjectTiles>().bullet.GetComponent<Bullet>().damageAmount *= enemyDamageMultiplier;
        }

        wave++;
        spawnManager.allowSpawn = true;

        FindFirstObjectByType<UIStatisticManager>().SetWave(wave);
        FindFirstObjectByType<UIStatisticManager>().SetEnemiesLeft(spawnManager.maxEnemyQuantity);
    }
}
