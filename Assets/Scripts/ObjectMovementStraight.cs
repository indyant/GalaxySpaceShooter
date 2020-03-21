using System.Collections;
using System.Collections.Generic;
using System.Transactions;
using UnityEngine;

public class ObjectMovementStraight : ObjectBehaviour
{
    public virtual void Awake()
    {
        enemy = gameObject.GetComponent<Enemy>();
    }
    
    public override Steering GetSteering()
    {
        return steering;
    }

    public void SetVector(Vector3 direction)
    {
        steering = new Steering();
        steering.linear = direction;
        steering.linear.Normalize();
    }
}
