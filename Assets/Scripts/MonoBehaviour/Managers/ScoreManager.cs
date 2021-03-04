using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(menuName = "Game Manager/Score")]
public class ScoreManager : ScriptableObject
{
    private int _score;
     public Text scoreText;
    
    public void SetScore(int number)
    {
        if (_score + number < 99999)
        {
            _score += number;
        }
        else
        {
            _score = 99999;
        }
        UpdateText();
    }

    public void UpdateText()
    {
        scoreText.text = _score.ToString(); 
    }

    public int GetScore()
    {
        return _score;
    }

    private void OnEnable()
    {
        _score = 0;
        scoreText = null;
    }
}
