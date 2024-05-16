using System.Collections;
using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public static Action onCountdownFinish;
    public GameObject coinPref;
    public RectTransform parent;
    public RectTransform coinDestination;
    public GameObject gameOVerScreen;
    public TextMeshProUGUI startTimerTxt;
    public GameObject wasteCollectEffect;
    public bool isGameOver;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        SoundManager.Instance.PlaySoundLoop(SoundManager.Sounds.BGM);
        StartCoroutine(StartTimer());
    }

    private IEnumerator StartTimer()
    {
        int countdownValue = 3;
        while (countdownValue > 0)
        {
            startTimerTxt.text = countdownValue.ToString();
            yield return new WaitForSeconds(1);
            countdownValue--;
        }
        startTimerTxt.text = "GO!";
        yield return new WaitForSeconds(1);
        startTimerTxt.text = null;
        onCountdownFinish?.Invoke();
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
            gameOVerScreen.SetActive(true);
        }
    }


    public void CoinAnimation(int count, Vector2 pos)
    {
        Destroy(Instantiate(wasteCollectEffect, pos, Quaternion.identity), 1f);
        for (int i = 0; i < count; i++)
        {
            GameObject coin = Instantiate(coinPref, parent);
            coin.GetComponent<RectTransform>().position = Camera.main.WorldToScreenPoint(pos);
            float delayTime = 0.1f;
            LeanTween.delayedCall(delayTime, () =>
            {
                LeanTween.move(coin, coinDestination.position, 0.5f)
                         .setEase(LeanTweenType.animationCurve)
                         .setOnComplete(() => { Destroy(coin); Bridge.GetInstance().UpdateCoins(1); });
            });
        }
    }
}
