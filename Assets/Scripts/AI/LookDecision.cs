using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AI/Decisions/Look")]
public class LookDecision : Decision
{
    public override bool Decide(EnemyController controller)
    {
        bool targetVisible = false;

        return targetVisible;
    }

    private bool Look(EnemyController controller)
    {
        return false;
    }
}
