using UnityEngine;
using System.Collections;
using System;

public class CameraController : MonoBehaviour
{
    public PlayerController player;
    public bool follow;
    private void Update()
    {
        FollowPlayer();
    }

    private void OnEnable()
    {
        PlayerController.onCamFollow += OnPlayerGenerated;
    }

    private void OnDisable()
    {
        PlayerController.onCamFollow -= OnPlayerGenerated;
    }

    private void OnPlayerGenerated()
    {
        follow = true;
        player = SpawnerPhoton.Instance.player.GetComponent<PlayerController>();
    }

    private void FollowPlayer()
    {
        if (follow)
        {
            transform.position = new Vector3(0, player.transform.position.y, -10);
        }
    }
}