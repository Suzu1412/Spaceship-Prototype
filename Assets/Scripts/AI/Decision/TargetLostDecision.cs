using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AI/Decisions/Target Lost")]
public class TargetLostDecision : Decision
{
    public override bool Decide(EnemyController controller)
    {
        controller.DetectPlayer();
        return !controller.detected;
    }
}
