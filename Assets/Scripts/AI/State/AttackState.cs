using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AI/State/Attack State")]
public class AttackState : State
{
    public override void Enter(EnemyController controller)
    {
        controller.SetShootDecision();
    }

    public override void PhysicsUpdate(EnemyController controller)
    {
        controller.rb.velocity = Vector2.zero;
    }

    public override void Exit(EnemyController controller)
    {
        base.Exit(controller);
    }
}
