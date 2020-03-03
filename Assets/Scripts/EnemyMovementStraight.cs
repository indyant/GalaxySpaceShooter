using System.Collections;
using System.Collections.Generic;
using System.Transactions;
using UnityEngine;

public class EnemyMovementStraight : EnemyBehaviour
{
    private Steering steering;
 
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
