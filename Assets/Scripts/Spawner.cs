using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject[] obstacles;
    public GameObject[] slowObstacles;
    public Transform[] obsSpawnPos;
    public Transform[] slowObjSpawnPos;

    private void Start()
    {
        StartCoroutine(Spawn());
    }

    private void SpawnObs(GameObject obs,Transform pos)
    {
       GameObject obj = Instantiate(obs, pos.position, Quaternion.identity,transform);
        Destroy(obj, 20f);
    }

    private IEnumerator Spawn()
    {
        while (true)
        {
            SpawnObs(obstacles[Random.Range(0,obstacles.Length)], obsSpawnPos[Random.Range(0, obsSpawnPos.Length)]);
            SpawnObs(slowObstacles[Random.Range(0,slowObstacles.Length)], slowObjSpawnPos[Random.Range(0, obsSpawnPos.Length)]);
            yield return new WaitForSeconds(5f);
        }
    }
}
