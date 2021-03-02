using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AI/State/Retire State")]
public class RetireState : State
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
        controller.rb.velocity = Vector2.up * controller.stats.moveSpeed;
    }
}

