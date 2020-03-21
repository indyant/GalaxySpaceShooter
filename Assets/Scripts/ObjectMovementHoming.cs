using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ObjectMovementHoming : ObjectBehaviour
{
    private Transform _target;
    private Transform _me;
    private Vector3 _targetDirection;
    private bool _isEnemy = true;
    
    public virtual void Awake()
    {
        enemy = gameObject.GetComponent<ISteeringObject>();
        _targetDirection = Vector3.down;
    }

    public override Steering GetSteering()
    {
        if (_target != null)
        {
            _targetDirection = _target.position - _me.position;
        }

        Vector3 cross = Vector3.Cross(_me.up, _targetDirection);
        steering.angular = Vector3.Angle(_me.up, _targetDirection); 

        if (cross.z < 0)
        {
            steering.angular *= -1f;
        }

        steering.linear = _me.up;
        return steering;
    }

    public void SetTarget(Transform target, Transform me, bool isEnemy = true)
    {
        steering = new Steering();
        _target = target;
        _me = me;
        _isEnemy = isEnemy;

        if (_isEnemy)
        {
            _targetDirection = Vector3.down;
        }
        else
        {
            _targetDirection = Vector3.up;
        }
    }
    
}
