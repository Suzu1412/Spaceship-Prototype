using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Decision : ScriptableObject
{
    public State nextState;

    public abstract bool Decide(EnemyController controller);
}
