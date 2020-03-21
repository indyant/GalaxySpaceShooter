using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

public class EnemyHomingMissile : Enemy
{
    private bool _isHeatSeek = false;
    
    // Start is called before the first frame update
    public override void Start()
    {
        Initialize();
        
        ObjectMovementHoming objectMovementHoming;
        objectMovementHoming = gameObject.AddComponent<ObjectMovementHoming>();
        Transform playerTransform = null;
        if (_player != null)
        {
            playerTransform = _player.transform;
        }
        
        objectMovementHoming.SetTarget(playerTransform, transform, true);
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
    
    protected override void CalculateMovement()
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
    
    public override void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Laser")
        {
            Destroy(other.gameObject);
            _player.AddScore(10);
            _anim.SetTrigger("OnMissileHit");
            _speed = 0;
            _audioSource.Play();
            Destroy(GetComponent<ObjectMovementHoming>());
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
            Destroy(GetComponent<ObjectMovementHoming>());
            Destroy(GetComponent<Collider2D>());
            Destroy(gameObject, 0.8f);
        }
    }
 
}

