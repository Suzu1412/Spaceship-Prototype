using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class FiniteStateMachine : ScriptableObject
{
    public abstract void Enter(EnemyController controller);

    public abstract void LogicUpdate(EnemyController controller);

    public abstract void PhysicsUpdate(EnemyController controller);

    public abstract void Exit(EnemyController controller);
}
