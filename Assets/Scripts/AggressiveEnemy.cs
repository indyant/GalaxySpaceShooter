using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AggressiveEnemy : Enemy
{
    // Start is called before the first frame update
    private ObjectMovementAggressive _objectMovementAggressive;
    
    
    public override void Start()
    {
        Initialize();
        
        _objectMovementAggressive = gameObject.AddComponent<ObjectMovementAggressive>();
        _objectMovementAggressive.SetPlayer(_player.transform, transform);

        _fireRate = Random.Range(2.0f, 5.0f);
        _canFire = Time.time + _fireRate;
        _speed = 2.0f;
    }

    protected override void CalculateMovement()
    {
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
            _objectMovementAggressive.ResetRam();
        }
    }
    
    
    public override void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Laser")
        {
            Destroy(other.gameObject);
            _player.AddScore(10);
            _anim.SetTrigger("OnNewEnemyDeath");
            _speed = 0;
            _audioSource.Play();
            Destroy(GetComponent<Collider2D>());
            _canFire += 10.0f;
            Destroy(gameObject, 1.0f);
        }
        else if (other.tag == "Player")
        {
            Player player = other.GetComponent<Player>();
            if (player != null)
            {
                player.Damage();
            }
            _anim.SetTrigger("OnNewEnemyDeath");
            _speed = 0;
            _audioSource.Play();
            _canFire += 10.0f;
            Destroy(gameObject, 1.0f);
        }
    }
    
}
