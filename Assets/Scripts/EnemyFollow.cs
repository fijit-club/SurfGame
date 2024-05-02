using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFollow : MonoBehaviour
{
    public Transform player;
    public float followSpeed = 5f;

    void Update()
    {
        if (player != null)
        {
            Vector3 direction = player.position - transform.position;
            direction.Normalize();
            transform.position += direction * followSpeed * Time.deltaTime;
        }
    }
}
