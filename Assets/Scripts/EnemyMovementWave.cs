using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyMovementWave : EnemyBehaviour
 {
     private Steering steering;
     private Vector3 _direction;
     private float _duration = 1.0f;
     private float _radian = 0f;
     private float _height = 1f;

     private bool _stopMovement = true;
     private float TwoPI;

    public virtual void Awake()
    {
        enemy = gameObject.GetComponent<NewEnemy>();
    }
     
     public void Start()
     {
         TwoPI = 2 * Mathf.PI;
     }

     public override Steering GetSteering()
     {
         _radian += (Mathf.PI / _duration) * Time.deltaTime;

         if (_radian > TwoPI)
         {
             _radian -= TwoPI;
         }

         steering.linear.x = Mathf.Sin(_radian) * _height;
         steering.linear.y = -1.0f;
         steering.linear.Normalize();
         
         return steering;
     }
 
     public void SetWave(float height, float duration)
     {
         steering = new Steering();

         
         steering.linear.y = -1.0f;
         _radian = 0;
         _height = height;
         _duration = duration;
         _stopMovement = false;
     }
 }

