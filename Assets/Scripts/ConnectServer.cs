using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class ConnectServer : MonoBehaviourPunCallbacks
{
    [SerializeField] private PannelManager pannelInstance;
    [SerializeField] private GameObject playBtcs;
    public GameObject loadingTxt;
    private void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        playBtcs.SetActive(true);
        loadingTxt.SetActive(false);
        print("ServerConnected");
    }
}
