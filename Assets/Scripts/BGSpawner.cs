using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class BGSpawner : MonoBehaviour
{
    public GameObject[] bgPref;
    public GameObject[] singlePlayerBgPref;
    public Transform previousBg;
    public float bgLengthShaderBg;
    public float bgLengthNoShaderBg;
    public Material[] newMaterial;
    public Sprite[] newSprite;
    private int selectedSea;

    private void Start()
    {
        if (GameManager.Instance.isGameOver)
        {
            return;
        }
        if (PhotonNetwork.InRoom)
        {
            selectedSea = (int)PhotonNetwork.MasterClient.CustomProperties["selectedSea"];
        }
        else
        {
            selectedSea = Shop.Instance.selectedSea;
        }
        previousBg.GetComponent<MeshRenderer>().material = newMaterial[selectedSea];
        //previousBg.GetComponent<SpriteRenderer>().sprite = newSprite[selectedSea];
        ////SpawnBG();
        StartCoroutine(Spawn());
    }

    public void SpawnBG()
    {
        if (PhotonNetwork.InRoom)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                Vector3 nextSpawnPosition = new Vector3(previousBg.position.x, previousBg.position.y - bgLengthShaderBg, 128);
                GameObject newBG = PhotonNetwork.Instantiate(bgPref[selectedSea].name, nextSpawnPosition, Quaternion.identity);
                newBG.GetComponent<MeshRenderer>().material = newMaterial[selectedSea];
                newBG.transform.rotation = Quaternion.Euler(new Vector3(-90, 0, 0));
                previousBg = newBG.transform;
            }
        }
        else
        {
            Vector3 nextSpawnPosition = new Vector3(previousBg.position.x, previousBg.position.y - bgLengthShaderBg, 128);
            GameObject newBG = Instantiate(singlePlayerBgPref[selectedSea], nextSpawnPosition, Quaternion.identity);
            newBG.GetComponent<MeshRenderer>().material = newMaterial[selectedSea];
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
