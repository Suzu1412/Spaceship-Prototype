using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AI/Suicide Pattern")]
public class SuicidePattern : FiniteStateMachine
{
    public override void Enter(EnemyController controller)
    {
        controller.targetFailed = false;
        controller.InvokeFindClosestEnemy();
    }

    public override void Exit(EnemyController controller)
    {
        controller.targetFailed = false;
        controller.DisableFindClosestEnemy();
    }

    public override void LogicUpdate(EnemyController controller)
    {
        return;
    }

    public override void PhysicsUpdate(EnemyController controller)
    {
        if (controller.playerPosition != null)
        {
            if (controller.EnemyDirection().y < 0f)
            {
                if (controller.EnemyDirection().x <= -0.1f)
                {
                    controller.rb.velocity = new Vector2(-controller.moveSpeed / 2, -controller.moveSpeed);
                }
                else if (controller.EnemyDirection().x >= 0.1f)
                {
                    controller.rb.velocity = new Vector2(controller.moveSpeed / 2, -controller.moveSpeed);
                }
                else 
                {
                    controller.rb.velocity = new Vector2(0f, -controller.moveSpeed);
                }
            }
        }
    }
}
