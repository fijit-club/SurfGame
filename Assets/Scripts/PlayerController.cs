using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance;
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
    private bool isChasing;
    private Coroutine chasingCoroutine;

    private void Awake()
    {
        Instance = this;
        mainCamera = Camera.main;
    }

    private void Start()
    {
        StartCoroutine(StartTimer()); 
    }

    private void FixedUpdate()
    {
        rb.velocity = direction * speed * Time.deltaTime;
        RestrictMovement();
        FollowPlayer();
        score += (rb.velocity.magnitude*Time.deltaTime)*10;
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
    }

    private IEnumerator SlowMove()
    {
        speed = baseSpeed/2;
        yield return new WaitForSeconds(4f);
        speed = baseSpeed;
    }

    private IEnumerator SpeedMove()
    {
        isFlying = true;
        speed = baseSpeed*2;
        direction = Vector2.down;
        GetComponent<BoxCollider2D>().enabled = false;
        yield return new WaitForSeconds(4f);
        speed = baseSpeed;
        GetComponent<BoxCollider2D>().enabled = true;
        isFlying = false;
    }

    private void RestrictMovement()
    {
        transform.position = new Vector2(Mathf.Clamp(transform.position.x, -camHalfWidth + 0.5f, camHalfWidth - 0.5f), transform.position.y);
    }

    private void FollowPlayer()
    {
        slowObsSpawnPos.position = new Vector2(slowObsSpawnPos.position.x, transform.position.y - 12f);
        if (isChasing)
        {
            octopus.GetComponent<EnemyFollow>().distance = 1.5f;
            octopus.transform.position = new Vector2(octopus.transform.position.x, octopus.transform.position.y);
        }
        else
        {
            octopus.GetComponent<EnemyFollow>().distance = 7;
            octopus.transform.position = new Vector2(octopus.transform.position.x, octopus.transform.position.y);
        }
        if (hitCount >= 2)
        {
            octopus.GetComponent<EnemyFollow>().distance = -1;
            octopus.GetComponent<EnemyFollow>().followSpeed = 1.5f;
            octopus.transform.position = new Vector2(octopus.transform.position.x, octopus.transform.position.y);
        }
    }

    private IEnumerator IsChaseing()
    {
        isChasing = true;
        yield return new WaitForSeconds(10f);
        isChasing = false;
        hitCount = 0;
    }


    public void OnTouchDrag(Vector2 currentTouchPosition)
    {
        if (isFlying)
        {
            return;
        }
        Vector2 previousTouchPosition = currentTouchPosition - (Vector2)Camera.main.ScreenToWorldPoint(currentTouchPosition);

        float horizontalMove = currentTouchPosition.x - previousTouchPosition.x;

        float verticalMove = (currentTouchPosition.y - previousTouchPosition.y);

        if (horizontalMove > 0)
        {
            direction = new Vector2(1, -1);
        }
        else if (horizontalMove < 0)
        {
            direction = new Vector2(-1, -1);
        }
        else if (verticalMove < 0)
        {
            direction = new Vector2(0, -1);
        }
        else if (verticalMove > 0)
        {
            direction = new Vector2(0, 0);
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



