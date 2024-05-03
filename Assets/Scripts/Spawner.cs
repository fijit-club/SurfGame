using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public static Spawner Instance;
    public GameObject[] obstacles;
    public GameObject[] slowObstacles;
    public GameObject[] wasteObstacles;
    public Transform[] obsSpawnPos;
    public Transform[] slowObjSpawnPos;
    public Transform[] wasteObjSpawnPos;
    public GameObject bgPref;
    public Transform previousBg;
    public float bgLength;

    private Vector3 lastPlayerPosition;

    private void Awake()
    {
        Instance = this;
    }

    private void OnEnable()
    {
        Shop.onGameBegin += GameBegin;
    }

    private void OnDisable()
    {
        Shop.onGameBegin -= GameBegin;
    }

    private void Update()
    {
        float distance = previousBg.position.y - Shop.Instance.playerPrefabs[Bridge.GetInstance().thisPlayerInfo.data.saveData.selectedPlayer].transform.position.y;
        if (distance >= 2)
        {
            SpawnBG();
        }
        print(distance + "Dis");
    }

    private void GameBegin()
    {
        lastPlayerPosition = PlayerController.Instance.transform.position;
        StartCoroutine(Spawn());
    }

    private void SpawnObs(GameObject obs, Transform pos)
    {
        GameObject obj = Instantiate(obs, pos.position, Quaternion.identity, transform);
    }

    public void SpawnBG()
    {
        Vector3 nextSpawnPosition = new Vector3(previousBg.position.x, previousBg.position.y - bgLength, 128);
        GameObject newBG = Instantiate(bgPref, nextSpawnPosition, Quaternion.identity);
        newBG.transform.rotation = Quaternion.Euler(new Vector3(-90, 0, 0));
        previousBg = newBG.transform;
    }



    private IEnumerator Spawn()
    {
        while (true)
        {
            Vector3 currentPlayerPosition = PlayerController.Instance.transform.position;
            float distanceTraveled = currentPlayerPosition.y - lastPlayerPosition.y;

            if (Mathf.Abs(distanceTraveled) >= 8f)
            {
                SpawnObs(obstacles[Random.Range(0, obstacles.Length)], obsSpawnPos[Random.Range(0, obsSpawnPos.Length)]);
                SpawnObs(slowObstacles[Random.Range(0, slowObstacles.Length)], slowObjSpawnPos[Random.Range(0, slowObjSpawnPos.Length)]);
                SpawnObs(wasteObstacles[Random.Range(0, wasteObstacles.Length)], wasteObjSpawnPos[Random.Range(0, wasteObjSpawnPos.Length)]);
                lastPlayerPosition = currentPlayerPosition;
            }
            yield return null;
        }
    }
}



