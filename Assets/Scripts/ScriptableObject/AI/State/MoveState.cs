using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AI/State/Move State")]
public class MoveState : State
{
    public override void Enter(EnemyController controller)
    {
        controller.DisableShootDecision();
    }

    public override void Exit(EnemyController controller)
    {
        base.Exit(controller);
        controller.rb.velocity = Vector3.zero;
    }

    public override void PhysicsUpdate(EnemyController controller)
    {
        controller.rb.velocity = Vector2.down * controller.stats.moveSpeed;
    }
}
