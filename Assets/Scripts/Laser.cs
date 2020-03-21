﻿using UnityEngine;

public class Laser : MonoBehaviour
{
    private readonly float _boundary = 8.0f;
    [SerializeField] private float _speed = 5.0f;
    [SerializeField] private bool _isEnemyLaser = false;
    private bool _isEnemyLaserBackward = false;

    [SerializeField] private bool _isDirectionalLaser = false;
    [SerializeField] private Vector3 _direction;
    private Vector3 _moveDirection;
    
    private Player _player;

    // Start is called before the first frame update
    private void Start()
    {
//        transform.position = new Vector3();
        if (_isDirectionalLaser)
        {
            SetDirection(_direction);
        }
    }

    // Update is called once per frame
    private void Update()
    {
        if (_isEnemyLaser)
        {
            if (_isDirectionalLaser)
            {
                MoveDirection();
            }
            else if (_isEnemyLaserBackward)
            {
                MoveUp();
            }
            else
            {
                MoveDown();
            }
        }
        else if (_isDirectionalLaser)
        {
            MoveDirection();
        }
        else
        {
            MoveUp();
        }
    }

    private void MoveUp()
    {
        transform.Translate(Vector3.up * _speed * Time.deltaTime);
        if (transform.position.y >= _boundary)
        {
            if (transform.parent != null)
            {
                if (transform.parent.tag != "PlayerLaserContainer")
                {
                    Destroy(transform.parent.gameObject);
                }
            }

            Destroy(gameObject);
        }
    }

    private void MoveDown()
    {
        transform.Translate(Vector3.up * _speed * -1.0f * Time.deltaTime);
        if (transform.position.y <= (_boundary * -1.0f))
        {
            if (transform.parent != null)
            {
                if (transform.parent.tag != "PlayerLaserContainer")
                {
                    Destroy(transform.parent.gameObject);
                }
            }

            Destroy(gameObject);
        }
    }

    // Phase I: Framework - Quiz - Secondary Fire Powerup
    private void MoveDirection()
    {
        // transform.Translate(direction * _speed * Time.deltaTime);
        transform.Translate(_moveDirection * _speed * Time.deltaTime);
        if ((transform.position.y >= _boundary) || (transform.position.y <= (_boundary * -1.0f)))
        {
            if (transform.parent != null)
            {
                Destroy(transform.parent.gameObject);
            }

            Destroy(gameObject);
        }
    }

    public void AssignEnemyLaser()
    {
        _isEnemyLaser = true;
    }

    public void SetEnemyLaserBackward(bool backward = true)
    {
        _isEnemyLaserBackward = backward;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player" && _isEnemyLaser)
        {
            Player player = other.GetComponent<Player>();
            if (player != null)
            {
                player.Damage();
            }
            Destroy(gameObject, 2.8f);
        }
    }

    public void SetDirection(Vector3 direction)
    {
        _isDirectionalLaser = true;
        _moveDirection = Vector3.up;

        float angle = Mathf.Atan2(direction.x, direction.y) * Mathf.Rad2Deg;
//        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        transform.Rotate(Vector3.forward, angle);
    }
}