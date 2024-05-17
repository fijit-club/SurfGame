using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class BGSpawner : MonoBehaviour
{
    public GameObject bgPref;
    public GameObject singlePlayerBgPref;
    public Transform previousBg;
    public float bgLength;

    private void Start()
    {
        SpawnBG();
        StartCoroutine(Spawn());
    }

    public void SpawnBG()
    {
        if (PhotonNetwork.InRoom)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                Vector3 nextSpawnPosition = new Vector3(previousBg.position.x, previousBg.position.y - bgLength, 128);
                GameObject newBG = PhotonNetwork.Instantiate(bgPref.name, nextSpawnPosition, Quaternion.identity);
                newBG.transform.rotation = Quaternion.Euler(new Vector3(-90, 0, 0));
                previousBg = newBG.transform;
            }
        }
        else
        {
            Vector3 nextSpawnPosition = new Vector3(previousBg.position.x, previousBg.position.y - bgLength, 128);
            GameObject newBG = Instantiate(singlePlayerBgPref, nextSpawnPosition, Quaternion.identity);
            newBG.transform.rotation = Quaternion.Euler(new Vector3(-90, 0, 0));
            previousBg = newBG.transform;
        }
    }

    //private void Update()
    //{
    //    player = SpawnerPhoton.Instance.player.transform;
    //    float dis = previousBg.position.y - player.position.y;
    //    if (dis >= 2.5f)
    //    {
    //        SpawnBG();
    //    }
    //}

    private IEnumerator Spawn()
    {
        while (true)
        {
            SpawnBG();
            yield return new WaitForSeconds(0.5f);
        }
    }
}
