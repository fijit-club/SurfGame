using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFollow : MonoBehaviour
{
    public Transform player;
    public float followSpeed = 2f;
    public float distance = 2f;

    void Update()
    {
        if (player != null)
        {
            Vector2 direction = player.position - transform.position;
            direction.Normalize();
            Vector2 targetPosition = (Vector2)player.position - direction;
            transform.position = Vector2.MoveTowards(transform.position, new Vector2(targetPosition.x,targetPosition.y+distance), followSpeed * Time.deltaTime);
        }
    }
}
