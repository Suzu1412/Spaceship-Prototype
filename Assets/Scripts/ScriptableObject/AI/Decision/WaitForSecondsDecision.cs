using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AI/Decisions/Wait for Seconds")]
public class WaitForSecondsDecision : Decision
{
    public float secondsToWait;

    public override bool Decide(EnemyController controller)
    {
        return controller.WaitForSeconds(secondsToWait);
    }
}
