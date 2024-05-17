using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Spawner : MonoBehaviour
{
    public static Spawner Instance;
    public GameObject[] obstacles;
    public GameObject[] slowObstacles;
    public GameObject[] wasteObstacles;
    public Transform[] obsSpawnPos;
    public Transform[] slowObjSpawnPos;
    public Transform[] wasteObjSpawnPos;

    private Vector3 lastPlayerPosition;

    private void Awake()
    {
        Instance = this;
    }


    private void Start()
    {
        SpawnObs(obstacles[Random.Range(0, obstacles.Length)], obsSpawnPos[Random.Range(0, obsSpawnPos.Length)]);
        SpawnObs(slowObstacles[Random.Range(0, slowObstacles.Length)], slowObjSpawnPos[Random.Range(0, slowObjSpawnPos.Length)]);
        SpawnObs(wasteObstacles[Random.Range(0, wasteObstacles.Length)], wasteObjSpawnPos[Random.Range(0, wasteObjSpawnPos.Length)]);

        //lastPlayerPosition = SpawnerPhoton.Instance.playerPrefab[Shop.Instance.selectedPlayer].transform.position;
        //StartCoroutine(Spawn());
    }

    

    private void SpawnObs(GameObject obs, Transform pos)
    {
        if (PhotonNetwork.InRoom)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                PhotonNetwork.Instantiate(obs.name, pos.position, Quaternion.identity);
            }
        }
        else
        {
            Instantiate(obs, pos.position, Quaternion.identity);
        }
       
    }



    //private IEnumerator Spawn()
    //{
    //    while (true)
    //    {
    //        Vector3 currentPlayerPosition = SpawnerPhoton.Instance.playerPrefab[Shop.Instance.selectedPlayer].transform.position;
    //        float distanceTraveled = currentPlayerPosition.y - lastPlayerPosition.y;

    //        if (Mathf.Abs(distanceTraveled) >= 8f)
    //        {
    //            SpawnObs(obstacles[Random.Range(0, obstacles.Length)], obsSpawnPos[Random.Range(0, obsSpawnPos.Length)]);
    //            SpawnObs(slowObstacles[Random.Range(0, slowObstacles.Length)], slowObjSpawnPos[Random.Range(0, slowObjSpawnPos.Length)]);
    //            SpawnObs(wasteObstacles[Random.Range(0, wasteObstacles.Length)], wasteObjSpawnPos[Random.Range(0, wasteObjSpawnPos.Length)]);
    //            lastPlayerPosition = currentPlayerPosition;
    //        }
    //        yield return null;
    //    }
    //}
}



