using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AI/State/Idle State")]
public class IdleState : State
{
    public override void Enter(EnemyController controller)
    {
        Debug.Log("Enter Idle");
    }

    public override void Exit(EnemyController controller)
    {
        Debug.Log("Exit Idle");
        //throw new System.NotImplementedException();
    }

    public override void LogicUpdate(EnemyController controller)
    {
        base.LogicUpdate(controller);
    }

    public override void PhysicsUpdate(EnemyController controller)
    {
        controller.rb.velocity = Vector2.zero;
    }
}
