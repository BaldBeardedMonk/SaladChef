/* GameController script that handles the timing and score for players as well as winning and game over conditions */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class GameController : MonoBehaviour
{
    public float gameTime;                  //set this in the inspector.
    public GameObject FinalPanel;
    public Text Player1ScoreText, Player2ScoreText, WinnerText;
    [HideInInspector] public bool player1TimeUp, player2TimeUp;
    [HideInInspector] public int player1Score, player2Score;
    void Start()
    {
        player1TimeUp = player2TimeUp = false;
    }

    public void CheckGameOver()
    {
        if (player1TimeUp == true && player2TimeUp == true) GameOver();
    }

    void GameOver()
    {
        Time.timeScale = 0;
        FinalPanel.SetActive(true);
        Player1ScoreText.text = "Player 1 score = " + player1Score.ToString();
        Player2ScoreText.text = "Player 2 score = " + player2Score.ToString();
        if (player1Score > player2Score) WinnerText.text = "WINNER IS : PLAYER 1";
        else if (player2Score > player1Score) WinnerText.text = "WINNER IS : PLAYER 2";
        else WinnerText.text = "GAME TIED";
    }

    public void OnRestartClick()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

   
}
