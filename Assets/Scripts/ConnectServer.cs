using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class ConnectServer : MonoBehaviourPunCallbacks
{
    [SerializeField] private PannelManager pannelInstance;
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
        pannelInstance.ActivateShopOnConnectedToServer();
        print("ServerConnected");
    }
}
