using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private Text _scoreText;
    [SerializeField] private Image _livesImage;
    [SerializeField] private Text _gameoverText;
    [SerializeField] private Sprite[] _livesSprites;
    [SerializeField] private Text _restartText;
    [SerializeField] private Text _remainingShieldText;

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

    void GameOverSequence()
    {
        _gameoverText.gameObject.SetActive(true);
        _restartText.gameObject.SetActive(true);
        _isGameOver = true;
        _gameManager.GameOver();
        StartCoroutine(BlinkingGameOverRoutine());
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
}