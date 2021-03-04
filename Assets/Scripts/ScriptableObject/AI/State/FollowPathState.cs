using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AI/State/Follow Path State")]
public class FollowPathState : State
{
    public override void Enter(EnemyController controller)
    {
        controller.AssignPath(0);
        controller.transform.position = controller.waypoints.GetChild(controller.currentWayPoint).position;
        controller.SetShootDecision();
        
    }

    public override void LogicUpdate(EnemyController controller)
    {
        base.LogicUpdate(controller);
        if (Vector3.Distance(controller.transform.position, controller.waypoints.GetChild(controller.currentWayPoint).transform.position) < 0.2f)
        {
            controller.UpdatePath();
        }
    }

    public override void PhysicsUpdate(EnemyController controller)
    {
        if (controller.followPath)
        {
            Vector3 dir = (controller.waypoints.GetChild(controller.currentWayPoint).transform.position - controller.rb.transform.position).normalized;
            controller.rb.MovePosition(controller.rb.transform.position + dir * controller.stats.moveSpeed * Time.fixedDeltaTime);
        }
        else
        {
            controller.gameObject.SetActive(false);
        }
    }
}
