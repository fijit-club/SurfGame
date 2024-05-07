using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFollow : MonoBehaviour
{
    public static EnemyFollow Instance;
    public Transform player;
    public float followSpeed = 6f;

    //private void OnEnable()
    //{
    //    Shop.onGameBegin += GameBegin;
    //}

    //private void OnDisable()
    //{
    //    Shop.onGameBegin -= GameBegin;
    //}

    //private void GameBegin()
    //{
    //    player = SpawnerPhoton.Instance.playerPrefab[Shop.Instance.selectedPlayer].transform;
    //}

    private void Start()
    {
        player = SpawnerPhoton.Instance.playerPrefab[Shop.Instance.selectedPlayer].transform;
    }

    private void Awake()
    {
        Instance = this;
    }

    public void MoveTowards(Transform pos)
    {
        Vector2 direction = pos.position - transform.position;
        direction.Normalize();
        Vector2 targetPosition = (Vector2)pos.position - direction;
        transform.position = Vector2.MoveTowards(transform.position, targetPosition, followSpeed * Time.deltaTime);
    }
}
