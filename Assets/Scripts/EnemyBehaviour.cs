using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehaviour : MonoBehaviour
{
    public GameObject _target;
    protected Enemy enemy;
    
    public virtual void Awake()
    {
        enemy = gameObject.GetComponent<Enemy>();
    }

    // Update is called once per frame
    public virtual void Update()
    {
        enemy.SetSteering(GetSteering());
    }
    
    public virtual Steering GetSteering()
    {
        return new Steering();
    }
}
