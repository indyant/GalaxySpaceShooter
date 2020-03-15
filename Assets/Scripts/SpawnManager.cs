using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

public class SpawnManager : MonoBehaviour
{
    // [SerializeField] private GameObject _enemyPrefab;
    [SerializeField] private GameObject[] _enemyPrefab;
    private float _spawnDelayMin = 3.0f;
    private float _spawnDelayMax = 5.0f; 
    
    private float _waveDelayMin = 30.0f;
    private float _waveDelayMax = 40.0f;
    [SerializeField] private int _enemyWaveAmount = 4;
    
    [SerializeField] private GameObject _enemyContainer;
    [SerializeField] private GameObject _pickupContainer;
    [SerializeField] private GameObject[] _powerups;
    private Dictionary<Powerup.PowerUpType, int> _powerupIntervals = new Dictionary<Powerup.PowerUpType, int>()
    {
        {Powerup.PowerUpType.TripleShot, 8},
        {Powerup.PowerUpType.SpeedUp, 8},
        {Powerup.PowerUpType.ShieldUp, 10},
        {Powerup.PowerUpType.Ammo, 3},
        {Powerup.PowerUpType.Health, 15},
        {Powerup.PowerUpType.MultiShotPowerUp, 10},
        {Powerup.PowerUpType.FireSpeedDebuff, 5},
        {Powerup.PowerUpType.MovementDebuff, 5}
    };

    private bool _stopSpawning = false;
    private float _leftBound = -9.0f;
    private float _rightBound = 9.0f;
    private float _waveLevel = 1;
    
    // Start is called before the first frame update
    void Start()
    {
    }

    public void StartSpawning()
    {
        StartCoroutine(SpawnEnemyRoutine());
        StartCoroutine(SpawnEnemyWave());
        foreach (var powerup in _powerupIntervals)
        {
            StartCoroutine(SpawnPowerupRoutine(powerup.Key, powerup.Value));
        }
    }

    // Update is called once per frame
    void Update()
    {
    }

    IEnumerator SpawnEnemyRoutine()
    {
        yield return new WaitForSeconds(3.0f);

        int enemyType = 0;
        while (_stopSpawning == false)
        {
            enemyType = Random.Range(0, 5);
            Vector3 enemyPosition = new Vector3(Random.Range(_leftBound, _rightBound), 6, 0);
            GameObject newEnemy = Instantiate(_enemyPrefab[enemyType], enemyPosition, Quaternion.identity);
            newEnemy.transform.parent = _enemyContainer.transform;
            float spawnDelay = Random.Range(_spawnDelayMin, _spawnDelayMax);
            yield return new WaitForSeconds(spawnDelay);
        }
    }

    IEnumerator SpawnEnemyWave()
    {
        yield return new WaitForSeconds(Random.Range(_waveDelayMin, _waveDelayMax));

        int enemyWaveCount;
        float x = 0f;
        float y = 0f;
        int enemyType = 0;
        
        while (_stopSpawning == false)
        {
            enemyWaveCount = (int) (_enemyWaveAmount * _waveLevel * 1.2);
            _waveLevel++;
            for (int i = 0; i < enemyWaveCount; i++)
            {
                x = Random.Range(_leftBound, _rightBound);
                y = Random.Range(6.0f, 8.0f);
                enemyType = Random.Range(0, 4);
                Vector3 enemyPosition = new Vector3(Random.Range(_leftBound, _rightBound), y, 0);
                GameObject newEnemy = Instantiate(_enemyPrefab[enemyType], enemyPosition, Quaternion.identity);
                newEnemy.transform.parent = _enemyContainer.transform;
            }
            yield return new WaitForSeconds(Random.Range(_waveDelayMin, _waveDelayMax));
        }
    }

    IEnumerator SpawnPowerupRoutine(Powerup.PowerUpType type, float minInterval)
    {
        yield return new WaitForSeconds(_powerupIntervals[type]);
        int powerupIndex = 0;

        while (_stopSpawning == false)
        {
            yield return new WaitForSeconds(Random.Range(minInterval, minInterval + 5.0f));
            Vector3 powerupPosition = new Vector3(Random.Range(_leftBound, _rightBound), 6, 0);
            powerupIndex = getPowerUpIndex(type);
            if (powerupIndex != -1)
            {
                GameObject newPowerup = Instantiate(_powerups[powerupIndex], powerupPosition, Quaternion.identity);
                newPowerup.transform.parent = _pickupContainer.transform;
            }
        }
    }

    int getPowerUpIndex(Powerup.PowerUpType type)
    {
        int powerupIndex = -1;
        
        switch (type)
        {
            case Powerup.PowerUpType.TripleShot:
                powerupIndex = 0;
                break;
            case Powerup.PowerUpType.SpeedUp:
                powerupIndex = 1;
                break;
            case Powerup.PowerUpType.ShieldUp:
                powerupIndex = 2;
                break;
            case Powerup.PowerUpType.Ammo:
                powerupIndex = 3;
                break;
            case Powerup.PowerUpType.Health:
                powerupIndex = 4 ;
                break;
            case Powerup.PowerUpType.MultiShotPowerUp:
                powerupIndex = 5;
                break;
            case Powerup.PowerUpType.FireSpeedDebuff:
                powerupIndex = 6;
                break;
            case Powerup.PowerUpType.MovementDebuff:
                powerupIndex = 7;
                break;
            default:
                Debug.LogError("Invalid PowerUpType (" + type + ")");
                break;
        }

        return powerupIndex;
    }

    public void OnPlayerDeath()
    {
        _stopSpawning = true;
    }
}
