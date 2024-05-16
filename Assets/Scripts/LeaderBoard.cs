using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using TMPro;

public class LeaderBoard : MonoBehaviour
{
    public GameObject leaderBoardIconPref;
    private TextMeshProUGUI leaderBoardScore;
    private TextMeshProUGUI leaderBoardName;
    private Transform parent;
    private PhotonView pv;
    private GameObject icon;
    private Slider slider;
    private float maxDistance = 1000f;
    private float startSliderValue = 0f;

    private void Start()
    {
        pv = GetComponent<PhotonView>();
        parent= GameObject.Find("Game UI Reference").GetComponent<Transform>();
       
        LeaderBoardDisplay();
        slider.minValue = 0f;
        slider.maxValue = maxDistance;
        slider.value = startSliderValue;
    }

    private void Update()
    {
        if (!pv.IsMine)
        {
            return;
        }
        UpdateLearBoardScore();
        float newPosition = startSliderValue + GetComponent<PlayerController>().GetScore();
        slider.value = newPosition;

        pv.RPC("UpdateLeaderboardRPC", RpcTarget.All, newPosition);
    }

    [PunRPC]
    private void UpdateLeaderboardRPC(float newPosition)
    {
        if (slider == null)
        {
            return;
        }
        slider.value = Mathf.Clamp(newPosition, slider.minValue, slider.maxValue);
    }

    private void LeaderBoardDisplay()
    {
        icon = Instantiate(leaderBoardIconPref, parent);
        slider = icon.GetComponent<Slider>();
        leaderBoardName = icon.transform.GetChild(1).transform.GetChild(0).transform.GetChild(2).GetComponent<TextMeshProUGUI>();
        leaderBoardScore = icon.transform.GetChild(1).transform.GetChild(0).transform.GetChild(3).GetComponent<TextMeshProUGUI>();
        Player photonPlayer = pv.Owner;

        if (pv.IsMine)
        {
            icon.transform.GetChild(0).gameObject.SetActive(true);
        }
        else
        {
            icon.transform.GetChild(0).gameObject.SetActive(false);
        }
        string avatarURL = photonPlayer.CustomProperties["AvatarURL"].ToString();
        StartCoroutine(RoomManager.Instance.DownloadImage(avatarURL, icon.transform.GetChild(1).transform.GetChild(0).transform.GetChild(1).transform.GetComponent<Image>()));

        leaderBoardName.text = photonPlayer.NickName;
        leaderBoardScore.text = "0";
    }

    private void UpdateDeadEffect()
    {
        pv.RPC("DeadEffect", RpcTarget.All);
    }

    [PunRPC]
    private void UpdateLeaderboardScoreRPC(int newScore)
    {
        leaderBoardScore.text = newScore.ToString();
    }

    private void UpdateLearBoardScore()
    {
        if (GetComponent<PlayerController>(). GetScore() > 0)
        {
            GetComponent<PhotonView>().RPC("UpdateLeaderboardScoreRPC", RpcTarget.All, GetComponent<PlayerController>().GetScore());
        }
    }
}
