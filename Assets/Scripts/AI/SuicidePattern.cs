using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AI/Suicide Pattern")]
public class SuicidePattern : FiniteStateMachine
{
    public override void Enter(EnemyController controller)
    {
        controller.targetFailed = false;
    }

    public override void Exit(EnemyController controller)
    {
        controller.targetFailed = false;
    }

    public override void LogicUpdate(EnemyController controller)
    {
        controller.FindClosestEnemy();
    }

    public override void PhysicsUpdate(EnemyController controller)
    {
        if (controller.transform.position.x > 3.0f)
        {
            controller.transform.position = new Vector3(-3.0f, 0.0f, 0.0f);
        }

        controller.rb.MovePosition(controller.transform.position + controller.transform.right * Time.fixedDeltaTime);
    }
}
