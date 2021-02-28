using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    public List<WaveObject> waves = new List<WaveObject>();
    private int _currentWave;
    private bool _canSPawnWaves = true;
    public bool canSpawnWaves { get { return _canSPawnWaves; } }
    public ObjectPooler objectPooler;
    private GameManager _manager;

    private void Awake()
    {
        _manager = GameObject.FindObjectOfType<GameManager>();
        objectPooler = ObjectPooler.Instance;
        _currentWave = 0;
        _canSPawnWaves = true;
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
                        itemList[k].size++;
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
            objectPooler.AddPool(item);
        }
    }

    private void Update()
    {
        waves[_currentWave].timeToSpawn -= Time.deltaTime;
        if (_manager.state == GameState.Playing && _canSPawnWaves && waves[_currentWave].timeToSpawn <= 0f)
        {
            for (int i = 0; i < waves[_currentWave].theWave.enemyList.Count; i++)
            {
                waves[_currentWave].theWave.enemyList[i].spawnTimeOffset -= Time.deltaTime;

                if (waves[_currentWave].theWave.enemyList[i].spawnTimeOffset <= 0)
                {
                    if (waves[_currentWave].theWave.enemyList[i].wayPoint != null)
                    {
                        //Assign Waypoint and position itself to the first point
                        //waves[_currentWave].theWave.enemyList[i].enemy.SetWaypoint(waves[_currentWave].theWave.enemyList[i].wayPoint);
                    }
                    else
                    {
                        if (waves[_currentWave].theWave.enemyList[i].randomXPosition)
                        {
                            float randomXPosition = Random.Range(waves[_currentWave].theWave.enemyList[i].minXPosition, waves[_currentWave].theWave.enemyList[i].maxXPosition);
                            Debug.Log(waves[_currentWave].theWave.enemyList[i].minXPosition);
                            Debug.Log(waves[_currentWave].theWave.enemyList[i].maxXPosition);
                            objectPooler.SpawnFromPool(waves[_currentWave].theWave.enemyList[i].enemy.name, new Vector3(randomXPosition, 8f, 0f), Quaternion.identity);
                        }
                        else
                        {
                            waves[_currentWave].theWave.enemyList[i].enemy.transform.position = new Vector2(waves[_currentWave].theWave.enemyList[i].spawnXPosition, 8f);
                        }
                    }

                    waves[_currentWave].theWave.enemyList[i].enemy.gameObject.SetActive(true);
                    waves[_currentWave].enemySpawned++;
                }
            }

            if (_currentWave < waves.Count - 1 && waves[_currentWave].enemySpawned == waves[_currentWave].theWave.enemyList.Count)
            {
                _currentWave++;
            }
            else
            {
                _canSPawnWaves = false;
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
        [Tooltip("Only if the enemy follows waypoint. Else use the bottom options")]public Transform wayPoint;

        [Header("Spawn Position if it doesn't follow waypoint")]
        public bool randomXPosition;
        [HideInInspector] public float minXPosition = -2.5f;
        [HideInInspector] public float maxXPosition = 2.5f;
        [Tooltip("If random X Position Unmarked and no waypoint configured")] [Range(-2.5f, 2.5f)] public float spawnXPosition;
    }
}