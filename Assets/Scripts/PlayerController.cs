using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance;
    public int scoreMultiplier;
    public int coinMultiplier;
    public Rigidbody2D rb;
    public float baseSpeed;
    public float speed;
    public bool camFollow;
    private Camera mainCamera;
    private float camHalfWidth;
    public Transform slowObsSpawnPos;
    private Vector2 direction;
    private float score = 0;
    private int coins;
    private bool isFlying;
    private float speedIncreaseInterval = 10f;
    private float lastSpeedIncreaseTime;
    public TextMeshProUGUI startTimerTxt;
    public GameObject octopus;
    public int hitCount;
    public bool isChasing;
    private Coroutine chasingCoroutine;
    public GameObject tral;
    public GameObject straightImgae;
    public GameObject rightImgae;
    public GameObject leftImgae;
    public GameObject jumpImgae;
    private bool canTouchControll;

    private void Awake()
    {
        Instance = this;
        mainCamera = Camera.main;
    }

    private void OnEnable()
    {
        Shop.onGameBegin += GameBegin;
    }

    private void OnDisable()
    {
        Shop.onGameBegin -= GameBegin;
    }

    private void GameBegin()
    {
        StartCoroutine(StartTimer());
        straightImgae.SetActive(true);
    }

    private void FixedUpdate()
    {
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

    private IEnumerator StartTimer()
    {
        float orthoSize = mainCamera.orthographicSize;
        camHalfWidth = orthoSize / 2;

        int countdownValue = 3;
        while (countdownValue > 0)
        {
            startTimerTxt.text = countdownValue.ToString();
            yield return new WaitForSeconds(1);
            countdownValue--;
        }
        startTimerTxt.text = "GO!";
        yield return new WaitForSeconds(1);
        canTouchControll = true;
        startTimerTxt.text = null;
        direction = Vector2.down;
        speed = baseSpeed;
        lastSpeedIncreaseTime = Time.time;
    }

    private void IncreaseSpeed()
    {
        baseSpeed += 10f;
        speed = baseSpeed;
    }

    private void Update()
    {
        if (Input.GetMouseButton(0))
        {
            OnTouchDrag(Input.mousePosition);
        }
        GameManager.Instance.UpdateScore();
        if (hitCount >= 2)
        {
            octopus.SetActive(true);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Follow"))
        {
            camFollow = true;
        }
        if (collision.CompareTag("Obstacle"))
        {
            if (isChasing)
            {
                hitCount++;
            }
            direction = Vector2.zero;
            GameManager.Instance.RedueHealth();
            SoundManager.Instance.PlaySound(SoundManager.Sounds.LifeLost);
            StartCoroutine(DimColourOnHit());
        }
        if (collision.CompareTag("Slow"))
        {
            if (chasingCoroutine != null)
            {
                StopCoroutine(chasingCoroutine);
            }
            StartCoroutine(SlowMove());
            chasingCoroutine = StartCoroutine(IsChaseing());
            hitCount++;
        }
        if (collision.CompareTag("Jump"))
        {
            StartCoroutine(SpeedMove());
        }
        if (collision.CompareTag("Monster"))
        {
            GameManager.Instance.GameOver();
        }
        if (collision.CompareTag("Waste"))
        {
            coins += coinMultiplier;
            GameManager.Instance.CoinAnimation(coinMultiplier,collision.transform.position);
            SoundManager.Instance.PlaySound(SoundManager.Sounds.CoinPick);
            Destroy(collision.gameObject);
        }
    }

    private IEnumerator SlowMove()
    {
        speed = baseSpeed/2;
        yield return new WaitForSeconds(3f);
        speed = baseSpeed;
    }

    private IEnumerator SpeedMove()
    {
        isFlying = true;
        speed = baseSpeed*2;
        direction = Vector2.down;
        transform.GetChild(0).GetComponent<BoxCollider2D>().enabled = false;
        straightImgae.SetActive(false);
        leftImgae.SetActive(false);
        rightImgae.SetActive(false);
        jumpImgae.SetActive(true);
        tral.SetActive(false);
        yield return new WaitForSeconds(3f);
        speed = baseSpeed;
        transform.GetChild(0).GetComponent<BoxCollider2D>().enabled = true;
        isFlying = false;
        straightImgae.SetActive(true);
        leftImgae.SetActive(false);
        rightImgae.SetActive(false);
        jumpImgae.SetActive(false);
        tral.SetActive(true);
    }

    private void RestrictMovement()
    {
        transform.position = new Vector2(Mathf.Clamp(transform.position.x, -camHalfWidth + 0.5f, camHalfWidth - 0.5f), transform.position.y);

        if (Mathf.Abs(transform.position.x) >= camHalfWidth - 0.5f)
        {
            straightImgae.SetActive(true);
            rightImgae.SetActive(false);
            leftImgae.SetActive(false);
            transform.GetChild(0).rotation = Quaternion.Euler(0, 0,0);
        }
    }

    private void FollowPlayer()
    {
        slowObsSpawnPos.position = new Vector2(slowObsSpawnPos.position.x, transform.position.y - 12f);
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
            straightImgae.SetActive(false);
            leftImgae.SetActive(false);
            rightImgae.SetActive(true);
            transform.GetChild(0).rotation = Quaternion.Euler(0, 0, 50);
        }
        else if (horizontalMove < 0)
        {
            direction = new Vector2(-1, -1);
            straightImgae.SetActive(false);
            leftImgae.SetActive(true);
            rightImgae.SetActive(false);
            transform.GetChild(0).rotation = Quaternion.Euler(0, 0, -50);
        }
        else if (verticalMove < 0)
        {
            direction = new Vector2(0, -1);
            straightImgae.SetActive(true);
            leftImgae.SetActive(false);
            rightImgae.SetActive(false);
            transform.GetChild(0).rotation = Quaternion.Euler(0, 0, 0);
        }
        else if (verticalMove > 0)
        {
            direction = new Vector2(0, 0);
            straightImgae.SetActive(true);
            leftImgae.SetActive(false);
            rightImgae.SetActive(false);
            transform.GetChild(0).rotation = Quaternion.Euler(0, 0, 0);
        }
    }

    private IEnumerator DimColourOnHit()
    {
        foreach (Transform child in transform)
        {
            SpriteRenderer spriteRenderer = child.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                Color color = spriteRenderer.color;
                color.a = 0.5f;
                spriteRenderer.color = color;
            }
        }
        yield return new WaitForSeconds(2f);
        foreach (Transform child in transform)
        {
            SpriteRenderer spriteRenderer = child.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                Color color = spriteRenderer.color;
                color.a = 1f;
                spriteRenderer.color = color;
            }
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



