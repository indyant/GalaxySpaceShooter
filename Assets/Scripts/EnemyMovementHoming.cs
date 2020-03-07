using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EnemyMovementHoming : EnemyBehaviour
{
    private Steering _steering;
    private Transform _target;
    private Transform _me; 
    public virtual void Awake()
    {
        enemy = gameObject.GetComponent<Enemy>();
    }

    public override Steering GetSteering()
    {
        if (_target != null)
        {
            _steering.linear = _target.position - _me.position;
        }
        else
        {
            _steering.linear = Vector3.down;
        }

        Vector3 cross = Vector3.Cross(_me.up, _steering.linear);
        _steering.angular = Vector3.Angle(_me.up, _steering.linear);

        if (cross.z < 0)
        {
            _steering.angular *= -1f;
        }

        return _steering;
    }

    public void SetPlayer(Transform target, Transform me)
    {
        _steering = new Steering();
        _target = target;
        _me = me;
    }
    
    
}
