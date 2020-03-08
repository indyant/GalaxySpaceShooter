using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EnemyMovementHoming : EnemyBehaviour
{
    private Transform _target;
    private Transform _me; 
    
    public virtual void Awake()
    {
        enemy = gameObject.GetComponent<Enemy>();
    }

    public override Steering GetSteering()
    {
        Vector3 targetDirection;
        
        if (_target != null)
        {
            targetDirection = _target.position - _me.position;
        }
        else
        {
            targetDirection = Vector3.down;
        }

        Vector3 cross = Vector3.Cross(_me.up, targetDirection);
        steering.angular = Vector3.Angle(_me.up, targetDirection); 

        if (cross.z < 0)
        {
            steering.angular *= -1f;
        }

        steering.linear = _me.up;
        return steering;
    }

    public void SetPlayer(Transform target, Transform me)
    {
        steering = new Steering();
        _target = target;
        _me = me;
    }
    
    
}
