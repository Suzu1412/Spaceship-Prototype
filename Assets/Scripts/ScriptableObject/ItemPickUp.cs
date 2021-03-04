using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType
{
    None,
    Health,
    Power,
    Buff,
    Score
}

[CreateAssetMenu(menuName = "DropItem/Item", fileName = "New Item")]
public class ItemPickUp : ScriptableObject
{
    public Item itemSpawn;
    public ItemType type;
    public int amount;

    [Range(0, 0.5f)] public float offsetPositionX = 0;
    [Range(0, 0.5f)] public float offsetPositionY = 0;
}


