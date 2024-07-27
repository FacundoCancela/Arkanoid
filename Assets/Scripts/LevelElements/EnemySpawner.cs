using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : CustomMonoBehaviour
{
    private PoolableFactory enemyFactory;
    [SerializeField] private Enemy enemyPrefab;
    [SerializeField] private ObjectPool enemyPool;

    private float spawnTimer = 0.0f;
    [SerializeField] private float secondsToSpawnNewEnemy = 8.0f;

    public override void CustomAwake()
    {
        enemyFactory = new PoolableFactory(enemyPool, enemyPrefab, enemyPool.PoolMaxSize);

    }
    public override void CustomUpdate()
    {
        spawnTimer += Time.deltaTime;

        if (spawnTimer >= secondsToSpawnNewEnemy)
        {
            enemyFactory.TryCreateObject();
            spawnTimer = 0.0f;
        }
    }
}
