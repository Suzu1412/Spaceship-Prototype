using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Stats : ScriptableObject
{
    [Header("Ship Stats")]
    public int maxHealth;
    public int damage;
    public float moveSpeed;
    [Header("Probability")]
    [Range(0f, 1f)] public float criticalChance;
    [Range(0f, 1f)] public float dodgeChance; 
}
