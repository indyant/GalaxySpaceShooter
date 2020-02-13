using System.Collections;
using System.Runtime.InteropServices;
using UnityEditor;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private float _fireRate = 0.3f;
    [SerializeField] private GameObject _laserPrefab;
    [SerializeField] private GameObject _tripleShotPrefab;
    [SerializeField] private GameObject _multiDirectionalShotPrefab;
    private readonly float _laserSpawnOffset = 0.85f;
    private float _nextFire = -1f;
    [SerializeField] private float _speed = 3.0f;
    [SerializeField] int _lives = 3;
    private SpawnManager _spawnManager;
    
    [SerializeField] private bool _isTripleShotActive = false;
    [SerializeField] private bool _isSpeedupActive = false;
    // [SerializeField] private bool _isShieldActive = false;
    
    // Phase I: Framework - Quiz - Secondary Fire Powerup
    [SerializeField] private bool _isMultiShotActive = false;
    
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

    // Phase I: Framework - Quiz - Ammo Count
    [SerializeField] private int _ammoCount = 15;

    [SerializeField] private GameObject _mainCamera;
    private CameraShake _camera;
    
    
    // Start is called before the first frame update
    private void Start()
    {
        transform.position = new Vector3(0, 0, 0);
        // _shieldVisualizer.SetActive(false);
        _spawnManager = GameObject.Find("SpawnManager").GetComponent<SpawnManager>();
        if (_spawnManager == null)
        {
            Debug.LogError("_spawnManager is null");
        }
        
        _uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();
        if (_uiManager == null)
        {
            Debug.LogError("_uiManager is null");
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
        _shieldCount = 0;
        
        // Phase I: Framework - Quiz - Ammo Count
        _ammoCount = 15;
        _uiManager.SetAmmoCount(_ammoCount);

        _camera = GameObject.Find("Main Camera").GetComponent<CameraShake>();
        if (_camera == null)
        {
            Debug.LogError("_camera is null");
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

        // Phase I: Framework - Quiz - Thrusters 
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            _isThrusterBoost = true;
            _uiManager.SetThrusterBoostActive();
        }
        else if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            _isThrusterBoost = false;
            _uiManager.ResetThrusterBoost();
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
            if (_uiManager.IsThrusterBoostActive())
            {
                speed = speed * _increasedRate;
            }
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

    // Phase I: Framework - Quiz - Ammo Count
    private void FireLaser()
    {
        _nextFire = Time.time + _fireRate;

        bool isAmmoAvailable = false;
        
        if (_isTripleShotActive || _isMultiShotActive)
        {
            if (_ammoCount > 3)
            {
                isAmmoAvailable = true;
            }
        }
        else
        {
            if (_ammoCount > 0)
            {
                isAmmoAvailable = true;
            }
        }

        if (isAmmoAvailable)
        {
            if (_isTripleShotActive)
            {
                var laserPos = new Vector3(transform.position.x - 0.15f, transform.position.y - 0.65f,
                    transform.position.z);
                Instantiate(_tripleShotPrefab, laserPos, Quaternion.identity);
                _ammoCount -= 3;
            }
            else if (_isMultiShotActive)
            {
                var laserPos = new Vector3(transform.position.x - 0.15f, transform.position.y - 0.65f,
                    transform.position.z);
                Instantiate(_multiDirectionalShotPrefab, laserPos, Quaternion.identity);
                _ammoCount -= 3;
            }
            else
            {
                var laserPos = new Vector3(transform.position.x, transform.position.y + _laserSpawnOffset,
                    transform.position.z);
                Instantiate(_laserPrefab, laserPos, Quaternion.identity);
                _ammoCount--;
            }
            _audioSource.Play();
        }
        else
        {
            EditorApplication.Beep();
        }

        _uiManager.SetAmmoCount(_ammoCount);
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
        }
        else
        {
            _lives -= damageLevel;

            if (_lives < 0)
            {
                _lives = 0;
            }
            
            // Phase I: Framework - Quiz - Camera Shake
            StartCoroutine(_camera.Shake(0.5f, 0.3f));
            DisplayDamage();
        }
    }
    
    private void DisplayDamage()
    {
        switch (_lives)
        {
            case 3:
                _leftEngineFire.SetActive(false);
                _rightEngineFire.SetActive(false);
                break;
            case 2:
                _leftEngineFire.SetActive(false);
                _rightEngineFire.SetActive(true);
                break;
            case 1:
                _leftEngineFire.SetActive(true);
                _rightEngineFire.SetActive(true);
                break;
            case 0:
                _spawnManager.OnPlayerDeath();
                Destroy(gameObject);
                break;
            default:
                Debug.LogError("DisplayDamage: invalid _lives");
                break;
        }

        _uiManager.SetLives(_lives);
    }

    public void EnableTripleShot(bool isTripleShot = true)
    {
        _isTripleShotActive = isTripleShot;
        StartCoroutine(TripleShotPowerDownRoutine());
    }

    // Phase I: Framework - Quiz - Secondary Fire Powerup
    public void EnableMultiShot(bool isMultiShot = true)
    {
        if (isMultiShot)
        {
            _isTripleShotActive = false;
            _isMultiShotActive = true;
            StartCoroutine(MultiShotPowerDownRoutine());
        }
    }

    IEnumerator TripleShotPowerDownRoutine()
    {
        yield return new WaitForSeconds(5.0f);
        _isTripleShotActive = false;
    }
    
    // Phase I: Framework - Quiz - Secondary Fire Powerup
    IEnumerator MultiShotPowerDownRoutine()
    {
        yield return new WaitForSeconds(5.0f);
        _isMultiShotActive = false;
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

    // Phase I: Framework - Quiz - Ammo Collectable
    public void AddAmmo()
    {
        _ammoCount += 15;
        _uiManager.SetAmmoCount(_ammoCount);
    }

    // Phase I: Framework - Quiz - Health Collectable
    public void RecoverHealth()
    {
        if (_lives < 3)
        {
            _lives += 1;
            _uiManager.SetLives(_lives);
            DisplayDamage();
        }
    }
}