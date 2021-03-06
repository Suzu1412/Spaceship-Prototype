using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    public Explosion explosion;


    public void DestroyGameObject()
    {
        this.gameObject.SetActive(false);
    }
}
