using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AI/Decisions/Reach Target")]
public class ReachTargetDecision : Decision
{
    public float yPos;
    public override bool Decide(EnemyController controller)
    {
        bool reached = false;
        if (Vector3.Distance(controller.transform.position, new Vector2(controller.transform.position.x, yPos)) < 0.3f)
        {
            reached = true;
        }
        return reached;
    }
}
