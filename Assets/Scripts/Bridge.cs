using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System;
using System.Runtime.InteropServices;
using Newtonsoft.Json;
using TMPro;
//using LightDev;

public class NativeAPI
    {
#if UNITY_IOS && !UNITY_EDITOR
        [DllImport("__Internal")]
        public static extern void sendMessageToMobileApp(string message);
#endif
    }


[Serializable]
    public class SaveData
    {
    public int selectedPlayer;
    }


[System.Serializable]
    public class Attributes
    {
        public string id;
        public int level;
    }

    [System.Serializable]
    public class Asset
    {
        public string id;
        public Attributes[] attributes;

        public Asset(string peteId)
        {
            id = peteId;
        }
    }

    [System.Serializable]
    public class MultiplayerData
    {
        public string username;
        public string avatar;
        public string chatLobbyId;
        public int lobbySize;
        public bool isHost;

    }

[System.Serializable]
    public class Data
    {
        public List<Asset> assets;
        public SaveData saveData;
        public MultiplayerData multiplayer;
}

    [System.Serializable]
    public class PlayerInfo
    {
        public int coins;
        public bool sound = true;
        public bool vibration = true;
        public int highScore = 0;
        public Data data;
        public static PlayerInfo CreateFromJSON(string jsonString)
        {
            return JsonUtility.FromJson<PlayerInfo>(jsonString);
        }
    }


    public class Bridge : MonoBehaviour
    {
        public PlayerInfo thisPlayerInfo;
        private static Bridge instance;
        public int coinsCollected = 0;
        public bool testing;

#if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")]
        private static extern void setScore(int score);

         
        [DllImport("__Internal")]
        private static extern void buyAsset(string assetId);

        [DllImport("__Internal")]
        private static extern void updateCoins(int coinsChange);

        [DllImport("__Internal")]
        private static extern void updateExp(int expChange);
        
        [DllImport("__Internal")]
        private static extern void upgradeAsset(string assetID, string attributeID, int level);

        [DllImport("__Internal")]
        private static extern void load();

        [DllImport("__Internal")]
        private static extern void restart();

        [DllImport("__Internal")]
        private static extern void vibrate(bool isLong);

        [DllImport("__Internal")]
        private static extern void setSavedata(string savedata);
#endif

    public static Bridge GetInstance()
        {
            return instance;
        }

        private void Start()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
        WebGLInput.captureAllKeyboardInput = false;
        
#endif
        }


    public void SaveData(int value)
    {
        thisPlayerInfo.data.saveData.selectedPlayer = value;

        string jsonData = JsonConvert.SerializeObject(thisPlayerInfo.data.saveData);

        print("DATA BEFORE SENDING: " + jsonData);

#if UNITY_WEBGL && !UNITY_EDITOR
                    setSavedata(jsonData);
#endif

        print("DATA AFTER SENDING: " + jsonData);

    }

    private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(this);
                Debug.Log("Loaded");
#if UNITY_WEBGL && !UNITY_EDITOR
            load();
