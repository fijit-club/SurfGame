using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Photon.Pun;
using TMPro;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance;

    public static Action onCamFollow;

    public int scoreMultiplier;
    public int coinMultiplier;
    public Rigidbody2D rb;
    public float baseSpeed;
    public float speed;

    private Camera mainCamera;
    private float camHalfWidth;
    private Vector2 direction;
    private float score = 0;
    private int coins;
    private float speedIncreaseInterval = 10f;
    private float lastSpeedIncreaseTime;
    private int hitCount;

    private Coroutine chasingCoroutine;
    private Coroutine slowMoveCoroutine;
    private Coroutine speedMoveCoroutine;

    public GameObject octopus;
    public GameObject tral;
    public GameObject straightImgae;
    public GameObject rightImgae;
    public GameObject leftImgae;
    public GameObject jumpImgae;

    private bool canTouchControll;
    private bool isChasing;
    private bool isFlying;
    private bool hitWithIsland;

    private TextMeshProUGUI scoreTxt;
    private TextMeshProUGUI coinTxt;
    private TextMeshProUGUI highScoreTxt;
    private List<GameObject> lifes = new List<GameObject>();
    private int remainingLife = 3;
    private GameObject healthSymbols;

    private void Awake()
    {
        Instance = this;
        mainCamera = Camera.main;
    }

    private void OnEnable()
    {
        GameManager.onCountdownFinish += GameBegin;
    }

    private void OnDisable()
    {
        GameManager.onCountdownFinish -= GameBegin;
    }

    private void GameBegin()
    {
        canTouchControll = true;
        onCamFollow?.Invoke();
        GameManager.Instance.startTimerTxt.text = null;
        direction = Vector2.down;
        speed = baseSpeed;
        lastSpeedIncreaseTime = Time.time;
        octopus= GameObject.FindWithTag("Monster");
    }

    private void Start()
    {
        float orthoSize = mainCamera.orthographicSize;
        camHalfWidth = orthoSize / 2;
        UpdateStraightImage(true);
        UpdateLeftImage(false);
        UpdateRightImage(false);
        UpdateJumpImage(false);
        Instantiate(octopus);

        scoreTxt = GameObject.Find("GameScore").GetComponent<TextMeshProUGUI>();
        highScoreTxt = GameObject.Find("GameHighScore").GetComponent<TextMeshProUGUI>();
        coinTxt = GameObject.Find("GameCoins").GetComponent<TextMeshProUGUI>();
        healthSymbols = GameObject.Find("HealthSymbols");
        print(healthSymbols.transform.GetChild(0).name);
        for (int i = 0; i < 3; i++)
        {
            lifes.Add(healthSymbols.transform.GetChild(i).gameObject);
        }
    }

    private void FixedUpdate()
    {
        if (gameObject.GetComponent<PhotonView>().IsMine == false && PhotonNetwork.IsConnected == true)
        {
            return;
        }

        rb.velocity = direction * speed * Time.deltaTime;
        RestrictMovement();
        FollowPlayer();
        score += (rb.velocity.magnitude*Time.deltaTime)*10*scoreMultiplier;
        if (Time.time - lastSpeedIncreaseTime >= speedIncreaseInterval)
        {
            IncreaseSpeed();
            lastSpeedIncreaseTime = Time.time;
        }
    }


    private void IncreaseSpeed()
    {
        baseSpeed += 20f;
        speed = baseSpeed;
    }

    private void Update()
    {
        if (gameObject.GetComponent<PhotonView>().IsMine == false && PhotonNetwork.IsConnected == true)
        {
            return;
        }
        UpdateScore();
        if (Input.GetMouseButton(0))
        {
            OnTouchDrag(Input.mousePosition);
            if (hitWithIsland)
            {
                StartCoroutine(OffCollider(2));
            }
        }
        if (hitCount >= 2)
        {
            octopus.SetActive(true);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (gameObject.GetComponent<PhotonView>().IsMine == false && PhotonNetwork.IsConnected == true)
        {
            return;
        }

        if (collision.CompareTag("Obstacle"))
        {
            if (isChasing)
            {
                hitCount++;
            }
            direction = Vector2.zero;
            RedueHealth();
            SoundManager.Instance.PlaySound(SoundManager.Sounds.LifeLost);
            hitWithIsland = true;
        }
        if (collision.CompareTag("Slow"))
        {
            if (chasingCoroutine != null)
            {
                StopCoroutine(chasingCoroutine);
            }
            if (slowMoveCoroutine != null)
            {
                StopCoroutine(SlowMove());
            }
            slowMoveCoroutine = StartCoroutine(SlowMove());
            chasingCoroutine = StartCoroutine(IsChaseing());
            hitCount++;
        }
        if (collision.CompareTag("Jump"))
        {
            if (speedMoveCoroutine != null)
            {
                StopCoroutine(SpeedMove());
            }
            speedMoveCoroutine = StartCoroutine(SpeedMove());
        }
        if (collision.CompareTag("Monster"))
        {
            GameManager.Instance.GameOver();
        }
        if (collision.CompareTag("Waste"))
        {
            coins += 5*coinMultiplier;
            GameManager.Instance.CoinAnimation(coinMultiplier,collision.transform.position);
            SoundManager.Instance.PlaySound(SoundManager.Sounds.CoinPick);
            Destroy(collision.gameObject);
        }
    }

    private IEnumerator OffCollider(int time)
    {
        transform.GetChild(0).gameObject.SetActive(false);
        yield return new WaitForSeconds(time);
        hitWithIsland = false;
        transform.GetChild(0).gameObject.SetActive(true);
    }

    private IEnumerator SlowMove()
    {
        speed = baseSpeed/2;
        yield return new WaitForSeconds(3f);
        speed = baseSpeed;
    }

    private IEnumerator SpeedMove()
    {
        StartCoroutine(OffCollider(5));
        isFlying = true;
        speed = baseSpeed*2;
        direction = Vector2.down;
        UpdateStraightImage(false);
        UpdateLeftImage(false);
        UpdateRightImage(false);
        UpdateJumpImage(true);
        transform.GetChild(0).rotation = Quaternion.Euler(0, 0, 0);
        tral.SetActive(false);
        yield return new WaitForSeconds(3f);
        speed = baseSpeed;
        isFlying = false;
        UpdateStraightImage(true);
        UpdateLeftImage(false);
        UpdateRightImage(false);
        UpdateJumpImage(false);
        tral.SetActive(true);
    }

    private void RestrictMovement()
    {
        transform.position = new Vector2(Mathf.Clamp(transform.position.x, -camHalfWidth + 0.5f, camHalfWidth - 0.5f), transform.position.y);

        if (Mathf.Abs(transform.position.x) >= camHalfWidth - 0.5f && !isFlying)
        {
            UpdateStraightImage(true);
            UpdateLeftImage(false);
            UpdateRightImage(false);
            UpdateJumpImage(false);
            transform.GetChild(0).rotation = Quaternion.Euler(0, 0,0);
        }
    }

    private void FollowPlayer()
    {
        if (octopus == null)
        {
            return;
        }
        if (isChasing)
        {
            octopus.GetComponent<EnemyFollow>().MoveTowards(transform.GetChild(1));
        }
        else
        {
            octopus.GetComponent<EnemyFollow>().MoveTowards(transform.GetChild(2));
        }
        if (hitCount >= 2)
        {
            octopus.GetComponent<EnemyFollow>().MoveTowards(transform);
        }
    }

    private IEnumerator IsChaseing()
    {
        isChasing = true;
        yield return new WaitForSeconds(8f);
        isChasing = false;
        hitCount = 0;
    }


    public void OnTouchDrag(Vector2 currentTouchPosition)
    {
        if (isFlying || !canTouchControll)
        {
            return;
        }
        Vector2 previousTouchPosition = currentTouchPosition - (Vector2)Camera.main.ScreenToWorldPoint(currentTouchPosition);

        float horizontalMove = currentTouchPosition.x - previousTouchPosition.x;

        float verticalMove = (currentTouchPosition.y - previousTouchPosition.y);

        if (horizontalMove > 0)
        {
            direction = new Vector2(1, -1);
            //straightImgae.SetActive(false);
            //leftImgae.SetActive(false);
            //rightImgae.SetActive(true);
            UpdateStraightImage(false);
            UpdateLeftImage(false);
            UpdateRightImage(true);
            UpdateJumpImage(false);

            transform.GetChild(0).rotation = Quaternion.Euler(0, 0, 50);
        }
        else if (horizontalMove < 0)
        {
            direction = new Vector2(-1, -1);
            //straightImgae.SetActive(false);
            //leftImgae.SetActive(true);
            //rightImgae.SetActive(false);
            UpdateStraightImage(false);
            UpdateLeftImage(true);
            UpdateRightImage(false);
            UpdateJumpImage(false);
            transform.GetChild(0).rotation = Quaternion.Euler(0, 0, -50);
        }
        //else if (verticalMove < 0)
        //{
        //    direction = new Vector2(0, -1);
        //    straightImgae.SetActive(true);
        //    leftImgae.SetActive(false);
        //    rightImgae.SetActive(false);
        //    transform.GetChild(0).rotation = Quaternion.Euler(0, 0, 0);
        //}
        //else if (verticalMove > 0)
        //{
        //    direction = new Vector2(0, 0);
        //    straightImgae.SetActive(true);
        //    leftImgae.SetActive(false);
        //    rightImgae.SetActive(false);
        //    transform.GetChild(0).rotation = Quaternion.Euler(0, 0, 0);
        //}
    }

    [PunRPC]
    void ActivateStraightImage(bool activate)
    {
        straightImgae.SetActive(activate);
    }

    [PunRPC]
    void ActivateJumpImage(bool activate)
    {
        jumpImgae.SetActive(activate);
    }

    [PunRPC]
    void ActivateRightImage(bool activate)
    {
        rightImgae.SetActive(activate);
    }

    [PunRPC]
    void ActivateLeftImage(bool activate)
    {
        leftImgae.SetActive(activate);
    }

    void UpdateStraightImage(bool activate)
    {
        GetComponent<PhotonView>().RPC("ActivateStraightImage", RpcTarget.All, activate);
    }


    void UpdateJumpImage(bool activate)
    {
        GetComponent<PhotonView>().RPC("ActivateJumpImage", RpcTarget.All, activate);
    }

    void UpdateRightImage(bool activate)
    {
        GetComponent<PhotonView>().RPC("ActivateRightImage", RpcTarget.All, activate);
    }

    void UpdateLeftImage(bool activate)
    {
        GetComponent<PhotonView>().RPC("ActivateLeftImage", RpcTarget.All, activate);
    }

    public void UpdateScore()
    {
        scoreTxt.text = GetScore().ToString();
        highScoreTxt.text = Bridge.GetInstance().thisPlayerInfo.highScore.ToString();
        coinTxt.text = GetCoins().ToString();
    }

    public void RedueHealth()
    {
        remainingLife--;
        lifes[remainingLife].SetActive(false);
        if (remainingLife == 0)
        {
            GameManager.Instance.GameOver();
        }
    }

    public int GetScore()
    {
        return (int)score;
    }

    public int GetCoins()
    {
        return coins;
    }
}



