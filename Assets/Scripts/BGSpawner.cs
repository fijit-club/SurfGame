using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class BGSpawner : MonoBehaviour
{
    public GameObject[] bgPref;
    public GameObject[] singlePlayerBgPref;
    public Transform previousBgDay;
    public Transform previousBgNight;
    public float bgLengthShaderBg;
    public float bgLengthNoShaderBg;
    public Material[] newMaterial;
    public Sprite[] newSprite;
    private int selectedSea;


    private void Awake()
    {
        if (PhotonNetwork.InRoom)
        {
            selectedSea = (int)PhotonNetwork.MasterClient.CustomProperties["selectedSea"];
        }
        else
        {
            selectedSea = Shop.Instance.selectedSea;
        }
        if (selectedSea == 0)
        {
            previousBgDay.gameObject.SetActive(true);
            previousBgNight.gameObject.SetActive(false);
        }
        else if (selectedSea == 1)
        {
            previousBgDay.gameObject.SetActive(false);
            previousBgNight.gameObject.SetActive(true);
        }
    }
    private void Start()
    {
        if (GameManager.Instance.isGameOver)
        {
            return;
        }
       
       
        //previousBg.GetComponent<MeshRenderer>().material = newMaterial[selectedSea];
        //previousBg.GetComponent<SpriteRenderer>().sprite = newSprite[selectedSea];
        ////SpawnBG();
        StartCoroutine(Spawn());
    }

    //public void SpawnBG()
    //{
    //    if (PhotonNetwork.InRoom)
    //    {
    //        if (PhotonNetwork.IsMasterClient)
    //        {
    //            Vector3 nextSpawnPosition = new Vector3(previousBgDay.position.x, previousBgDay.position.y - bgLengthShaderBg, 128);
    //            GameObject newBG = PhotonNetwork.Instantiate(bgPref[selectedSea].name, nextSpawnPosition, Quaternion.identity);
    //            newBG.GetComponent<MeshRenderer>().material = newMaterial[selectedSea];
    //            newBG.transform.rotation = Quaternion.Euler(new Vector3(-90, 0, 0));
    //            previousBgDay = newBG.transform;
    //        }
    //    }
    //    else
    //    {
    //        Vector3 nextSpawnPosition = new Vector3(previousBgDay.position.x, previousBgDay.position.y - bgLengthShaderBg, 128);
    //        GameObject newBG = Instantiate(singlePlayerBgPref[selectedSea], nextSpawnPosition, Quaternion.identity);
    //        newBG.GetComponent<MeshRenderer>().material = newMaterial[selectedSea];
    //        newBG.transform.rotation = Quaternion.Euler(new Vector3(-90, 0, 0));
    //        previousBgDay = newBG.transform;
    //    }
    //}

    private void SpawnDay()
    {
        if (PhotonNetwork.InRoom)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                Vector3 nextSpawnPosition = new Vector3(previousBgDay.position.x, previousBgDay.position.y - bgLengthShaderBg, 128);
                GameObject newBG = PhotonNetwork.Instantiate(bgPref[0].name, nextSpawnPosition, Quaternion.identity);
                newBG.transform.rotation = Quaternion.Euler(new Vector3(-90, 0, 0));
                previousBgDay = newBG.transform;
            }
        }
        else
        {
            Vector3 nextSpawnPosition = new Vector3(previousBgDay.position.x, previousBgDay.position.y - bgLengthShaderBg, 128);
            GameObject newBG = Instantiate(bgPref[0], nextSpawnPosition, Quaternion.identity);
            newBG.transform.rotation = Quaternion.Euler(new Vector3(-90, 0, 0));
            previousBgDay = newBG.transform;
        }
    }

    private void SpawnNight()
    {
        if (PhotonNetwork.InRoom)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                Vector3 nextSpawnPosition = new Vector3(previousBgDay.position.x, previousBgDay.position.y - bgLengthNoShaderBg, 128);
                GameObject newBG = PhotonNetwork.Instantiate(bgPref[1].name, nextSpawnPosition, Quaternion.identity);
                previousBgDay = newBG.transform;
            }
        }
        else
        {
            Vector3 nextSpawnPosition = new Vector3(previousBgDay.position.x, previousBgDay.position.y - bgLengthNoShaderBg, 128);
            GameObject newBG = Instantiate(bgPref[1], nextSpawnPosition, Quaternion.identity);
            previousBgDay = newBG.transform;
        }
    }

    private void SpawnBackGround()
    {
        if (selectedSea == 0)
        {
            SpawnDay();
        }
        else if (selectedSea == 1)
        {
            SpawnNight();
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
            //SpawnBG();
            SpawnBackGround();
            yield return new WaitForSeconds(0.5f);
        }
    }
}
