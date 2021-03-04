using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHealth 
{
    void Heal(int amount);

    void Damage(int amount);

    void Death();
    
}
