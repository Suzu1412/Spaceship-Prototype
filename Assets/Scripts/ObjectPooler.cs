using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler : MonoBehaviour
{
    private static ObjectPooler instance;

    private GameObject projectileParentFolder;

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

    public void Awake()
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
