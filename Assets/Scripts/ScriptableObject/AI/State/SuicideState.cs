using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AI/State/Suicide State")]
public class SuicideState: State
{
    public override void Enter(EnemyController controller)
    {
    }

    public override void Exit(EnemyController controller)
    {
        base.Exit(controller);
        controller.rb.velocity = Vector2.zero;
    }

    public override void LogicUpdate(EnemyController controller)
    {
        base.LogicUpdate(controller);
    }

    public override void PhysicsUpdate(EnemyController controller)
    {
        if (controller.playerPosition == null) return;

        if (controller.EnemyDirection().y < 0f)
        {
            if (controller.EnemyDirection().x <= -0.1f)
            {
                controller.rb.velocity = new Vector2(-controller.stats.moveSpeed * 0.75f, -controller.stats.moveSpeed * 3);
            }
            else if (controller.EnemyDirection().x >= 0.1f)
            {
                controller.rb.velocity = new Vector2(controller.stats.moveSpeed * 0.75f, -controller.stats.moveSpeed * 3);
            }
            else
            {
                controller.rb.velocity = Vector2.down * controller.stats.moveSpeed * 2;
            }
        }
    }

}
