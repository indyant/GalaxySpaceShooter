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
    [SerializeField] private bool _isShieldActive = false;
    [SerializeField] private GameObject _shieldVisualizer;

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
    }

    // Update is called once per frame
    private void Update()
    {
        CalculateMovement();

        if (Input.GetKeyDown(KeyCode.Space) && Time.time > _nextFire)
        {
            FireLaser();
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

    public void Damage(int damageLevel = 1)
    {
        if (_isShieldActive)
        {
            _isShieldActive = false;
            _shieldVisualizer.SetActive(false);
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
        _isShieldActive = true;
        _shieldVisualizer.SetActive(true);
    }

    public void AddScore(int points)
    {
        _score += points;
        _uiManager.SetScore(_score);
    }
}