using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "DropItem/Drop table", fileName = "New Drop Table")]
public class DropTable : ScriptableObject
{
    public List<DropDefinition> drops;

    List<ItemPickUp> GetDrop()
    {
        List<ItemPickUp> dropItems = new List<ItemPickUp>();

        foreach(DropDefinition drop in drops)
        {
            bool shouldDrop = Random.value < drop.dropChance;

            if (shouldDrop)
            {
                dropItems.Add(drop.item);
            }
        }

        return dropItems;
    }

    public void SpawnItem(EnemyController controller)
    {
        List<ItemPickUp> dropItems = GetDrop();

        foreach (ItemPickUp item in dropItems)
        {
            float positionX = Random.Range(-item.offsetPositionX, item.offsetPositionX);
            float positionY = Random.Range(-item.offsetPositionY, item.offsetPositionY);

            switch (item.type)
            {
                case ItemType.Power:
                    controller.objectPooler.SpawnFromPool(item.itemSpawn.name, controller.transform.position + new Vector3(positionX, positionY, 0f), Quaternion.identity);
                    break;

                case ItemType.Health:
                    controller.objectPooler.SpawnFromPool(item.itemSpawn.name, controller.transform.position + new Vector3(positionX, positionY, 0f), Quaternion.identity);
                    break;

                case ItemType.Score:
                    controller.objectPooler.SpawnFromPool(item.itemSpawn.name, controller.transform.position + new Vector3(positionX, positionY, 0f), Quaternion.identity);
                    break;

                default:
                    Instantiate(item.itemSpawn, controller.transform.position + new Vector3(positionX, positionY, 0f), Quaternion.identity);
                    break;
            }
            
        }
    }
}

[System.Serializable]
public class DropDefinition
{
    public ItemPickUp item;
    [Range(0f, 1f)] public float dropChance;
}