#endif

            }
            else
                Destroy(this);


        }



    public void AddExp(int exp)
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            updateExp(exp);
#endif
        }

        public void GameLoaded()
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            load();
#endif
        }

        public void ButtonPressed()
        {
            if (Application.platform == RuntimePlatform.Android)
            {
                using (AndroidJavaClass jc = new AndroidJavaClass("com.azesmwayreactnativeunity.ReactNativeUnityViewManager"))
                {
                    jc.CallStatic("sendMessageToMobileApp", "The button has been tapped!");
                }
            }
            else if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
#if UNITY_IOS && !UNITY_EDITOR
                NativeAPI.sendMessageToMobileApp("The button has been tapped!");
#endif
            }
        }

        public void SendScore(int score)
        {

            Debug.Log(coinsCollected + "sent coin");
#if UNITY_WEBGL && !UNITY_EDITOR
            updateCoins(coinsCollected);
#endif
#if UNITY_WEBGL && !UNITY_EDITOR

            setScore(score);
#elif UNITY_EDITOR
            Debug.Log("sendingscore" + score);
#endif
        }

        public void Mute()
        {
            SoundManager.Instance.Mute();
        }

        public void Unmute()
        {
            SoundManager.Instance.Unmute();
        }

        [ContextMenu("replay")]
        public void Replay()
        {
            GameManager.Instance.Reload();
            //SceneManager.LoadScene(0);
            //coinsCollected = 0;
        }

        public void SendInitialData(string json)
        {
            thisPlayerInfo = PlayerInfo.CreateFromJSON(json);
            Debug.Log(json);
        
        Shop.Instance.ShowSaveData(thisPlayerInfo.data.saveData.selectedPlayer);
        if (thisPlayerInfo.data.assets.Count == 0)
            {
                Debug.Log("buying default player");
                BuyPete("surf-ava");
            }

            if (thisPlayerInfo.sound)
            {
            Unmute();
            }
            else
            {
            Mute();

            }
            //Replay();
            //Events.CoinsCountChanged.Call();
        }

        public void AddCoin()
        {
            thisPlayerInfo.coins++;
        }

        public void UpdateCoins(int value)
        {
            thisPlayerInfo.coins += value;
            //coinsCollected += value;
            if (value > 0)
            {
                Debug.Log(value);
#if UNITY_WEBGL && !UNITY_EDITOR
            //updateCoins(coinsCollected);
#endif
            }
        }

        public void CollectCoins(int value)
        {
            thisPlayerInfo.coins += value;
            coinsCollected += value;
            //Debug.Log(value);

        }


    public void BuyPete(string peteID)
        {
#if UNITY_WEBGL && !UNITY_EDITOR
                    buyAsset(peteID);
#endif
            AddPete(peteID);
        }

        public void AddPete(string peteID)
        {
            Asset addedPete = new Asset(peteID);
            addedPete.id = peteID;



            Debug.Log("added nnew item " + addedPete.id);

            thisPlayerInfo.data.assets.Add(addedPete);
        }



    [ContextMenu("Do Something")]
        public void SendTextData()
        {
        //SendInitialData("{\"coins\": 123,\"playerData\": {\"shootPower\":25,\"shootSpeed\":20}}");
        //SendInitialData("{\"coins\":3400,\"data\":{\"assets\":[{\"id\":\"hill-climb-basic\",\"attributes\":[]}]}}");

        //SendInitialData("{\"coins\":3400,\"data\":{\"cannons\":[{\"id\":\"bvb-cannon-1\",\"attributes\":[{\"bvb-cannon-1-speed\":0,\"bvb-cannon-1-power\":0}]}]}}");
        //SendInitialData("{\"coins\":30000,\"data\":{}}");

        SendInitialData(
              "{\"coins\":1894,\"data\":{\"assets\":[{\"attributes\":[],\"id\":\"player-1\"},{\"attributes\":[],\"id\":\"surf-zane\"}],\"saveData\":null,\"multiplayer\":{\"chatLobbyId\":\"095c91be-a376-4c74-805e-d153523c40af\",\"avatar\":\"https://assets.fijit.club/fijit-v2/avatars/2.png\",\"isHost\":true,\"lobbySize\":3,\"username\":\"Vyshak\"}},\"highScore\":1000,\"sound\":true,\"vibration\":true}");

        //SendInitialData("{\"coins\":34,\"data\":{\"petes\":[{\"id\":\"health-1\",\"attributes\":[{\"id\":\"bullet-1-speed\",\"level\":91},{\"id\":\"compo-1-power\",\"level\":92}]},{\"id\":\"bvb-cannon-2\",\"attributes\":[{\"id\":\"bvb-cannon-2-speed\",\"level\":3},{\"id\":\"bvb-cannon-2-power\",\"level\":2}]}]}}");
        //SendInitialData("{\"coins\": 123,\"data\": null}");
        //Debug.Log(JsonUtility.ToJson( thisPlayerInfo.data));
        //Debug.Log( thisPlayerInfo.data);
        //SendInitialData("{\"coins\":10,\"sound\":false,\"vibration\":false,\"highScore\":1000,\"data\":null}");

    }

    [ContextMenu("SinglePlayer")]
    public void Testing()
    {
        SendInitialData("{\"coins\":1894,\"data\":{\"assets\":[{\"attributes\":[],\"id\":\"player-1\"},{\"attributes\":[],\"id\":\"knife-hit-knife-20\"}],\"saveData\":null,\"multiplayer\":null},\"highScore\":1000,\"sound\":true,\"vibration\":true}");
    }


    public int GetCoins()
        {
            return thisPlayerInfo.coins;
        }


        public void Silence(string silence)
        {
        if (silence == "true")
            AudioListener.pause = true;

        if (silence == "false")
            AudioListener.pause = false;

        System.Console.WriteLine("called silence " + silence);

        }

    public void VibrateBridge(bool isLong)
    {
#if UNITY_WEBGL && !UNITY_EDITOR
    //if(isVibrationOn)
      vibrate(isLong);
#endif
#if UNITY_EDITOR
        //if (isVibrationOn)
        Debug.Log("vibrating device " + isLong);
#endif
    }
}
