using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class Lobby : UnDestroyable {
    public struct PlayerData
    {
        public PLAYERS_ENUM playerEnum; //Represent the player number allowing to represent player by a unique value matching with connexionID;
        public Color playerColor;
        public string playerName;
        public PlayerConnexionScript.ConnectionData connection;
        public bool ready;
    }

    public string lobbyName = "Testing";
    //Behaviour Data
    public int maxPlayers = 4;
    public int minPlayers = 2;
    //Enums
    public enum PLAYERS_ENUM { J1 = 0, J2, J3, J4 }

    //Events
    public delegate void DoorObserver(PlayerData playerData, int playerCount);
    public event DoorObserver OnPlayerEnter;
    public event DoorObserver OnPlayerLeave;
    public delegate void PlayerObserver(PlayerData playerData);
    public event PlayerObserver OnPlayerReady;
    public delegate void LobbyObserver();
    public event LobbyObserver OnGameStart;
    public event LobbyObserver OnGameWillStart;

    //Local data
    private ServerNetworkDiscoveryScript netDiscovery;
    private NetworkScript net;
    private Dictionary<string, PlayerData> players = new Dictionary<string, PlayerData>();
    private int playersCount = 0;
    private bool InGame = false;

    // Use this for initialization
    void Start() {
        netDiscovery = GetComponentInChildren<ServerNetworkDiscoveryScript>();
        net = GetComponentInChildren<NetworkScript>();
        net.OnConnect += CreatePlayer;
        net.OnDisconnect += DestroyPlayer;

        netDiscovery.broadcastData = lobbyName + ";" + playersCount + "/" + maxPlayers;
        netDiscovery.Initialize();
        netDiscovery.StartAsServer();
    }

    void CreatePlayer(PlayerConnexionScript.ConnectionData connectionData)
    {
        if (playersCount == maxPlayers || InGame)
            return;// TODO send error to client;

        if (!players.ContainsKey(connectionData.ipAddress))
        {
            Color[] playerColors = { Color.red, Color.blue, Color.yellow, Color.green };
            PlayerData p = new PlayerData
            {
                ready = false,
                playerName = "J" + connectionData.connexionId,
                playerColor = playerColors[connectionData.connexionId - 1],
                connection = connectionData,
                playerEnum = (PLAYERS_ENUM)playersCount
            };
            players.Add(connectionData.ipAddress, p);
            playersCount++;
            //Update broadcast data
            //StartCoroutine(UpdateBroadcastData(lobbyName + ";" + playersCount + "/" + maxPlayers));
            //Update color;
            var pScript = net.GetPlayer(p.connection.connexionId).GetComponent<PlayerConnexionScript>();
            pScript.SendColorUpdate(p.playerColor);
            pScript.OnMessageReceived += ParseMessage;
            if (OnPlayerEnter != null)
                OnPlayerEnter(p, playersCount);
        }
        else//Reconnect or error
        {
            Debug.Log("Reconnect requested.");//For ingame;
        }
    }

    void DestroyPlayer(PlayerConnexionScript.ConnectionData connectionData)
    {
        if (players.ContainsKey(connectionData.ipAddress))
        {
            PlayerData p = players[connectionData.ipAddress];
            players.Remove(connectionData.ipAddress);
            playersCount--;
            // StartCoroutine(UpdateBroadcastData(lobbyName + ";" + playersCount + "/" + maxPlayers));

            if (OnPlayerLeave != null)
                OnPlayerLeave(p, playersCount);
        }
        else
        {
            Debug.Log("Player already disconnected error");
        }
    }

    public void LoadSceneByIndexIfReady(int index)
    {
        bool run = true;
        if (playersCount < minPlayers || playersCount > maxPlayers)
            return;

        foreach (var playerMap in players)
        {
            if (!playerMap.Value.ready)
            {
                run = false;
                break;
            }
        }
        if (run)
        {
            InGame = true;
            SceneManager.LoadScene(index);
        }
        else
        {
            //Players are not ready. Fade.DisplayError;
        }
    }

    void ParseMessage(PlayerConnexionScript.ConnectionData connectionData, string buffer)
    {
        string[] command = buffer.Split(";".ToCharArray());
        if (command[0] == "Ready")
        {
            if (players.ContainsKey(connectionData.ipAddress))
            {
                var p = players[connectionData.ipAddress];
                Debug.Log("Player ready" + p.ready);
                p.ready = !p.ready;
                Debug.Log("Player ready" + p.ready);
                players[connectionData.ipAddress] = p;
                if (OnPlayerReady != null)
                    OnPlayerReady(p);
                Debug.Log("Player " + players[connectionData.ipAddress].playerName + " Ready : " + players[connectionData.ipAddress].ready);
            }
            else
            {
                Debug.Log("Player don't exists");
            }
        }
    }

    public void Back()
    {
        foreach (var c in players)
        {
            net.Disconnect(c.Value.connection.connexionId);
        }
        Destroy(net);
        if (netDiscovery.isActiveAndEnabled)
            netDiscovery.StopBroadcast();
        Destroy(netDiscovery);
        SceneManager.LoadScene(0);
        Destroy(gameObject);
    }

    IEnumerator UpdateBroadcastData(string data)
    {
        netDiscovery.broadcastData = data;
        if(netDiscovery.isActiveAndEnabled)
            netDiscovery.StopBroadcast();
        yield return null;
        if(!netDiscovery.isActiveAndEnabled)
            netDiscovery.StartAsServer();
        yield return null;
        if (playersCount == maxPlayers)
        {
            netDiscovery.StopBroadcast();
        }
    }

}
