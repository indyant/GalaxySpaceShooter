using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewEnemy : Enemy
{
    [SerializeField] private GameObject _missilePrefab;
    [SerializeField] private bool _hasShield;
    [SerializeField] private GameObject _shieldVisualizer;
    private GameObject _enemyContainer;
    
    // Start is called before the first frame update
    enum NewEnemyType
    {
        EnemyType1,
        EnemyType2,
        EnemyType3,
    };
    
    [SerializeField] private NewEnemyType _enemyType;
    
    public override void Start()
    {
        Initialize();
        
        _enemyContainer = GameObject.Find("EnemyContainer");
        if (_enemyContainer == null)
        {
            Debug.LogError("_enemyContainer is null");
        }
        
        ObjectMovementZigzag objectMovementZigzag;
        ObjectMovementWave objectMovementWave;
        SetShield(false);

        switch (_enemyType)
        {
            case NewEnemyType.EnemyType1: // zigzag
                objectMovementZigzag = gameObject.AddComponent<ObjectMovementZigzag>();
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
                objectMovementZigzag.SetZigzag(direction, 1.3f);
                break;
            
            case NewEnemyType.EnemyType2:
                objectMovementWave = gameObject.AddComponent<ObjectMovementWave>();
                objectMovementWave.SetWave(1f, 1.5f);
                break;
            
            case NewEnemyType.EnemyType3:
                objectMovementWave = gameObject.AddComponent<ObjectMovementWave>();
                objectMovementWave.SetWave(1f, 3f);
                SetShield();
                break;
            
            default:
                Debug.LogError("Invalid dirSwitch");
                break;
        }
        _fireRate = Random.Range(2.0f, 4.0f);
        _canFire = Time.time + _fireRate;
    }

    private void SetShield(bool hasShield = true)
    {
        _hasShield = hasShield;
        _shieldVisualizer.SetActive(_hasShield);
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
                    Vector3 laserPosition = transform.position;
                    laserPosition.y -= 0.6f;
                    GameObject enemyLaser =
                        Instantiate(_laserPrefab, laserPosition, Quaternion.identity);
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
                    enemyMissile.transform.parent = _enemyContainer.transform;
                    break;
                default:
                    Debug.LogError("Invlaid FireSwitch");
                    break;
            }
        }
        
        if (!_isFirePickup && CheckPickupAhead())
        {
            _isFirePickup = true;
            
            Vector3 laserPosition = transform.position;
            laserPosition.y -= 0.6f;
            GameObject enemyLaser = Instantiate(_laserPrefab, laserPosition, Quaternion.identity);
            Laser laser = enemyLaser.GetComponent<Laser>();
            laser.AssignEnemyLaser();
        }
        
    }

    public override void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Laser")
        {
            if (_hasShield)
            {
                SetShield(false);
            }
            else
            {
                Destroy(other.gameObject);
                _player.AddScore(10);
                _anim.SetTrigger("OnEnemyDeath");
                _speed = 0;
                _audioSource.Play();
                Destroy(GetComponent<Collider2D>());
                _canFire += 10.0f;
                Destroy(gameObject, 1.0f);
            }
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
            Destroy(gameObject, 1.0f);
        }
    }
}
