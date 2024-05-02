using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public static Spawner Instance;
    public GameObject[] obstacles;
    public GameObject[] slowObstacles;
    public Transform[] obsSpawnPos;
    public Transform[] slowObjSpawnPos;

    private Vector3 lastPlayerPosition;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        lastPlayerPosition = PlayerController.Instance.transform.position;
        StartCoroutine(Spawn());
    }

    private void SpawnObs(GameObject obs, Transform pos)
    {
        GameObject obj = Instantiate(obs, pos.position, Quaternion.identity, transform);
    }

    private IEnumerator Spawn()
    {
        while (true)
        {
            Vector3 currentPlayerPosition = PlayerController.Instance.transform.position;
            float distanceTraveled = currentPlayerPosition.y - lastPlayerPosition.y;

            if (Mathf.Abs(distanceTraveled) >= 10f)
            {
                SpawnObs(obstacles[Random.Range(0, obstacles.Length)], obsSpawnPos[Random.Range(0, obsSpawnPos.Length)]);
                SpawnObs(slowObstacles[Random.Range(0, slowObstacles.Length)], slowObjSpawnPos[Random.Range(0, slowObjSpawnPos.Length)]);
                lastPlayerPosition = currentPlayerPosition;
            }
            yield return null;
        }
    }
}



