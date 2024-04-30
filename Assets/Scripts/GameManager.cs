using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public GameObject[] lifes;
    private int remainingLife=3;
    public TextMeshProUGUI scoreTxt;

    private void Awake()
    {
        Instance = this;
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
            Time.timeScale = 0;
        }
    }

    public void UpdateScore()
    {
        scoreTxt.text = PlayerController.Instance.GetScore().ToString();
    }
}
