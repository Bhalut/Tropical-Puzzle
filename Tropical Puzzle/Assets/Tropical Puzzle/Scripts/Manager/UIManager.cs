using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    private static UIManager _instance;
    public static UIManager Instance
    {
        get
        {
            return _instance;
        }
    }

    public Text moveText;
    public Text scoreText;
    private int _moveCounter;
    private int _score;

    public int Score
    {
        get
        {
            return _score;
        }

        set
        {
            _score = value;
            scoreText.text = $"SCORE: {_score}";
        }
    }

    public int MoveCounter
    {
        get
        {
            return _moveCounter;
        }

        set
        {
            if (_moveCounter <= 0)
            {
                _moveCounter = 0;
                StartCoroutine(GameOver());
            }
            _moveCounter = value;
            moveText.text = $"MOVE: {_moveCounter}";
        }
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
        _moveCounter = 30;
        scoreText.text = $"SCORE: {_score}";
        moveText.text = $"MOVE: {_moveCounter}";
    }

    private IEnumerator GameOver()
    {
        yield return new WaitUntil(() => !GridManager.Instance.IsShifting);
        yield return new WaitForSeconds(0.25f);

        //TODO
    }
}
