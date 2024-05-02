using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public GameObject[] lifes;
    private int remainingLife=3;
    public TextMeshProUGUI scoreTxt;
    public TextMeshProUGUI coinTxt;
    public TextMeshProUGUI highScoreTxt;
    public bool isGameOver;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        SoundManager.Instance.PlaySoundLoop(SoundManager.Sounds.BGM);
    }

    public void RedueHealth()
    {
        remainingLife--;
        for(int i = 0; i < 1;i++)
        {
            lifes[remainingLife].SetActive(false);
        }
        if (remainingLife == 0)
        {
            GameOver();
        }
    }

    public void UpdateScore()
    {
        scoreTxt.text = PlayerController.Instance.GetScore().ToString();
        highScoreTxt.text = Bridge.GetInstance().thisPlayerInfo.highScore.ToString();
        coinTxt.text = PlayerController.Instance.GetCoins().ToString();
    }

    public void Reload()
    {
        SceneManager.LoadScene(0);
    }

    public void GameOver()
    {
        if (!isGameOver)
        {
            isGameOver = true;
            SoundManager.Instance.PlaySound(SoundManager.Sounds.EndGame);
            Time.timeScale = 0;
        }
    }
}
