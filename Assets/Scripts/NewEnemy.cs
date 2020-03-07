using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewEnemy : Enemy
{
    /*
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
*/
    [SerializeField] private GameObject _missilePrefab;
    
    // Start is called before the first frame update

    enum NewEnemyType
    {
        EnemyType1,
        EnemyType2,
        EnemyType3
    };
    
    [SerializeField] private NewEnemyType _enemyType;
    
    public override void Start()
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
        EnemyMovementZigzag enemyMovementZigzag;
        EnemyMovementWave enemyMovementWave;

        switch (_enemyType)
        {
            case NewEnemyType.EnemyType1: // zigzag
                enemyMovementZigzag = gameObject.AddComponent<EnemyMovementZigzag>();
                float randomDirection = Random.value;
                Vector3 direction;
                if (randomDirection > 0.5f)
                {
                    direction = new Vector3(-1, -1, 0);
                }
                else
                {
                    direction = new Vector3(1, -1, 0);
                }
                enemyMovementZigzag.SetZigzag(direction, 1.3f);
                break;
            
            case NewEnemyType.EnemyType2:
                enemyMovementWave = gameObject.AddComponent<EnemyMovementWave>();
                enemyMovementWave.SetWave(1f, 1.5f);
                break;
            
            case NewEnemyType.EnemyType3:
                enemyMovementWave = gameObject.AddComponent<EnemyMovementWave>();
                enemyMovementWave.SetWave(1f, 3f);
                break;
            
            default:
                Debug.LogError("Invalid dirSwitch");
                break;
        }
        _fireRate = Random.Range(2.0f, 4.0f);
        _canFire = Time.time + _fireRate;
    }

    // Update is called once per frame
    public override void Update()
    {
        CalculateMovement();

        if (Time.time > _canFire)
        {
            switch (_enemyType)
            {
                case NewEnemyType.EnemyType1:
                case NewEnemyType.EnemyType2:
                    _fireRate = Random.Range(2.0f, 5.0f);
                    _canFire = Time.time + _fireRate;
                    GameObject enemyLaser =
                        Instantiate(_laserPrefab, transform.position, Quaternion.identity);
                    Laser laser = enemyLaser.GetComponent<Laser>();
                    laser.AssignEnemyLaser();
                    break;
                case NewEnemyType.EnemyType3:
                    _fireRate = Random.Range(2.0f, 8.0f);
                    _canFire = Time.time + _fireRate;
                    Vector3 missilePosition = transform.position;
                    missilePosition.y -= 0.5f;
                    GameObject enemyMissile =
                        Instantiate(_missilePrefab, missilePosition, _missilePrefab.transform.rotation); // Quaternion.identity);
                    
                    break;
                default:
                    Debug.LogError("Invlaid FireSwitch");
                    break;
            }
        }
    }

    public override void CalculateMovement()
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
    }

    public override void LateUpdate()
    {
        _velocity = _steering.linear * _speed;  //* Time.deltaTime;
        _rotation += _steering.angular; //  * Time.deltaTime;

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

    public override void SetSteering(Steering steering)
    {
        _steering = steering;
    } 
}
