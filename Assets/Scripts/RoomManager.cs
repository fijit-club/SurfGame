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
    public void createRoom()
    {
        int count = int.Parse(playerCountText.text);
        PhotonNetwork.CreateRoom(createText.text, new RoomOptions() { MaxPlayers = count, IsVisible = isVisibile, IsOpen = true }, TypedLobby.Default, null);
    }

    public void visibility(bool visible)
    {
        if (visible)
        {
            isVisibile = true;
        }
        else
        {
            isVisibile = false;
        }
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
            //player.GetComponent<PlayerNameItem>().playerNameListText.text = players[i].NickName;
        }
    }

    public void StartGame()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            Debug.Log("Master client is starting the game.");
            //PhotonNetwork.AutomaticallySyncScene = true;
            PhotonNetwork.LoadLevel("Game");
        }
        else
        {
            Debug.LogWarning("Only the master client can start the game.");
        }
    }


    public void JoinRoomInList(string roomname)
    {
        PhotonNetwork.JoinRoom(roomname);
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
        //player.GetComponent<PlayerNameItem>().playerNameListText.text = newPlayer.NickName;
    }
}
