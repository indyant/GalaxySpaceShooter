using System.Collections;
using System.Runtime.InteropServices;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private float _fireRate = 0.3f;
    [SerializeField] private GameObject _laserPrefab;
    [SerializeField] private GameObject _tripleShotPrefab;
    private readonly float _laserSpawnOffset = 0.85f;
    private float _nextFire = -1f;
    [SerializeField] private float _speed = 3.0f;
    [SerializeField] int _lives = 3;
    private SpawnManager _spawnManager;
    
    [SerializeField] private bool _isTripleShotActive = false;
    [SerializeField] private bool _isSpeedupActive = false;
    // [SerializeField] private bool _isShieldActive = false;
    
    // Phase I: Framework - Quiz - Shield Strength
    [SerializeField] private int _shieldCount = 3;
    private Color[] _shieldHitColor;
    
    [SerializeField] private GameObject _shieldVisualizer;

    // Phase I: Framework - Quiz - Thrusters
    [SerializeField] private bool _isThrusterBoost = false;
    [SerializeField] private float _increasedRate = 2.0f;

    [SerializeField] private int _score = 0;

    private float _speedBoostMultiplier = 2.0f;
    private UIManager _uiManager;
    [SerializeField] private GameObject _rightEngineFire;
    [SerializeField] private GameObject _leftEngineFire;

    [SerializeField] AudioClip _laserSoundClip;
    [SerializeField] private AudioClip _explosionSoundClip;
    [SerializeField] private AudioSource _audioSource;
    
    // Start is called before the first frame update
    private void Start()
    {
        transform.position = new Vector3(0, 0, 0);
        // _shieldVisualizer.SetActive(false);
        _spawnManager = GameObject.Find("SpawnManager").GetComponent<SpawnManager>();
        if (_spawnManager == null)
        {
            Debug.Log("_spawnManager is null");
        }
        
        _uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();
        if (_uiManager == null)
        {
            Debug.Log("_uiManager is null");
        }

        _audioSource = GetComponent<AudioSource>();
        if (_audioSource == null)
        {
            Debug.LogError("_audioSource is null");
        }
        else
        {
            _audioSource.clip = _laserSoundClip;
        }
        
        // Phase I: Framework - Quiz - Shield Strength
        _shieldHitColor = new Color[3];
        _shieldHitColor[0] = Color.white;// new Color(255, 255, 255 );
        _shieldHitColor[1] = Color.blue; // new Color(0, 255, 0);
        _shieldHitColor[2] = Color.red; // new Color(75, 56, 54);
    }

    // Update is called once per frame
    private void Update()
    {
        CalculateMovement();

        if (Input.GetKeyDown(KeyCode.Space) && Time.time > _nextFire)
        {
            FireLaser();
        }

        // Phase I: Framework - Quiz - Thrusters 
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            _isThrusterBoost = true;
        }
        else if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            _isThrusterBoost = false;
        }
    }

    private void CalculateMovement()
    {
        var horizontalInput = Input.GetAxis("Horizontal");
        var verticalInput = Input.GetAxis("Vertical");

        var direction = new Vector3(horizontalInput, verticalInput, 0);
        float speed = _speed;

        if (_isSpeedupActive)
        {
            speed = _speed * _speedBoostMultiplier;
        }
        
        // Phase I: Framework - Quiz - Thrusters   
        if (_isThrusterBoost)
        {
            speed = speed * _increasedRate;
        }

        transform.Translate(direction * speed * Time.deltaTime);

        transform.position = new Vector3(transform.position.x, Mathf.Clamp(transform.position.y, -4.8f, 0), transform.position.z);

        if (transform.position.x > 11.3)
        {
            transform.position = new Vector3(-11.0f, transform.position.y, transform.position.z);
        }
        else if (transform.position.x < -11.3)
        {
            transform.position = new Vector3(11.0f, transform.position.y, transform.position.z);
        }
    }

    private void FireLaser()
    {
        _nextFire = Time.time + _fireRate;

        if (_isTripleShotActive)
        {
            var laserPos = new Vector3(transform.position.x-0.15f, transform.position.y-0.65f, transform.position.z);
            Instantiate(_tripleShotPrefab, laserPos, Quaternion.identity);
        }
        else
        {
            var laserPos = new Vector3(transform.position.x, transform.position.y + _laserSpawnOffset, transform.position.z);
            Instantiate(_laserPrefab, laserPos, Quaternion.identity);
        }

        _audioSource.Play();
    }

    // Phase I: Framework - Quiz - Shield Strength
    public void Damage(int damageLevel = 1)
    {
        SpriteRenderer spriteRenderer;
        if (_shieldCount > 0)
        {
            // _isShieldActive = false;
            _shieldCount--;
            _uiManager.SetRemainingShield(_shieldCount);    
            
            if (_shieldCount == 0)
            {
                _shieldVisualizer.SetActive(false);
            }
            else
            {
                spriteRenderer = _shieldVisualizer.GetComponent<SpriteRenderer>();
                spriteRenderer.color = _shieldHitColor[3 - _shieldCount];
            }
            return;
        }
        
        _lives -= damageLevel;

        if (_lives == 2)
        {
            _rightEngineFire.SetActive(true);
        }
        else if (_lives == 1)
        {
            _leftEngineFire.SetActive(true);
        }
        else if (_lives < 1)
        {
            _spawnManager.OnPlayerDeath();
//            _audioSource.clip = _explosionSoundClip;
//            _audioSource.Play();

            Destroy(gameObject);
        }

        _uiManager.SetLives(_lives);
    }

    public void EnableTripleShot(bool isTripleShot = true)
    {
        _isTripleShotActive = isTripleShot;
        StartCoroutine(TripleShotPowerDownRoutine());
    }

    IEnumerator TripleShotPowerDownRoutine()
    {
        yield return new WaitForSeconds(5.0f);
        _isTripleShotActive = false;
    }

    public void EnableSpeedup()
    {
        _isSpeedupActive = true;
        _speed = 6.0f;
        StartCoroutine(SpeedDownRoutine());
    }

    IEnumerator SpeedDownRoutine()
    {
        yield return new WaitForSeconds(5.0f);
        _isSpeedupActive = false;
        _speed = 3.0f;
    }

    public void EnableShield()
    {
        // _isShieldActive = true;
        _shieldCount = 3;
        _shieldVisualizer.SetActive(true);
        _uiManager.SetRemainingShield(_shieldCount);
        
        SpriteRenderer spriteRenderer = _shieldVisualizer.GetComponent<SpriteRenderer>();
        spriteRenderer.color = _shieldHitColor[0];
    }

    public void AddScore(int points)
    {
        _score += points;
        _uiManager.SetScore(_score);
    }
}