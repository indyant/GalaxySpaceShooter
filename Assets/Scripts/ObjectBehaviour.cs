using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectBehaviour : MonoBehaviour
{
    public GameObject _target;
    protected ISteeringObject enemy;
    protected Steering steering;

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
