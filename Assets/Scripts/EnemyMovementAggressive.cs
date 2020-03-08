using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

public class EnemyMovementAggressive : EnemyBehaviour
{
    private Transform _target;
    private Transform _me;
    private float _ramSpeedFactor = 1.5f;
    private Vector3 _ramDirection;
    private bool _isRam = false;
    
    public virtual void Awake()
    {
        enemy = gameObject.GetComponent<Enemy>();
    }

    public override Steering GetSteering()
    {
        float distance;
        if (_target != null)
        {
            distance = Vector3.Distance(_target.position, _me.position);
        }
        else
        {
            distance = 10.0f;
        }

        if (distance <= 3.5f)
        {
            if (!_isRam)
            {
                _ramDirection = _target.position - _me.position;
                _isRam = true;
            }

            steering.linear = _ramDirection * _ramSpeedFactor;
        }
        
        return steering;
    }

    public void SetPlayer(Transform target, Transform me)
    {
        steering = new Steering();
        ResetRam();
        _target = target;
        _me = me;
    }

    public void ResetRam()
    {
        _isRam = false;
        _ramDirection = Vector3.down;
        steering.linear = _ramDirection;
    }
}
