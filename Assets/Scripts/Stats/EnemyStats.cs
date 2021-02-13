using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Stats/Enemy Stats")]
public class EnemyStats : Stats
{
    [Range(0f, 1f)] public float dropChance;
    [Range(0f, 1f)] public float shootChance;
    [Range(0f, 1f)] public float waitUntilShoot;
    public int collissionDamage;

}
