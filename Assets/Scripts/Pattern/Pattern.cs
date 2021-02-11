using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Pattern : ScriptableObject
{
    public int[] bulletAmount = new int[3];
    public float initialOffset;
    public float offsetBetweenShots;
    [Range(0, 360)]
    public int spread;

    public abstract void PlaceProjectile(Weapon weapon, int level, int pointer);
}
