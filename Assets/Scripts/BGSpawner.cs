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
    public Transform previousTutorialBgNight;
    public Transform previousTutorialBgDay;
    public float bgLengthShaderBg;
    public float bgLengthShaderBgTurotial;
    public float bgLengthNoShaderBg;
    public float bgLengthNoShaderBgTutorial;
    public Material[] newMaterial;
    public Sprite[] newSprite;
    private int selectedSea;
    private bool isFirstSpawn = true;


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
        if (selectedSea == 0 && Shop.Instance.isTutorial==false)
        {
            previousBgDay.gameObject.SetActive(true);
            previousBgNight.gameObject.SetActive(false);
            previousTutorialBgDay.gameObject.SetActive(false);
            previousTutorialBgNight.gameObject.SetActive(false);
        }
        else if (selectedSea == 1 && Shop.Instance.isTutorial == false)
        {
            previousBgDay.gameObject.SetActive(false);
            previousBgNight.gameObject.SetActive(true);
            previousTutorialBgDay.gameObject.SetActive(false);
            previousTutorialBgNight.gameObject.SetActive(false);
        }
        else if(selectedSea == 0 && Shop.Instance.isTutorial == true)
        {
            previousBgDay.gameObject.SetActive(false);
            previousBgNight.gameObject.SetActive(false);
            previousTutorialBgDay.gameObject.SetActive(true);
            previousTutorialBgNight.gameObject.SetActive(false);
        }
        else if(selectedSea == 1 && Shop.Instance.isTutorial == true)
        {
            previousBgDay.gameObject.SetActive(false);
            previousBgNight.gameObject.SetActive(false);
            previousTutorialBgDay.gameObject.SetActive(false);
            previousTutorialBgNight.gameObject.SetActive(true);
        }
    }
    private void Start()
    {
        if (GameManager.Instance.isGameOver)
        {
            return;
        }
        StartCoroutine(Spawn());
    }

    private void SpawnDay()
    {
        float length;

        if (Shop.Instance.isTutorial == true  && isFirstSpawn)
        {
             length = bgLengthShaderBgTurotial;
        }
        else
        {
             length = bgLengthShaderBg;
        }
        if (PhotonNetwork.InRoom)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                Vector3 nextSpawnPosition = new Vector3(previousBgDay.position.x, previousBgDay.position.y - length, 128);
                GameObject newBG = PhotonNetwork.Instantiate(bgPref[0].name, nextSpawnPosition, Quaternion.identity);
                newBG.transform.rotation = Quaternion.Euler(new Vector3(-90, 0, 0));
                previousBgDay = newBG.transform;
            }
        }
        else
        {
            Vector3 nextSpawnPosition = new Vector3(previousBgDay.position.x, previousBgDay.position.y - length, 128);
            GameObject newBG = Instantiate(bgPref[0], nextSpawnPosition, Quaternion.identity);
            newBG.transform.rotation = Quaternion.Euler(new Vector3(-90, 0, 0));
            previousBgDay = newBG.transform;
        }
        isFirstSpawn = false;
    }

    private void SpawnNight()
    {
        float length;
        if (Shop.Instance.isTutorial == true && isFirstSpawn)
        {
             length = bgLengthNoShaderBgTutorial;
        }
        else
        {
             length = bgLengthNoShaderBg;
        }
        if (PhotonNetwork.InRoom)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                Vector3 nextSpawnPosition = new Vector3(previousBgDay.position.x, previousBgDay.position.y - length, 128);
                GameObject newBG = PhotonNetwork.Instantiate(bgPref[1].name, nextSpawnPosition, Quaternion.identity);
                previousBgDay = newBG.transform;
            }
        }
        else
        {
            Vector3 nextSpawnPosition = new Vector3(previousBgDay.position.x, previousBgDay.position.y - length, 128);
            GameObject newBG = Instantiate(bgPref[1], nextSpawnPosition, Quaternion.identity);
            previousBgDay = newBG.transform;
        }isFirstSpawn = false;

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

    private IEnumerator Spawn()
    {
        while (true)
        {
            SpawnBackGround();
            yield return new WaitForSeconds(0.5f);
        }
    }
}
