using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler : MonoBehaviour
{
    private static ObjectPooler instance;
    private GameObject projectileParentFolder;
    public List<GameObject> pooledObjects;
    public List<ObjectPoolItem> itemsToPool;

    [System.Serializable]
    public class ObjectPoolItem
    {
        public GameObject objectToPool;
        public int amountToPool;
        public bool shouldExpand = true;
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

    private GameObject currentItem;

    void Awake()
    {
        if(instance == null)
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
        pooledObjects = new List<GameObject>();

        foreach(ObjectPoolItem item in itemsToPool)
        {
            for (int i = 0; i < item.amountToPool; i++)
            {
                GameObject obj = Instantiate(item.objectToPool);
                obj.SetActive(false);
                pooledObjects.Add(obj);
            }
        }
        
    }

    public void CreatePool(WeaponType weapon, List<GameObject> currentPool, GameObject projectileParentFolder)
    {
        for(int i=0; i < weapon.amountToPool; i++)
        {
            currentItem = Instantiate(weapon.projectile);

            currentItem.SetActive(false);
            currentPool.Add(currentItem);
            currentItem.transform.SetParent(projectileParentFolder.transform);
        }

        projectileParentFolder.name = weapon.name;
    }

    public GameObject GetPooledObject(string tag)
    {
        for (int i=0; i < pooledObjects.Count; i++)
        {
            if (!pooledObjects[i].activeInHierarchy && pooledObjects[i].tag == tag)
            {
                return pooledObjects[i];
            }
        }

        foreach(ObjectPoolItem item in itemsToPool)
        {
            if (item.objectToPool.tag == tag && item.shouldExpand)
            {
                GameObject obj = Instantiate(item.objectToPool);
                obj.SetActive(false);
                pooledObjects.Add(obj);
                return obj;
            }
        }
        return null;
    }

    public GameObject GetObject(List<GameObject> currentPool)
    {
        for(int i=0; i<currentPool.Count; i++)
        {
            if (!currentPool[i].activeInHierarchy)
            {
                return currentPool[i];
            }
        }

        return null;
    }
}
