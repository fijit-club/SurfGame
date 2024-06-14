using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class ConnectServer : MonoBehaviourPunCallbacks
{
    [SerializeField] private PannelManager pannelInstance;
    [SerializeField] private GameObject playBtcs;
    [SerializeField] private GameObject toggleSeaBtn;
    public GameObject loadingTxt;
    private void Start()
    {
        if (Bridge.GetInstance().thisPlayerInfo.data.multiplayer.lobbySize <=1 || Bridge.GetInstance().thisPlayerInfo.data.multiplayer==null)
        {
            playBtcs.SetActive(true);
            loadingTxt.SetActive(false);
            toggleSeaBtn.SetActive(true);
            return;
        }
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
        toggleSeaBtn.SetActive(false);
        print("ServerConnected");
    }
}
