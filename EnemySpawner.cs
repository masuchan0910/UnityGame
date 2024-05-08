using System.Collections;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{

    [SerializeField]
    GameObject[] enemyPrefabs = { };
    [SerializeField]
    Transform target = null;
    [SerializeField]
    int maxSpawnCount = 50;
    [SerializeField, Min(0)]
    float spawnDelay = 0;
    [SerializeField, Min(0)]
    float spawnInterval = 3;
    [SerializeField]
    bool spawnOnStart = false;

    Transform thisTransform;
    WaitForSeconds spawnDelayWait;
    WaitForSeconds spawnWait;

    void Start()
    {
        thisTransform = transform;
        spawnDelayWait = new WaitForSeconds(spawnDelay);
        spawnWait = new WaitForSeconds(spawnInterval);

        if (spawnOnStart)
        {
            StartSpawn();
        }
    }

    public void StartSpawn()
    {
        StartCoroutine(nameof(SpawnTimer));
    }

    public void StopSpawn()
    {
        StopCoroutine(nameof(SpawnTimer));
    }

    IEnumerator SpawnTimer()
    {
        yield return spawnDelayWait;

        for (int i = 0; i < maxSpawnCount; i++)
        {
            EnemyController enemy = Instantiate(enemyPrefabs[Random.Range(0, enemyPrefabs.Length)], thisTransform.position, Quaternion.identity).GetComponent<EnemyController>();
            enemy.Target = target;

            yield return spawnWait;
        }
    }

}