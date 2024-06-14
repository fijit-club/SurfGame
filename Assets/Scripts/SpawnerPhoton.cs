using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using Photon.Realtime;
using Photon.Pun;
using TMPro;
using System;

public class SpawnerPhoton : MonoBehaviourPunCallbacks
{
	public static SpawnerPhoton Instance;
	public static Action onCamFollow;

	[SerializeField] private Transform[] spawnPoints;

	[Tooltip("The prefab to use for representing the player")]
	public GameObject[] playerPrefab;
	public GameObject[] singlePlayerPrefab;
	public GameObject player;

    private void Awake()
    {
		Instance = this;
    }
    void Start()
	{
		Spawn();
	}

	public void Spawn()
	{
		Transform randonPoint = spawnPoints[UnityEngine.Random.Range(0, spawnPoints.Length - 1)];
		if (!PhotonNetwork.IsConnected || Bridge.GetInstance().thisPlayerInfo.data.multiplayer.lobbySize<=0)
		{
			//SceneManager.LoadScene("0");
			player = Instantiate(singlePlayerPrefab[Bridge.GetInstance().thisPlayerInfo.data.saveData.selectedPlayer], randonPoint.position, Quaternion.identity);
			onCamFollow?.Invoke();
			return;
		}

		if (playerPrefab == null)
		{
			Debug.LogError("<Color=Red><b>Missing</b></Color> playerPrefab Reference. Please set it up in GameObject 'Game Manager'", this);
		}
		else
		{


			if (PhotonNetwork.InRoom)
			{
				Debug.LogFormat("We are Instantiating LocalPlayer from {0}", SceneManagerHelper.ActiveSceneName);
				player = PhotonNetwork.Instantiate(this.playerPrefab[Bridge.GetInstance().thisPlayerInfo.data.saveData.selectedPlayer].name, randonPoint.position, Quaternion.identity, 0);
				onCamFollow?.Invoke();
			}
			else
			{
				
			}
		}
	}


	public override void OnLeftRoom()
	{
		SceneManager.LoadScene(0);
	}
}
