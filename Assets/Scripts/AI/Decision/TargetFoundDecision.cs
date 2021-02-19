using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AI/Decisions/Target Found")]
public class TargetFoundDecision : Decision
{
    public override bool Decide(EnemyController controller)
    {
        controller.DetectPlayer();
        return controller.detected;
    }


}
