using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomingLaser : MonoBehaviour, ISteeringObject
{
    [SerializeField] protected float _speed = 5.0f;
    protected float _maxSpeed = 8.0f;
    
    protected float _screenBoundRight = 9.0f;
    protected float _screenBoundLeft = -9.0f;
    protected float _screenOutTop = 6.0f;
    protected float _screenOutBottom = -6.0f;
    
    protected float _orientation;
    protected float _rotation;
    
    protected Steering _steering;
    private GameObject _enemyContainer;
    private Vector3 _velocity;
    Transform _nearestEnemyTransform = null;
    ObjectMovementHoming _objectMovementHoming;
    
    // Start is called before the first frame update
    void Start()
    {
        float distance;
        float nearestDistance = 1000f;
        _objectMovementHoming = gameObject.AddComponent<ObjectMovementHoming>();
        
        _enemyContainer = GameObject.Find("EnemyContainer");
        if (_enemyContainer == null)
        {
            Debug.LogError("_enemyContainer is null");
        }


        _velocity = Vector3.zero;
        _steering = new Steering();
     
        /*
        objectMovementHoming.SetTarget(playerTransform, transform);
        StartCoroutine(LaunchMissileRoutine());
        */
    }

    // Update is called once per frame
    void Update()
    {
        CalculateMovement();
    }

    protected virtual void CalculateMovement()
    {
        // transform.Translate(Vector3.down * _speed * Time.deltaTime);

        // Phase-II-1 
        Vector3 displacement = _velocity * Time.deltaTime;
        _orientation = _rotation * _speed * Time.deltaTime;

        if (_orientation < 0.0f)
        {
            _orientation += 360.0f;
        }
        else if (_orientation > 360.0f)
        {
            _orientation -= 360.0f;
        }

        // transform.rotation = new Quaternion();
        transform.Rotate(Vector3.forward, _orientation);
        transform.Translate(displacement, Space.World);

        if (CheckOutOfScreen())
        {
            Destroy(gameObject);
        }
        
        
        if (_nearestEnemyTransform == null)
        {
            SetNearestEnemy();
        }
    }
    
    private bool CheckOutOfScreen()
    {
        if ((transform.position.y <= _screenOutBottom) ||
            (transform.position.x >= _screenBoundRight) ||
            (transform.position.x <= _screenBoundLeft) ||
            (transform.position.y >= _screenOutTop))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    
    public virtual void LateUpdate()
    {
        _velocity = _steering.linear * _speed; //  * _speed * Time.deltaTime;
        _rotation = _steering.angular; // * Time.deltaTime;

        if (_velocity.magnitude > _maxSpeed)
        {
            _velocity.Normalize();
            _velocity = _velocity * _maxSpeed;
        }

        if (_steering.angular == 0.0f)
        {
            _rotation = 0.0f;
        }

        if (_steering.linear.sqrMagnitude == 0.0f)
        {
            _velocity = Vector3.zero;
        }

        _steering = new Steering();
    }
    

    public void SetSteering(Steering steering)
    {
        // Debug.Log("Steering.linear = " + steering.linear);
        _steering = steering;
    }

    private void SetNearestEnemy()
    {
        float distance;
        float nearestDistance = 1000f;

        if (_enemyContainer.transform.childCount > 0)
        {
            foreach (Transform enemyTransform in _enemyContainer.transform)
            {
                distance = Vector3.Distance(transform.position, enemyTransform.position);
                if (distance <= nearestDistance)
                {
                    nearestDistance = distance;
                    _nearestEnemyTransform = enemyTransform;
                }
            }
        }
        else
        {
            _nearestEnemyTransform = null;
        }
        
        _objectMovementHoming.SetTarget(_nearestEnemyTransform, transform, false);
    }
    
}
