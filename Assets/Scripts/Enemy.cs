using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Enemy : MonoBehaviour, ISteeringObject
{
    [SerializeField] protected float _speed = 4.0f;
    protected float _screenBoundRight = 9.0f;
    protected float _screenBoundLeft = -9.0f;
    protected float _screenOutBottom = -6.0f;
    protected GameObject _playerObject;
    protected Player _player;
    protected Animator _anim;
    
    // Preparation for Phase-II-1 New Enemy Movement
    protected float _maxSpeed = 10;
    protected float _maxAccel = 10;
    protected float _orientation;
    [SerializeField] protected float _rotation;
    protected Vector3 _velocity;
    protected Steering _steering;


    [SerializeField] protected AudioClip _explosionSoundClip;
    protected AudioSource _audioSource;

    [SerializeField] protected GameObject _laserPrefab;
    protected float _fireRate = 3.0f;
    protected float _canFire = 0f;
    protected bool _isFirePickup = false;
    private GameObject _pickupContainer;
    private GameObject _playerLaserContainer;

    private Vector3 _currentVector;
    private Vector3 _vectorDown;
    private Vector3 _vectorDownRight;
    private Vector3 _vectorDownLeft;
    private ObjectMovementStraight _objectMovementStraight;
    private bool _isLaserAvoid = false;

    protected void Initialize()
    {
        _playerObject = GameObject.Find("Player");
        
        if (_playerObject == null)
        {
            Debug.Log("_player is null");
        }
        else
        {
            _player = GameObject.Find("Player").GetComponent<Player>();
        }

        _anim = GetComponent<Animator>();
        if (_anim == null)
        {
            Debug.LogError("_anim is null");
        }

        _audioSource = GetComponent<AudioSource>();
        if (_audioSource == null)
        {
            Debug.LogError("_audioSource is null");
        }
        else
        {
            _audioSource.clip = _explosionSoundClip;
        }

        _pickupContainer = GameObject.Find("PickupContainer");
        if (_pickupContainer == null)
        {
            Debug.LogError("_pickupContainer is null");
        }

        _playerLaserContainer = GameObject.Find("PlayerLaserContainer");
        if (_playerLaserContainer == null)
        {
            Debug.LogError("_playerLaserContainer is null");
        }
        
        // Preparation for Phase-II-1 New Enemy Movement
        _velocity = Vector3.zero;
        _steering = new Steering();
        
        _vectorDown = Vector3.down;
        _vectorDownRight = new Vector3(1f, -1f, 0f);
        _vectorDownLeft = new Vector3(-1f, -1f, 0f);
    }

    // Start is called before the first frame update
    public virtual void Start()
    {
        Initialize();

        int dirSwitch = Random.Range(0, 2);
        _objectMovementStraight  = gameObject.AddComponent<ObjectMovementStraight>();
            
        switch (dirSwitch)
        {
            case 0: // straight down
                _currentVector = _vectorDown;
                break;
            case 1: // slant
                if (transform.position.x >= 0)
                {
                    _currentVector = _vectorDownLeft;
                }
                else
                {
                    _currentVector = _vectorDownRight;
                }

                break;
            default:
                _currentVector = _vectorDownLeft;
                break;
        }
        
        _objectMovementStraight.SetVector(_currentVector);
        _fireRate = Random.Range(2.0f, 5.0f);
        _canFire = Time.time + _fireRate;
    }

    // Update is called once per frame
    public virtual void Update()
    {
        CalculateMovement();

        if (Time.time > _canFire)
        {
            _fireRate = Random.Range(2.0f, 5.0f);
            _canFire = Time.time + _fireRate;
            GameObject enemyLaser = Instantiate(_laserPrefab, transform.position, Quaternion.identity);
            Laser[] lasers = enemyLaser.GetComponentsInChildren<Laser>();

            for (int i = 0; i < lasers.Length; i++)
            {
                lasers[i].AssignEnemyLaser();
            }
        }

        if (!_isFirePickup && CheckPickupAhead())
        {
            _isFirePickup = true;
            
            GameObject enemyLaser = Instantiate(_laserPrefab, transform.position, Quaternion.identity);
            Laser[] lasers = enemyLaser.GetComponentsInChildren<Laser>();

            for (int i = 0; i < lasers.Length; i++)
            {
                lasers[i].AssignEnemyLaser();
            }
        }
    }

    protected virtual void CalculateMovement()
    {
        // transform.Translate(Vector3.down * _speed * Time.deltaTime);
        
        // Phase-II-1 
        Vector3 displacement = _velocity * Time.deltaTime;
        
        _orientation += _rotation * Time.deltaTime;

        if (_orientation < 0.0f)
        {
            _orientation += 360.0f;
        }
        else if (_orientation > 360.0f)
        {
            _orientation -= 360.0f;
        }
        
        transform.Translate(displacement);
        transform.rotation = new Quaternion();
        transform.Rotate(Vector3.forward, _orientation);
        
        if (transform.position.y <= _screenOutBottom)
        {
            float randomX = Random.Range(_screenBoundLeft, _screenBoundRight);
            transform.position = new Vector3(randomX, 7, 0);
        }

        if (!_isLaserAvoid && CheckAllLasersInRange())
        {
            if (_currentVector == _vectorDown)
            {
                if (transform.position.x >= 0)
                {
                    _objectMovementStraight.SetVector(_vectorDownLeft);
                    _currentVector = _vectorDownLeft;
                }
                else
                {
                    _objectMovementStraight.SetVector(_vectorDownRight);
                    _currentVector = _vectorDownLeft;
                }
            }
            else if (_currentVector == _vectorDownLeft)
            {
                _objectMovementStraight.SetVector(_vectorDownRight);
                _currentVector = _vectorDownRight;
            }
            else if (_currentVector == _vectorDownRight)
            {
                _objectMovementStraight.SetVector(_vectorDownLeft);
                _currentVector = _vectorDownLeft;
            }

            _isLaserAvoid = true;
            StartCoroutine(ResetLaserAvoidRoutine());
        }
    }

    protected bool CheckPickupAhead()
    {
        bool isPickupAhead = false;

        foreach (Transform childTransform in _pickupContainer.transform)
        {
            if (childTransform.position.y <= transform.position.y)
            {
                if ((childTransform.position.x >= transform.position.x - 0.25) &&
                    (childTransform.position.x <= transform.position.x + 0.25))
                {
                    isPickupAhead = true;
                    break;
                }
            }
        }

        return isPickupAhead;
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
    
    public virtual void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Laser")
        {
            Destroy(other.gameObject);
            _player.AddScore(10);
            _anim.SetTrigger("OnEnemyDeath");
            _speed = 0;
            _audioSource.Play();
            Destroy(GetComponent<Collider2D>());
            _canFire += 10.0f;
            Destroy(gameObject, 2.8f);
        }
        else if (other.tag == "Player")
        {
            Player player = other.GetComponent<Player>();
            if (player != null)
            {
                player.Damage();
            }
            _anim.SetTrigger("OnEnemyDeath");
            _speed = 0;
            _audioSource.Play();
            _canFire += 10.0f;
            Destroy(gameObject, 2.8f);
        }
    }

    public virtual void SetSteering(Steering steering)
    {
        // Debug.Log("Steering.linear = " + steering.linear);
        _steering = steering;
    }

    private bool CheckAllLasersInRange()
    {
        bool isLaserInRange = false;

        foreach (Transform childTransform in _playerLaserContainer.transform)
        {
            if (childTransform.childCount > 0)
            {
                foreach (Transform childLaserTransform in childTransform)
                {
                    if (CheckLaserInRange(childLaserTransform))
                    {
                        isLaserInRange = true;
                        break;
                    }
                }
            }
            else
            {
                if (CheckLaserInRange(childTransform))
                {
                    isLaserInRange = true;
                }
            }
            
            if (isLaserInRange)
            {
                break;
            }
        }
        
        return isLaserInRange;
    }

    private bool CheckLaserInRange(Transform laserTransform)
    {
        bool isInRange = false;
        
        if ((laserTransform.position.x >= transform.position.x - 0.25f) &&
            (laserTransform.position.x <= transform.position.x + 0.25f))
        {
            if (laserTransform.position.y - transform.position.y <= 1.0f)
            {
                isInRange = true;
            }
        }

        return isInRange;
    }

    IEnumerator ResetLaserAvoidRoutine()
    {
        yield return new WaitForSeconds(3.0f);

        _isLaserAvoid = false;
    }
}