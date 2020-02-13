using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private Text _scoreText;
    [SerializeField] private Image _livesImage;
    [SerializeField] private Text _gameoverText;
    [SerializeField] private Sprite[] _livesSprites;
    [SerializeField] private Text _restartText;
    [SerializeField] private Text _remainingShieldText;
    [SerializeField] private Text _ammoCountText;
    
    [SerializeField] private Slider _thrusterSlider;
    [SerializeField] private bool _isThrusterBoost = false;
    private bool _isThrusterBoostEnabled = true;

    private GameManager _gameManager;

    private bool _isGameOver = false;
    
    // Start is called before the first frame update
    void Start()
    {
         _gameoverText.gameObject.SetActive(false);
         _gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
         if (_gameManager == null)
         {
             Debug.Log("_gameManager is null");
         }

         StartCoroutine(DecreaseThrusterBoost());
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void SetScore(int score)
    {
        _scoreText.text = "Score: " + score;
    }

    public void SetLives(int lives)
    {
        if (lives >= 0 && lives <= 3)
        {
            _livesImage.sprite = _livesSprites[lives];
        }
        
        if (lives == 0)
        {
            GameOverSequence();
        }
    }

    // Phase I: Framework - Quiz - Shield Strength
    public void SetRemainingShield(int shields)
    {
        if (shields == 0)
        {
            _remainingShieldText.gameObject.SetActive(false);
        }
        else
        {
            _remainingShieldText.text = "Remaining Shield: " + shields;
            _remainingShieldText.gameObject.SetActive(true);
        }
    }

    // Phase I: Framework - Quiz - Ammo Count
    public void SetAmmoCount(int ammo)
    {
        _ammoCountText.text = "Ammo: " + ammo;
    }
    
    void GameOverSequence()
    {
        _gameoverText.gameObject.SetActive(true);
        _restartText.gameObject.SetActive(true);
        _isGameOver = true;
        _gameManager.GameOver();
    }

    IEnumerator BlinkingGameOverRoutine()
    {
        while (_isGameOver)
        {
            _gameoverText.text = "GAME OVER";
            yield return new WaitForSeconds(0.5f);
            _gameoverText.text = "";
            yield return new WaitForSeconds(0.5f);
        }
    }

    public void SetThrusterBoostActive()
    {
        if (_isThrusterBoostEnabled)
        {
            _isThrusterBoost = true;
            _isThrusterBoostEnabled = false;

            StartCoroutine(DecreaseThrusterBoost());
        }
    }

    public bool IsThrusterBoostActive()
    {
        return _isThrusterBoost;
    }

    public void ResetThrusterBoost()
    {
        _isThrusterBoost = false;
        StartCoroutine(IncreaseThrusterBoost());
    }

    // Phase I: Framework - Quiz - Thruster: Scaling Bar HUD
    IEnumerator DecreaseThrusterBoost()
    {
        while (_isThrusterBoost)
        {
            if (_thrusterSlider.value > 0)
            {
                _thrusterSlider.value -= 0.02f;
                if (_thrusterSlider.value <= 0)
                {
                    _thrusterSlider.value = 0;
                    _isThrusterBoost = false;
                }
                else
                {
                    yield return new WaitForSeconds(0.1f);
                }
            }
            else
            {
                _thrusterSlider.value = 0;
                _isThrusterBoost = false;
            }
        }
    }

    IEnumerator IncreaseThrusterBoost()
    {
        while (!_isThrusterBoost && !_isThrusterBoostEnabled)
        {
            if (_thrusterSlider.value < 1.0f)
            {
                _thrusterSlider.value += 0.02f;
                if (_thrusterSlider.value >= 1.0f)
                {
                    _thrusterSlider.value = 1.0f;
                    _isThrusterBoostEnabled = true;
                }
                else
                {
                    yield return new WaitForSeconds(0.1f);
                }
            }
            else
            {
                _thrusterSlider.value = 1.0f;
                _isThrusterBoostEnabled = true;
            }
        }
        
    }
}