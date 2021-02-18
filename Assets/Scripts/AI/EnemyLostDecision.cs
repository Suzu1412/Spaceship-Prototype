using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AI/Decisions/On Enemy Lost")]
public class EnemyLostDecision : Decision
{
    public override bool Decide(EnemyController controller)
    {
        return EnemyNotFound(controller);
    }

    private bool EnemyNotFound(EnemyController controller)
    {
        bool enemiesLost = false;
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        if (players.Length == 0)
        {
            enemiesLost = true;
        }
        return enemiesLost;
    }
}
