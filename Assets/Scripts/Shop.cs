using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;

public class Shop : MonoBehaviour
{
    public static Action onGameBegin;
    public static Shop Instance;
    public GameObject shopUI;
    public GameObject gameUI;
    public int selectedPlayer;
    public Image itemImage;
    public Sprite[] itemSprites;
    private int itemPrice;
    public string[] itemNames;
    public string[] playerNames;
    public string[] itemDsicriptions;
    public GameObject buyBtn;
    public GameObject useBtn;
    public TextMeshProUGUI totalCoins;
    private string itemName;
    public TextMeshProUGUI itemNameTxt;
    public TextMeshProUGUI itemDiscriptionTxt;
    public int[] prices;
    public GameObject[] playerPrefabs;
    public GameObject[] playerPowers;

    public Sprite[] menuBgs;
    public Image bgImgageSinglePlayer;
    public Image bgImgageMultiPlayer;


    public int selectedSea;
    public GameObject[] seaIcons;
    public GameObject[] seaIconsRoom;

    public void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        //ShowSaveData(Bridge.GetInstance().thisPlayerInfo.data.saveData.selectedPlayer);
        PhotonNetwork.LocalPlayer.SetCustomProperties(new ExitGames.Client.Photon.Hashtable { { "selectedSea", 0 } });
    }

    public void play()
    {
        SceneManager.LoadScene(2);
    }

    private void Update()
    {
        ChangeForamte(totalCoins, Bridge.GetInstance().thisPlayerInfo.coins);
    }

    public void ShowSaveData(int value)
    {
        itemName = playerNames[value];
        itemNameTxt.text = playerNames[value];
        itemDiscriptionTxt.text = itemDsicriptions[value];
        ShowPovwerUpStats(value);
        selectedPlayer = value;
        useBtn.SetActive(true);
        buyBtn.SetActive(false);
        itemImage.sprite = itemSprites[value];   
    }

    public void Play()
    {
        GameBegin();
        gameUI.SetActive(true);
        shopUI.SetActive(false);
        onGameBegin?.Invoke();
        GameManager.Instance.isGameOver = false;
    }

    public void GameBegin()
    {
        for (int i = 0; i < playerPrefabs.Length; i++)
        {
            if (i == Bridge.GetInstance().thisPlayerInfo.data.saveData.selectedPlayer)
            {
                playerPrefabs[i].SetActive(true);
            }
            else
            {
                playerPrefabs[i].SetActive(false);
            }
        }
    }

    public void ToggleSea(bool toggle)
    {
        if (toggle)
        {
            PhotonNetwork.LocalPlayer.SetCustomProperties(new ExitGames.Client.Photon.Hashtable { { "selectedSea", 0 } });
            selectedSea = 0;
            seaIcons[0].SetActive(true);
            seaIcons[1].SetActive(false);

            seaIconsRoom[0].SetActive(true);
            seaIconsRoom[1].SetActive(false);

            bgImgageSinglePlayer.sprite = menuBgs[0];

            if (Bridge.GetInstance().thisPlayerInfo.data.multiplayer.lobbySize > 1)
            {
                GetComponent<PhotonView>().RPC("UpdateMultiplayerBackground", RpcTarget.All, 0);
            }
        }
        else
        {
            PhotonNetwork.LocalPlayer.SetCustomProperties(new ExitGames.Client.Photon.Hashtable { { "selectedSea", 1 } });
            selectedSea = 1;
            seaIcons[0].SetActive(false);
            seaIcons[1].SetActive(true);

            seaIconsRoom[0].SetActive(false);
            seaIconsRoom[1].SetActive(true);

            bgImgageSinglePlayer.sprite = menuBgs[1];

            if (Bridge.GetInstance().thisPlayerInfo.data.multiplayer.lobbySize > 1)
            {
                GetComponent<PhotonView>().RPC("UpdateMultiplayerBackground", RpcTarget.All, 1);
            }
        }
    }

    [PunRPC]
    private void UpdateMultiplayerBackground(int bgIndex)
    {
        bgImgageMultiPlayer.sprite = menuBgs[bgIndex];
    }

    public void SelectedCar()
    {
        Bridge.GetInstance().SaveData(selectedPlayer);
    }


    public void Next()
    {
        selectedPlayer++;
        if (selectedPlayer == itemSprites.Length)
        {
            selectedPlayer = 0;
        }
       
        ActivateItems();
    }

    public void Previous()
    {
        selectedPlayer--;
        if (selectedPlayer == -1)
        {
            selectedPlayer = itemSprites.Length - 1;
        }
       
        ActivateItems();
    }


    public void BuyItem()
    {
        int totalCoin = Bridge.GetInstance().thisPlayerInfo.coins;
        if (itemPrice > totalCoin)
        {
            SoundManager.Instance.PlaySound(SoundManager.Sounds.PurchaseFail);
            return;
        }
        SoundManager.Instance.PlaySound(SoundManager.Sounds.PurchaseSuccess);
        Bridge.GetInstance().UpdateCoins(-itemPrice);
        ChangeForamte(totalCoins, Bridge.GetInstance().thisPlayerInfo.coins);
        useBtn.SetActive(true);
        buyBtn.SetActive(false);
        SelectedCar();
        Bridge.GetInstance().BuyPete("surf-"+itemName);
    }


    public void ActivateItems()
    {
        ShowPovwerUpStats(selectedPlayer);
        itemImage.sprite = itemSprites[selectedPlayer];
        itemNameTxt.text = playerNames[selectedPlayer];
        itemDiscriptionTxt.text = itemDsicriptions[selectedPlayer];
        itemName = itemNames[selectedPlayer];
        SoundManager.Instance.PlaySound(SoundManager.Sounds.BottonClick);
        bool itemFound = false;
        foreach (Asset asset in Bridge.GetInstance().thisPlayerInfo.data.assets)
        {
            if (asset.id == "surf-"+itemName)
            {
                itemFound = true;
                break;
            }
        }

        if (itemFound)
        {
            useBtn.SetActive(true);
            buyBtn.SetActive(false);
            SelectedCar();
        }
        else
        {
            if (selectedPlayer > 0)
            {
                itemPrice = prices[selectedPlayer];
                useBtn.SetActive(false);
                buyBtn.SetActive(true);

                buyBtn.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = itemPrice.ToString();
            }
            if (selectedPlayer == 0)
            {
                useBtn.SetActive(true);
                buyBtn.SetActive(false);
            }
        }
    }

    private void ShowPovwerUpStats(int value)
    {
        for (int i = 0; i < itemSprites.Length; i++)
        {
            if (i == value)
            {
                playerPowers[i].SetActive(true);
            }
            else
            {
                playerPowers[i].SetActive(false);
            }
        }
    }


    private string ChangeCoinsFormate(int coins)
    {
        if (coins >= 10000)
        {
            return (coins / 1000).ToString() + "K";
        }
        else
        {
            return coins.ToString();
        }
    }

    public void ChangeForamte(TextMeshProUGUI text, int totalCoins)
    {
        text.text = ChangeCoinsFormate(totalCoins);
    }
}
