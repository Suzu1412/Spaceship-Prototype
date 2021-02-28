using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(menuName = "Game Manager/Score")]
public class ScoreManager : ScriptableObject
{
    private int _score;
    public string player;
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
        scoreText.text = player + " Score: " + _score.ToString(); 
    }

    private void OnEnable()
    {
        _score = 0;
    }
}
