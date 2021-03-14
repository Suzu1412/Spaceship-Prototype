using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TemporaryBuff : ScriptableObject
{
    public bool limitBreak;
    public float duration;

    public abstract void Initialize(PlayerController player);

    public abstract void RemoveBuff(PlayerController player);
}
