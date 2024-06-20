using UnityEngine;
using System.Collections;
using System;

public class CameraController : MonoBehaviour
{
    public Transform player;
    public bool follow;
    private void Update()
    {
        FollowPlayer();
    }

    private void OnEnable()
    {
        SpawnerPhoton.onCamFollow += OnPlayerGenerated;
    }

    private void OnDisable()
    {
        SpawnerPhoton.onCamFollow -= OnPlayerGenerated;
    }

    private void OnPlayerGenerated()
    {
        follow = true;
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void FollowPlayer()
    {
        if (follow)
        {
            transform.position = new Vector3(0, player.transform.position.y-1f, -10);
        }

    }
}