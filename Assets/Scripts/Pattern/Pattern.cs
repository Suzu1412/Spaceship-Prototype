using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Pattern : ScriptableObject
{
    public int[] bulletAmount = new int[3];

    public abstract void PlaceProjectile(Weapon weapon, int level, int pointer);
}
