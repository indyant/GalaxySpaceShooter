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
    [SerializeField] private int _enemyWaveAmount = 6;
    
    [SerializeField] private GameObject _enemyContainer;
    [SerializeField] private GameObject[] _powerups;

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
        StartCoroutine(SpawnPowerupRoutine());
        StartCoroutine(SpawnNewPowerupRoutine());
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
            enemyType = Random.Range(0, 4);
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

    IEnumerator SpawnPowerupRoutine()
    {
        yield return new WaitForSeconds(3.0f);
        
        while (_stopSpawning == false)
        {
            yield return new WaitForSeconds(Random.Range(3.0f, 5.0f));
            Vector3 powerupPosition = new Vector3(Random.Range(_leftBound, _rightBound), 6, 0);
            int randomPowerup = Random.Range(0, 10);
            GameObject newPowerup = Instantiate(_powerups[randomPowerup], powerupPosition, Quaternion.identity);
            newPowerup.transform.parent = transform;
        }
    }

    IEnumerator SpawnNewPowerupRoutine()
    {
        yield return new WaitForSeconds(10.0f);
        
        while (_stopSpawning == false)
        {
            yield return new WaitForSeconds(Random.Range(10.0f, 20.0f));
            Vector3 powerupPosition = new Vector3(Random.Range(_leftBound, _rightBound), 6, 0);
            GameObject newPowerup = Instantiate(_powerups[5], powerupPosition, Quaternion.identity);
            newPowerup.transform.parent = transform;
        }
    }
    
    public void OnPlayerDeath()
    {
        _stopSpawning = true;
    }
}
