using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

public class EnemyMissile : Enemy
{
    private bool _isHeatSeek = false;
    
    // Start is called before the first frame update
    public override void Start()
    {
//        _rb = GetComponent<Rigidbody2D>();
        _velocity = Vector3.zero;
        _steering = new Steering();

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
        
        /*
        EnemyMovementStraight enemyMovementStraight;
        
        enemyMovementStraight  = gameObject.AddComponent<EnemyMovementStraight>();
        enemyMovementStraight.SetVector(new Vector3(0f, -1f, 0f));
        */
        
        EnemyMovementHoming enemyMovementHoming;
        enemyMovementHoming = gameObject.AddComponent<EnemyMovementHoming>();
        Transform playerTransform = null;
        if (_player != null)
        {
            playerTransform = _player.transform;
        }
        
        enemyMovementHoming.SetPlayer(playerTransform, transform);
        StartCoroutine(LaunchMissileRoutine());
    }

    IEnumerator LaunchMissileRoutine()
    {
        yield return new WaitForSeconds(0.4f);
        _isHeatSeek = true;
        _anim.SetTrigger("OnMissileFlying");
    }

    // Update is called once per frame
    public override void Update()
    {
        CalculateMovement();
    }

    private void FixedUpdate()
    {
//        _rb.velocity = transform.up * _speed;
    }
    
    public override void CalculateMovement()
    {
        if (_isHeatSeek == true)
        {
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
        }
        else
        {
            Vector3 displacement = _velocity * Time.deltaTime;
            transform.Translate(displacement, Space.World);
        }

        if (CheckOutOfScreen())
        {
            if (transform.parent != null)
            {
                Destroy(transform.parent.gameObject);
            }
            Destroy(gameObject);
        }
    }

    private bool CheckOutOfScreen()
    {
        if ((transform.position.y <= _screenOutBottom) ||
            (transform.position.x >= _screenBoundRight) ||
            (transform.position.x <= _screenBoundLeft))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public override void LateUpdate()
    {
        // _velocity = _steering.linear * _speed;  //* Time.deltaTime;
        _velocity = transform.up * _speed;
        _rotation = _steering.angular;

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
    
    public override void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Laser")
        {
            Destroy(other.gameObject);
            _player.AddScore(10);
            _anim.SetTrigger("OnMissileHit");
            _speed = 0;
            _audioSource.Play();
            Destroy(GetComponent<EnemyMovementHoming>());
            Destroy(GetComponent<Collider2D>());
            Destroy(gameObject, 0.8f);
        }
        else if (other.tag == "Player")
        {
            Player player = other.GetComponent<Player>();
            if (player != null)
            {
                player.Damage();
            }
            _anim.SetTrigger("OnMissileHit");
            _speed = 0;
            _audioSource.Play();
            Destroy(GetComponent<EnemyMovementHoming>());
            Destroy(GetComponent<Collider2D>());
            Destroy(gameObject, 0.8f);
        }
    }
 
}

