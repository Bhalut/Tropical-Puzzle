using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    private static UIManager _instance;
    public static UIManager Instance => _instance;

    public Text timerText;
    public Text scoreText;
    public Text highscoreText;

    public Slider progressSlider;
    public RectTransform menuResults;
    
    private float _timerCounter;
    private int _score;

    public int Score
    {
        get => _score;

        set
        {
            _score = value;
            scoreText.text = _score.ToString();
        }
    }

    public float TimerCounter
    {
        get => _timerCounter;
    }

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
    }

    private void Start()
    {
        _score = 0;
        _timerCounter = 180.0f;
        progressSlider.maxValue = _timerCounter;
        scoreText.text = $"{_score}";
        timerText.text = $"{_timerCounter}";
    }

    private void Update()
    {
        if (_timerCounter > 0)
        {
            var min = ((int)_timerCounter/60).ToString("00");
            var seg = (_timerCounter % 60).ToString("00");

            timerText.text = $"{min}:{seg}";
            progressSlider.value = _timerCounter;
            _timerCounter -= Time.deltaTime;
        }
        else
        {
            StartCoroutine(GameOver());
            Debug.Log("Time OFF");
        }
    }

    private IEnumerator GameOver()
    {
        yield return new WaitUntil(() => !GridManager.Instance.IsShifting);
        yield return new WaitForSeconds(0.25f);

        highscoreText.text = scoreText.text; 
        menuResults.DOAnchorPos(Vector2.zero, 0.25f);
        Debug.Log("Final");
    }
}
