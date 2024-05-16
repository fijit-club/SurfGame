using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using TMPro;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class RoomManager : MonoBehaviourPunCallbacks
{
    public static RoomManager Instance;
    [SerializeField] private PannelManager pannelInstance;
    [SerializeField] private GameObject nameListPref;
    [SerializeField] private GameObject playerListParent;
    [SerializeField] private GameObject startGameBtn;
    [SerializeField] private GameObject waitingToStartTxt;
    private Dictionary<int, GameObject> playerListObj;
    private string avatarURL;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.NickName = Bridge.GetInstance().thisPlayerInfo.data.multiplayer.username;
        avatarURL = Bridge.GetInstance().thisPlayerInfo.data.multiplayer.avatar;
    }

    public void TryCreateRoom()
    {
        if (!Bridge.GetInstance().testing)
            PhotonNetwork.LocalPlayer.NickName = Bridge.GetInstance().thisPlayerInfo.data.multiplayer.username;

        // Set the avatar URL as a custom property
        ExitGames.Client.Photon.Hashtable playerCustomProperties = new ExitGames.Client.Photon.Hashtable();
        playerCustomProperties.Add("AvatarURL", avatarURL);
        PhotonNetwork.LocalPlayer.SetCustomProperties(playerCustomProperties);

        CreateRoom();
    }

    private void CreateRoom()
    {
        PhotonNetwork.JoinRoom(Bridge.GetInstance().thisPlayerInfo.data.multiplayer.lobbyId);
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.BroadcastPropsChangeToAll = true;
        roomOptions.EmptyRoomTtl = 0;
        PhotonNetwork.CreateRoom(Bridge.GetInstance().thisPlayerInfo.data.multiplayer.lobbyId, roomOptions);
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        PhotonNetwork.JoinRoom(Bridge.GetInstance().thisPlayerInfo.data.multiplayer.lobbyId);
    }

    public override void OnJoinedRoom()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            startGameBtn.SetActive(true);
            waitingToStartTxt.SetActive(false);
        }
        else
        {
            startGameBtn.SetActive(false);
            waitingToStartTxt.SetActive(true);
        }

        playerListObj = new Dictionary<int, GameObject>();
        pannelInstance.ActivateStartPannel();
        Player[] players = PhotonNetwork.PlayerList;
        foreach (Player player in players)
        {
            GameObject playerObject = Instantiate(nameListPref, Vector3.zero, Quaternion.identity, playerListParent.transform);
            playerObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = player.NickName;

            // Get the avatar URL from custom properties
            string avatarURL = (string)player.CustomProperties["AvatarURL"];

            StartCoroutine(DownloadImage(avatarURL, playerObject.GetComponent<Image>()));
            playerListObj.Add(player.ActorNumber, playerObject);
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            startGameBtn.SetActive(true);
            waitingToStartTxt.SetActive(false);
        }
        else
        {
            startGameBtn.SetActive(false);
            waitingToStartTxt.SetActive(true);
        }

        GameObject playerObject = Instantiate(nameListPref, Vector3.zero, Quaternion.identity, playerListParent.transform);
        playerObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = newPlayer.NickName;

        // Get the avatar URL from custom properties
        string avatarURL = (string)newPlayer.CustomProperties["AvatarURL"];

        StartCoroutine(DownloadImage(avatarURL, playerObject.GetComponent<Image>()));
        playerListObj.Add(newPlayer.ActorNumber, playerObject);
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
        if (PhotonNetwork.InRoom)
        {
            PhotonNetwork.LeaveRoom();
        }
        pannelInstance.ActivateShopPannel();
    }

    public override void OnLeftRoom()
    {
        foreach (var playerObj in playerListObj.Values)
        {
            Destroy(playerObj);
        }
        playerListObj.Clear();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            startGameBtn.SetActive(true);
            waitingToStartTxt.SetActive(false);
        }
        else
        {
            startGameBtn.SetActive(false);
            waitingToStartTxt.SetActive(true);
        }

        Destroy(playerListObj[otherPlayer.ActorNumber]);
        playerListObj.Remove(otherPlayer.ActorNumber);

        if (PhotonNetwork.CurrentRoom.PlayerCount == 0)
        {
            PhotonNetwork.CurrentRoom.IsOpen = false;
            PhotonNetwork.CurrentRoom.IsVisible = false;
        }
    }

    public IEnumerator DownloadImage(string MediaUrl, Image profilePic)
    {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(MediaUrl);
        yield return request.SendWebRequest();
        if (request.isNetworkError || request.isHttpError)
        {
            Debug.Log(request.error);
        }
        else
        {
            var tex = ((DownloadHandlerTexture)request.downloadHandler).texture;
            Sprite profileSprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0, 0));
            profilePic.sprite = profileSprite;
        }
    }
}

