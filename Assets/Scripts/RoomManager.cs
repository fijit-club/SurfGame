using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class RoomManager : MonoBehaviourPunCallbacks
{
    public static RoomManager Instance;
    [SerializeField] private PannelManager pannelInstance;

    [SerializeField] private TMP_InputField createText;
    [SerializeField] private TMP_InputField joinText;
    [SerializeField] private TMP_InputField playerCountText;
    [SerializeField] private GameObject nameListPref;
    [SerializeField] private GameObject playerListParent;

    public bool isVisibile;


    private void Start()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    public void TryCreateRoom()
    {
        if (!Bridge.GetInstance().testing)
            PhotonNetwork.LocalPlayer.NickName = Bridge.GetInstance().thisPlayerInfo.data.multiplayer.username;
        CreateRoom();
    }

    private void CreateRoom()
    {
        if (!Bridge.GetInstance().testing)
            PhotonNetwork.LocalPlayer.NickName = Bridge.GetInstance().thisPlayerInfo.data.multiplayer.username;
        PhotonNetwork.JoinRoom(Bridge.GetInstance().thisPlayerInfo.data.multiplayer.lobbyId);
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.BroadcastPropsChangeToAll = true;
        PhotonNetwork.CreateRoom(Bridge.GetInstance().thisPlayerInfo.data.multiplayer.lobbyId, roomOptions);
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        PhotonNetwork.JoinRoom(Bridge.GetInstance().thisPlayerInfo.data.multiplayer.lobbyId);
    }

    public void JoinRoom()
    {
        PhotonNetwork.JoinRoom(joinText.text);
    }

    public override void OnJoinedRoom()
    {
        pannelInstance.ActivateStartPannel();
        Player[] players = PhotonNetwork.PlayerList;
        for (int i = 0; i < players.Length; i++)
        {
            GameObject player = Instantiate(nameListPref, Vector3.zero, Quaternion.identity, playerListParent.transform);
            player.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = Bridge.GetInstance().thisPlayerInfo.data.multiplayer.username;
        }
    }

    public void StartGame()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            Debug.Log("Master client is starting the game.");
            PhotonNetwork.LoadLevel("Game");
        }
        else
        {
            Debug.LogWarning("Only the master client can start the game.");
        }
    }

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }

    public override void OnLeftRoom()
    {
        pannelInstance.ActivateShopPannel();
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        GameObject player = Instantiate(nameListPref, Vector3.zero, Quaternion.identity, playerListParent.transform);
        player.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = Bridge.GetInstance().thisPlayerInfo.data.multiplayer.username;
    }
}
