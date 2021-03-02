using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnItem : MonoBehaviour
{
    public ItemPickUp[] items;
    Item itemSpawned;

    private float totalSpawnWeight = 0;
    private float whichToSpawn = 0;
    private int chosen = 0;

    private void Start()
    {
        foreach(ItemPickUp item in items)
        {
            //totalSpawnWeight += item.spawnChance;
        }
    }

    public void Spawn()
    {
        foreach (ItemPickUp item in items)
        {
            //whichToSpawn += item.spawnChance;

            if (whichToSpawn >= chosen)
            {
                float positionX = Random.Range(-item.offsetPositionX, item.offsetPositionX);
                float positionY = Random.Range(-item.offsetPositionY, item.offsetPositionY);
                itemSpawned = Instantiate(item.itemSpawn, transform.position + new Vector3(positionX, positionY, 0f), Quaternion.identity);
                break;
            }
        }
    }
}
