using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler : MonoBehaviour
{
    //Para que aparezca en el inspector tenemos que agregar la etiqueta de System.Serializable
    [System.Serializable]
    public class Pool
    {
        public string tag;
        public GameObject prefab;
        public int size;
        public bool shouldExpandPool = false;
        public bool isChild = true;
    }

    private static ObjectPooler instance;
    public Dictionary<string, List<GameObject>> poolDictionary = new Dictionary<string, List<GameObject>>();
    public List<Pool> pools = new List<Pool>();

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        foreach (Pool pool in pools)
        {
            CreatePool(pool);
        }
    }

    public static ObjectPooler Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject obj = new GameObject("ObjectPooler");
                obj.AddComponent<ObjectPooler>();
            }
            return instance;
        }
    }

    public void AddPool(Pool pool)
    {
        pools.Add(pool);
    }

    public void CreatePool(Pool pool)
    {
        if (!poolDictionary.ContainsKey(pool.tag))
        {
            List<GameObject> objectPool = new List<GameObject>();

            for (int i = 0; i < pool.size; i++)
            {
                GameObject obj = Instantiate(pool.prefab);
                obj.SetActive(false);
                objectPool.Add(obj);
                if (pool.isChild)
                    obj.transform.SetParent(this.transform);
            }

            poolDictionary.Add(pool.tag, objectPool);
        }
    }

    public GameObject SpawnFromPool(string tag, Vector3 position, Quaternion rotation)
    {
        if (!poolDictionary.ContainsKey(tag))
        {
            Debug.LogWarning("Pool with tag" + tag + " doesn't exist");
            return null;
        }

        if (poolDictionary[tag].Count > 0)
        {
            for (int i = 0; i < poolDictionary[tag].Count; i++)
            {
                if (!poolDictionary[tag][i].activeInHierarchy)
                {
                    GameObject objectToSpawn = poolDictionary[tag][i];

                    objectToSpawn.SetActive(true);
                    objectToSpawn.transform.position = position;
                    objectToSpawn.transform.rotation = rotation;

                    return objectToSpawn;
                }
            }
        }

        foreach (Pool pool in pools)
        {
            if (pool.tag.Equals(tag) && pool.shouldExpandPool)
            {
                GameObject objectToSpawn = Instantiate(pool.prefab);


                if (pool.isChild)
                    objectToSpawn.transform.SetParent(this.transform);

                poolDictionary[tag].Add(objectToSpawn);

                objectToSpawn.SetActive(true);
                objectToSpawn.transform.position = position;
                objectToSpawn.transform.rotation = rotation;

                return objectToSpawn;
            }
        }

        return null;
    }
}
