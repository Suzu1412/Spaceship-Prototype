using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class AttackBuff : TemporaryBuff
{
    public float amount;

    public override void Initialize(PlayerController player)
    {
        player.stats.damage += (int)amount;
    }

    public override void RemoveBuff(PlayerController player)
    {
        player.stats.damage -= (int)amount;
    }
}
