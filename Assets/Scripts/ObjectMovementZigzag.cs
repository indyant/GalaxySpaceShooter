using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class ObjectMovementZigzag : ObjectBehaviour
 {
     private Vector3 _direction;
     private float _duration = 1.0f;

     private bool _stopMovement = true;
     private Vector3 _reverseDirection;

    public virtual void Awake()
    {
        enemy = gameObject.GetComponent<NewEnemy>();
    }
     
     public void Start()
     {
         _reverseDirection = new Vector3(-1, 1, 0);
     }

     public override Steering GetSteering()
     {
         steering.linear = _direction;
         return steering;
     }
 
     public void SetZigzag(Vector3 direction, float duration)
     {
         steering = new Steering();
         _direction = direction;
         _duration = duration;
         _stopMovement = false;

         StartCoroutine(ChangeDirectionRoutine());
     }
     
    IEnumerator ChangeDirectionRoutine()
    {
        while (_stopMovement == false)
        {
            yield return new WaitForSeconds(_duration);
            _direction.x *= -1;
        }
    }
     
 }

