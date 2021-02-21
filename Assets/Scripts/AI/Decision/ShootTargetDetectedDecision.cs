using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AI/Decisions/Shoot Target Detected")]
public class ShootTargetDetectedDecision : Decision
{
    public override bool Decide(EnemyController controller)
    {
        controller.DetectPlayer();
        controller.canShoot = false;
        if (controller.detected)
        {
            controller.canShoot = true;

        }
        return controller.detected;
    }
}
