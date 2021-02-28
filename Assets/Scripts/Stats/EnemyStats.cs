using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Stats/Enemy Stats")]
public class EnemyStats : Stats
{
    [Header("Enemy Stats")]
    public int collissionDamage;
    public int score = 50;

    [Header("Enemy Detection")]
    public LayerMask playerLayer;
    [Range(0f, 360f)] public float angle;
    public float distance;

    [Header("Chance Based Stats")]
    [Range(0f, 1f)] public float dropChance;
    [Range(0f, 1f)] public float shootChance;
    [Range(0f, 1f)] public float waitUntilShoot;
}
