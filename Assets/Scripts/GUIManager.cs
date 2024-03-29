﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GUIManager : MonoBehaviour
{

    public Text movesText, scoreText;
    private int moveCounter;
    private int score;


    public int Score
    {
        get
        {
            return score;
        }

        set
        {
            score = value;
            scoreText.text = "Score: " + score;
        }
    }

    public int MoveCounter
    {
        get { return moveCounter; }
        set
        {
            moveCounter = value;
            movesText.text = "Moves: " + moveCounter;
            if (moveCounter <= 0)
            {
                StartCoroutine(GameOver());
            }
        }
    }

    public static GUIManager sharedInstance;
    // Start is called before the first frame update
    void Start()
    {

        if (sharedInstance == null)
        {
            sharedInstance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        score = 0;
        moveCounter = 15;
        movesText.text = "Moves: " + moveCounter;
        scoreText.text = "Score: " + score;
    }

    private IEnumerator GameOver()
    {
        yield return new WaitForSeconds(1.5f);

        PlayerPrefs.SetInt("FinalScore", score);
        SceneManager.LoadScene("GameOver");
    }


}
