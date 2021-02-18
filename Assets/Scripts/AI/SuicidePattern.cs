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
                    controller.rb.velocity = new Vector2(-controller.stats.moveSpeed / 2, -controller.stats.moveSpeed);
                }
                else if (controller.EnemyDirection().x >= 0.1f)
                {
                    controller.rb.velocity = new Vector2(controller.stats.moveSpeed / 2, -controller.stats.moveSpeed);
                }
                else 
                {
                    controller.rb.velocity = Vector2.down * controller.stats.moveSpeed;
                }
            }
        }
    }

    public override bool Transition(Decision decision)
    {
        throw new System.NotImplementedException();
    }
}
