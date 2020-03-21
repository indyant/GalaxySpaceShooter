using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

public class EnemyBoss : MonoBehaviour
{
    enum EnemyBossMove
    {
        Invalid,
        Appearance,
        HorizontalMove
    };

    private EnemyBossMove _currentMove = EnemyBossMove.Invalid;
    private float _speed = 3.0f;
    private const float ReadyPositionY = 2.5f;
    private const float LeftBound = -5.5f;
    private const float RightBound = 5.5f;
    private Vector3 _currentDirection;
    private int _acceptableDamage = 20;
    private GameObject _playerObject;
    private Player _player;
    private Animator _anim;
    private AudioSource _audioSource;
    [SerializeField] protected AudioClip _explosionSoundClip;
    private bool _isPreparing = false;
    private SpawnManager _spawnManager;
    [SerializeField] private GameObject _laserPrefab;
    private float _canFire = 0f;
    private float _fireRate = 0.3f;
    private float _fireDirectionChangeRate = 0.15f;
    private int _currentFireDirection = 0;  // 0: left, 1: right
    private Vector3 _fireVector;
    
    // Start is called before the first frame update
    void Start()
    {
        _spawnManager = GameObject.Find("SpawnManager").GetComponent<SpawnManager>();
        if (_spawnManager == null)
        {
            Debug.LogError("_spawnManager is null");
        }
        
        _currentMove = EnemyBossMove.Appearance;
        int startDirection = Random.Range(0, 2);
        if (startDirection == 0)
        {
            _currentDirection = Vector3.left;
        }
        else
        {
            _currentDirection = Vector3.right;
        }
        
        _playerObject = GameObject.Find("Player");
        if (_playerObject != null)
        {
            _player = _playerObject.GetComponent<Player>();
            if (_player == null)
            {
                Debug.LogError("_player is null");
            }
        }
        else
        {
            Debug.LogError("_playerObject is null");
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
        
        _canFire = Time.time + _fireRate;
        _fireVector = new Vector3(0f, -1f, 0f);
    }

    // Update is called once per frame
    void Update()
    {
        switch (_currentMove)
        {
            case EnemyBossMove.Appearance:
                Appear();
                break;
            case EnemyBossMove.HorizontalMove:
                MoveAround();
                if (Time.time > _canFire)
                {
                    _canFire = Time.time + _fireRate;
                    FireLaser();
                }

                break;
            default:
                Debug.LogError("Invalid Boss move mode");
                break;
        }
    }

    private void Appear()
    {
        if (transform.position.y <= ReadyPositionY)
        {
            if (!_isPreparing)
            {
                _isPreparing = true;
                StartCoroutine(WaitToStartRoutine());
            }
        }
        else
        {
            transform.Translate(Vector3.down * _speed * Time.deltaTime);
        }
    }

    IEnumerator WaitToStartRoutine()
    {
        yield return new WaitForSeconds(1.5f);
        _currentMove = EnemyBossMove.HorizontalMove;
    }

    private void MoveAround()
    {
        if (_currentDirection == Vector3.left)
        {
            if (transform.position.x > LeftBound)
            {
                transform.Translate(_currentDirection * _speed * Time.deltaTime);
            }
            else
            {
                _currentDirection = Vector3.right;
            }
        }
        else
        {
            if (transform.position.x < RightBound)
            {
                transform.Translate(_currentDirection * _speed * Time.deltaTime);
            }
            else
            {
                _currentDirection = Vector3.left;
            }
        }
    }

    private void FireLaser()
    {
        Vector3 laserPosition = new Vector3(transform.position.x, transform.position.y - 2.4f, 0f);
        GameObject laserObject = Instantiate(_laserPrefab, laserPosition, Quaternion.identity);
        Laser laser = laserObject.GetComponent<Laser>();
        if (_currentFireDirection == 0)
        {
            _fireVector.x -= _fireDirectionChangeRate;
            if (_fireVector.x < -1)
            {
                _currentFireDirection = 1;
            }
        }
        else
        {
            _fireVector.x += _fireDirectionChangeRate;
            if (_fireVector.x > 1)
            {
                _currentFireDirection = 0;
            }
        }

        _fireVector.Normalize();
        laser.SetDirection(_fireVector);
        laser.AssignEnemyLaser();
    }
    
    public virtual void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Laser")
        {
            _acceptableDamage--;
            
            Destroy(other.gameObject);
            
            if (_acceptableDamage <= 0)
            {
                _player.AddScore(100);
                _anim.SetTrigger("OnEnemyBossDeath");
                _speed = 0;
                _audioSource.Play();
                _spawnManager.OnEnemyBossDeath();
                Destroy(GetComponent<Collider2D>());
                Destroy(gameObject, 2.8f);
            }
        }
        else if (other.tag == "Player")
        {
            _acceptableDamage--;

            Player player = other.GetComponent<Player>();
            if (player != null)
            {
                player.Damage();
            }
            
            if (_acceptableDamage <= 0)
            {
                _anim.SetTrigger("OnEnemyBossDeath");
                _speed = 0;
                _audioSource.Play();
                Destroy(gameObject, 2.8f);
            }
        }
    }

    
}
