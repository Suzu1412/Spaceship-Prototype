using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "DropItem/Buff/Shield")]
public class ShieldBuff : TemporaryBuff
{
    public int shieldDuration;

    public override void Initialize(PlayerController player)
    {
        player.ActivateShield(shieldDuration);
    }

    public override void RemoveBuff(PlayerController player)
    {
        player.DisableShield();
    }
}
