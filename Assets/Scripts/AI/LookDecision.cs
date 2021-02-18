using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AI/Decisions/Look")]
public class LookDecision : Decision
{
    public override bool Decide(EnemyController controller)
    {
        return FindEnemies(controller);
    }

    private bool FindEnemies(EnemyController controller)
    {
        bool enemiesFound = false;
        GameObject[] players =  GameObject.FindGameObjectsWithTag("Player");
        if (players.Length > 0)
        {
            enemiesFound = true;
        }
        return enemiesFound;
    }
}
