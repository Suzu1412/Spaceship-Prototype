using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class State : ScriptableObject
{
    public Decision[] decisions;

    public abstract void Enter(EnemyController controller);

    public virtual void LogicUpdate(EnemyController controller)
    {
        MakeTransition(controller);
    }

    public abstract void PhysicsUpdate(EnemyController controller);

    public abstract void Exit(EnemyController controller);


    public void MakeTransition(EnemyController controller)
    {
        for (int i = 0; i < decisions.Length; i++)
        {
            if (decisions[i].Decide(controller))
            {
                controller.TransitionToState(decisions[i].nextState);
                break;
            }
        }
    }
}
