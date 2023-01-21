using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreUI : MonoBehaviour
{   
    
    public int score = 0;
    private Text scoreText;
    public GameObject scoreObj;
    
    void Start()
    {
        scoreText = scoreObj.GetComponent<Text>();
        scoreText.text = "Score: 0";
    }
    
    public void UpdateScore()
    {
        score += 10;
        updateScoreText();
    }
    
    public void updateScoreText()
    {
        scoreText.text = "Score: " + score;
    }
}
