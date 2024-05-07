using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PannelManager : MonoBehaviour
{

    [SerializeField] private GameObject shopPannel;
    [SerializeField] private GameObject hostPannel;
    [SerializeField] private GameObject joinPannel;
    [SerializeField] private GameObject playPannel;


    public void ActiveHostPanel()
    {
        shopPannel.SetActive(false);
        hostPannel.SetActive(true);
        joinPannel.SetActive(false);
    }

    public void ActivateJoinPannel()
    {
        shopPannel.SetActive(false);
        hostPannel.SetActive(false);
        joinPannel.SetActive(true);
    }

    public void ActivateShopPannel()
    {
        shopPannel.SetActive(true);
        hostPannel.SetActive(false);
        joinPannel.SetActive(false);
    }

    public void ActivateStartPannel()
    {
        playPannel.SetActive(true);
        shopPannel.SetActive(false);
        hostPannel.SetActive(false);
        joinPannel.SetActive(false);
    }
}
