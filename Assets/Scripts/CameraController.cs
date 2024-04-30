using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour
{
    private void Update()
    {
        FollowPlayer();
    }

    private void FollowPlayer()
    {
        if (PlayerController.Instance.camFollow)
        {
            transform.position = new Vector3(0,PlayerController.Instance.transform.position.y, -10);
        }
    }
}