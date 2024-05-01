using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance;
    public Rigidbody2D rb;
    public float speed;
    public bool camFollow;
    private Camera mainCamera;
    private float camHalfWidth;
    public Transform slowObsSpawnPos;
    public float dragSpeed = 0.1f;
    private Vector2 direction;
    public bool isHolding;
    private float score=0;

    private void Awake()
    {
        Instance = this;
        mainCamera = Camera.main;
    }

    private void Start()
    {
        float orthoSize = mainCamera.orthographicSize;
        camHalfWidth = orthoSize / 2;
        direction = Vector2.down;
    }

    private void FixedUpdate()
    {
        rb.velocity = direction * speed * Time.deltaTime;
        RestrictMovement();
        FollowPlayer();
        score += Time.deltaTime * 2;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Follow"))
        {
            camFollow = true;
        }
        if (collision.CompareTag("Obstacle"))
        {
            direction = Vector2.zero;
            GameManager.Instance.RedueHealth();
        }
        if (collision.CompareTag("Slow"))
        {
            StartCoroutine(SlowMove());
        }
    }

    private IEnumerator SlowMove()
    {
        speed = 50;
        yield return new WaitForSeconds(4f);
        speed = 100;
    }

    private void RestrictMovement()
    {
        transform.position = new Vector2(Mathf.Clamp(transform.position.x, -camHalfWidth+0.5f, camHalfWidth-0.5f), transform.position.y);
    }

    private void FollowPlayer()
    {
        slowObsSpawnPos.position = new Vector2(slowObsSpawnPos.position.x, transform.position.y - 12f);
    }

    public void OnTouchDrag(Vector2 currentTouchPosition)
    {
        Vector2 previousTouchPosition = currentTouchPosition - (Vector2)Camera.main.ScreenToWorldPoint(currentTouchPosition);

        float horizontalMove = currentTouchPosition.x - previousTouchPosition.x;

        float verticalMove = (currentTouchPosition.y - previousTouchPosition.y) - transform.position.y + 3;

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

    private void Update()
    {
        if (Input.GetMouseButton(0))
        {
            OnTouchDrag(Input.mousePosition);
        }
            GameManager.Instance.UpdateScore();
    }

    public int GetScore()
    {
        return (int)score;
    }
}


