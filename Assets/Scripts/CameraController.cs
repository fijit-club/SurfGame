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
        if (Shop.Instance.playerPrefabs[Bridge.GetInstance().thisPlayerInfo.data.saveData.selectedPlayer].GetComponent<PlayerController>().camFollow)
        {
            transform.position = new Vector3(0, Shop.Instance.playerPrefabs[Bridge.GetInstance().thisPlayerInfo.data.saveData.selectedPlayer].transform.position.y, -10);
        }
    }
}