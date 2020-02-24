using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Enemy : MonoBehaviour
{
    [SerializeField] private float _speed = 4.0f;
    private float _screenBoundRight = 9.0f;
    private float _screenBoundLeft = -9.0f;
    private float _screenOutBottom = -6.0f;
    private Player _player;
    private Animator _anim;
    
    // Preparation for Phase-II-1 New Enemy Movement
    private float _maxSpeed = 10;
    private float _maxAccel = 10;
    private float _orientation;
    private float _rotation;
    private Vector3 _velocity;
    private Steering _steering;


    [SerializeField] private AudioClip _explosionSoundClip;
    private AudioSource _audioSource;

    [SerializeField] private GameObject _laserPrefab;
    private float _fireRate = 3.0f;
    private float _canFire = 0f;

    // Start is called before the first frame update
    void Start()
    {
        _player = GameObject.Find("Player").GetComponent<Player>();
        if (_player == null)
        {
            Debug.LogError("_player is null");
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
        
        // Preparation for Phase-II-1 New Enemy Movement
        _velocity = Vector3.zero;
        _steering = new Steering();


        int dirSwitch = Random.Range(0, 4);
        EnemyMovementStraight enemyMovementStraight;
        EnemyMovementZigzag enemyMovementZigzag;
            
        switch (dirSwitch)
        {
            case 0: // straight down
                enemyMovementStraight  = gameObject.AddComponent<EnemyMovementStraight>();
                enemyMovementStraight.SetVector(new Vector3(0f, -1f, 0f));
                break;
            case 1: // right
                enemyMovementStraight = gameObject.AddComponent<EnemyMovementStraight>();
                enemyMovementStraight.SetVector(new Vector3(1f, -1f, 0f));
                break;
            case 2:  // left
                enemyMovementStraight = gameObject.AddComponent<EnemyMovementStraight>();
                enemyMovementStraight.SetVector(new Vector3(-1f, -1f, 0f));
                break;
            case 3: // zigzag
                enemyMovementZigzag = gameObject.AddComponent<EnemyMovementZigzag>();
                enemyMovementZigzag.SetZigzag(new Vector3(-1, -1, 0), 1.5f);
                break;
            default:
                enemyMovementStraight = gameObject.AddComponent<EnemyMovementStraight>();
                enemyMovementStraight.SetVector(new Vector3(0f, -1f, 0f));
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        CalculateMovement();

        if (Time.time > _canFire)
        {
            _fireRate = Random.Range(3.0f, 7.0f);
            _canFire = Time.time + _fireRate;
            GameObject enemyLaser = Instantiate(_laserPrefab, transform.position, Quaternion.identity);
            Laser[] lasers = enemyLaser.GetComponentsInChildren<Laser>();

            for (int i = 0; i < lasers.Length; i++)
            {
                lasers[i].AssignEnemyLaser();
            }
        }
    }

    void CalculateMovement()
    {
        // transform.Translate(Vector3.down * _speed * Time.deltaTime);
        
        // Phase-II-1 
        Debug.Log("velocity: " + _velocity);
        Vector3 displacement = _velocity * Time.deltaTime;
        _orientation += _rotation * Time.deltaTime;

        Debug.Log("displacement: " + displacement.ToString("F4"));
        

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
        transform.Rotate(Vector3.up, _orientation);
        
        if (transform.position.y <= _screenOutBottom)
        {
            float randomX = Random.Range(_screenBoundLeft, _screenBoundRight);
            transform.position = new Vector3(randomX, 7, 0);
        }
    }

    void LateUpdate()
    {
        _velocity = _steering.linear * _speed; //  * _speed * Time.deltaTime;
        _rotation += _steering.angular; // * Time.deltaTime;

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
    
    private void OnTriggerEnter2D(Collider2D other)
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

    public void SetSteering(Steering steering)
    {
        // Debug.Log("Steering.linear = " + steering.linear);
        _steering = steering;
    }
}