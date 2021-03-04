using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    public List<WaveObject> waves = new List<WaveObject>();
    private int _currentWave;
    private bool _canSpawnWaves = true;
    public bool canSpawnWaves { get { return _canSpawnWaves; } }
    private ObjectPooler _objectPooler;
    private GameManager _manager;

    private void Awake()
    {
        _manager = GameObject.FindObjectOfType<GameManager>();
        _objectPooler = ObjectPooler.Instance;
        _currentWave = 0;
        _canSpawnWaves = true;
        SpawnEnemies();
    }

    void SpawnEnemies()
    {
        List<ObjectPooler.Pool> itemList = new List<ObjectPooler.Pool>();

        for (int i=0; i < waves.Count; i++)
        {
            for (int j=0; j < waves[i].theWave.enemyList.Count; j++)
            {
                bool found = false;

                for (int k = 0; k < itemList.Count; k++)
                {
                    if (itemList[k].tag.Equals(waves[i].theWave.enemyList[j].enemy.name))
                    {
                        if (itemList[k].size < waves[i].theWave.enemyList.Count)
                        {
                            itemList[k].size++;
                        }
                        found = true;
                    }
                }

                if (!found)
                {
                    ObjectPooler.Pool item = new ObjectPooler.Pool
                    {
                        prefab = waves[i].theWave.enemyList[j].enemy.gameObject,
                        shouldExpandPool = true,
                        size = 1,
                        tag = waves[i].theWave.enemyList[j].enemy.name,
                        isChild = true
                    };
                    itemList.Add(item);
                }
            }
        }

        foreach(ObjectPooler.Pool item in itemList)
        {
            _objectPooler.CreatePool(item);
        }
    }

    private void Update()
    {
        waves[_currentWave].timeToSpawn -= Time.deltaTime;
        if (_manager.State == GameState.Playing && canSpawnWaves && waves[_currentWave].timeToSpawn <= 0f)
        {
            for (int i = 0; i < waves[_currentWave].theWave.enemyList.Count; i++)
            {
                waves[_currentWave].theWave.enemyList[i].spawnTimeOffset -= Time.deltaTime;

                if (waves[_currentWave].theWave.enemyList[i].spawnTimeOffset <= 0f && !waves[_currentWave].theWave.enemyList[i].alreadySpawned)
                {
                    if (waves[_currentWave].theWave.enemyList[i].wayPoint != null)
                    {
                        GameObject obj = _objectPooler.SpawnFromPool(waves[_currentWave].theWave.enemyList[i].enemy.name, waves[_currentWave].theWave.enemyList[i].wayPoint.position, Quaternion.identity);
                        obj.GetComponent<EnemyController>().SetWaypoint(waves[_currentWave].theWave.enemyList[i].wayPoint);
                    }
                    else
                    {
                        if (waves[_currentWave].theWave.enemyList[i].randomXPosition)
                        {
                            float randomXPosition = Random.Range(waves[_currentWave].theWave.enemyList[i].minXPosition, waves[_currentWave].theWave.enemyList[i].maxXPosition);
                            _objectPooler.SpawnFromPool(waves[_currentWave].theWave.enemyList[i].enemy.name, new Vector3(randomXPosition, 8f, 0f), Quaternion.identity);
                        }
                        else
                        {
                            _objectPooler.SpawnFromPool(waves[_currentWave].theWave.enemyList[i].enemy.name, new Vector3(waves[_currentWave].theWave.enemyList[i].spawnXPosition, 8f, 0f),
                                Quaternion.identity);
                        }
                    }

                    waves[_currentWave].theWave.enemyList[i].enemy.gameObject.SetActive(true);
                    waves[_currentWave].enemySpawned++;
                    waves[_currentWave].theWave.enemyList[i].alreadySpawned = true;
                }
            }

            if (_currentWave < waves.Count - 1 && waves[_currentWave].enemySpawned == waves[_currentWave].theWave.enemyList.Count)
            {
                _currentWave++;
            }
            else if (waves[_currentWave].enemySpawned == waves[_currentWave].theWave.enemyList.Count)
            {
                _canSpawnWaves = false;
            }

            if (_currentWave == waves.Count - 1 && canSpawnWaves == false)
            {
                _manager.LastWave();
            }
        }

        //Acelera la aparici?n de la pr?xima oleada si todos est?n muertos
        if (_currentWave - 1 >= 0)
        {
            if (waves[_currentWave - 1].enemySpawned == waves[_currentWave - 1].theWave.enemyList.Count && _manager.GetEnemyCount() == 0)
            {
                waves[_currentWave].timeToSpawn = 0f;
            }
        }
    }
}

[System.Serializable]
public class WaveObject
{
    public float timeToSpawn;
    public EnemyWave theWave;
    [HideInInspector] public int enemySpawned = 0;
}

[System.Serializable]
public class EnemyWave
{
    public List<EnemySpawn> enemyList = new List<EnemySpawn>();

    [System.Serializable]
    public class EnemySpawn
    {
        public EnemyController enemy;
        [Tooltip("Wait time before appear")] public float spawnTimeOffset;
        
        [Header("Waypoint Position")]
        [Tooltip("Only if the enemy follows waypoint. If empty it will use the bottom options")]public Transform wayPoint;

        [Header("Spawn Position if it doesn't follow waypoint")]
        public bool randomXPosition;
        [HideInInspector] public float minXPosition = -2.5f;
        [HideInInspector] public float maxXPosition = 2.5f;
        [Tooltip("If random X Position Unmarked and no waypoint configured")] [Range(-2.5f, 2.5f)] public float spawnXPosition;
        [HideInInspector] public bool alreadySpawned;
    }
}