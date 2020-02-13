using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

public class SpawnManager : MonoBehaviour
{
    [SerializeField] private GameObject _enemyPrefab;
    [SerializeField] private float _spawnDelay = 3.0f;
    [SerializeField] private GameObject _enemyContainer;
    [SerializeField] private GameObject[] _powerups;

    private bool _stopSpawning = false;
    private float _leftBound = -9.0f;
    private float _rightBound = 9.0f;
    
    // Start is called before the first frame update
    void Start()
    {
    }

    public void StartSpawning()
    {
        StartCoroutine(SpawnEnemyRoutine());
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
        
        while (_stopSpawning == false)
        {
            Vector3 enemyPosition = new Vector3(Random.Range(_leftBound, _rightBound), 6, 0);
            GameObject newEnemy = Instantiate(_enemyPrefab, enemyPosition, Quaternion.identity);
            newEnemy.transform.parent = _enemyContainer.transform;
            yield return new WaitForSeconds(_spawnDelay);
        }
    }

    IEnumerator SpawnPowerupRoutine()
    {
        yield return new WaitForSeconds(3.0f);
        
        while (_stopSpawning == false)
        {
            yield return new WaitForSeconds(Random.Range(3.0f, 7.0f));
            Vector3 powerupPosition = new Vector3(Random.Range(_leftBound, _rightBound), 6, 0);
            int randomPowerup = Random.Range(0, 5);
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
