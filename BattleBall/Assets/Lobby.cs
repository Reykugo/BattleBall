using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class Lobby : UnDestroyable {
 
    public GameObject playerPrefab;
    public NetworkScript net;

    public string lobbyName = "Testing";
    //Behaviour Data
    public int maxPlayers = 4;
    public int minPlayers = 2;
    //Enums
    public enum PLAYERS_ENUM { J1 = 0, J2, J3, J4 }

    //Events
    public delegate void DoorObserver (PlayerScript player, int playerCount);
    public event DoorObserver OnPlayerEnter;
    public event DoorObserver OnPlayerLeave;
    public delegate void PlayerObserver(PlayerScript player);
    public event PlayerObserver OnPlayerReady;

    //Local data
    private ServerNetworkDiscoveryScript netDiscovery;
    private Dictionary<string, GameObject> players = new Dictionary<string, GameObject>();
    public int playersCount = 0;//Todo use get/set restrictions;
    private bool InGame = false;

    private GameManager gameManager;

    // Use this for initialization
    void Start() {
        netDiscovery = net.gameObject.GetComponent<ServerNetworkDiscoveryScript>();
        net.OnConnect += CreatePlayer;
        net.OnDisconnect += DestroyPlayer;
        net.OnMessage += DispatchToPlayer;

        netDiscovery.broadcastData = lobbyName + ";" + playersCount + "/" + maxPlayers;
        netDiscovery.Initialize();
        netDiscovery.StartAsServer();

        gameManager = GetComponentInChildren<GameManager>();
    }

    void CreatePlayer(NetworkScript.ConnectionData connectionData)
    {
        if (playersCount == maxPlayers || InGame)
            return;// TODO send error to client;

        if (!players.ContainsKey(connectionData.ipAddress))
        {
            var go = Instantiate(playerPrefab, transform);

            go.name = "player " + connectionData.connexionId + " @" + connectionData.ipAddress;
            var s = go.GetComponent<PlayerConnexionScript>();
            s.clientData = connectionData;
            s.net = net;
            Color[] playerColors = { Color.red, Color.blue, Color.yellow, Color.green };

            var p = go.GetComponent<PlayerScript>();
            p.playerColor = playerColors[connectionData.connexionId - 1];
            p.playerConnexion = s;
            p.playerName = "J" + connectionData.connexionId;
            p.ready = false;
            p.playerEnum = (PLAYERS_ENUM)(connectionData.connexionId - 1);//TEMP TODO remove;
            //add a int / bool dictionnary or a list.

            players.Add(connectionData.ipAddress, go);
            playersCount++;
            //Update broadcast data
            StartCoroutine(UpdateBroadcastData(lobbyName + ";" + playersCount + "/" + maxPlayers));
            //Update color;
            s.SendColorUpdate(p.playerColor);
            s.OnMessageReceived += ParseMessage;
            if (OnPlayerEnter != null)
                OnPlayerEnter(p, playersCount);
        }
        else//Reconnect or error
        {
            Debug.Log("Reconnect requested.");//For ingame;
        }
    }

    void DestroyPlayer(NetworkScript.ConnectionData connectionData)
    {
        if (players.ContainsKey(connectionData.ipAddress))
        {
            GameObject p = players[connectionData.ipAddress];
            players.Remove(connectionData.ipAddress);
            playersCount--;
            if (p != null)
            {
                StartCoroutine(UpdateBroadcastData(lobbyName + ";" + playersCount + "/" + maxPlayers));
                if (OnPlayerLeave != null)
                    OnPlayerLeave(p.GetComponent<PlayerScript>(), playersCount);
                Destroy(p);
            }
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
            if (!playerMap.Value.GetComponent<PlayerScript>().ready)
            {
                run = false;
                break;
            }
        }
        if (run)
        {
            net.connectionAuthorized = false;
            List<GameObject> playersGO = new List<GameObject>(players.Values);
            foreach(var player in playersGO)
            {
                player.GetComponent<PlayerConnexionScript>().SendStartGame();
            }
            gameManager.players = playersGO;
            SceneManager.LoadScene(index);
            InGame = true;//TODO state;
        }
        else
        {
            //Players are not ready. Fade.DisplayError;
        }
    }

    //TODO refactor 

    void DispatchToPlayer(NetworkScript.ConnectionData connectionData, string buffer)
    {
        players[connectionData.ipAddress].GetComponent<PlayerConnexionScript>().ParseMessage(buffer);  
    }

    void ParseMessage(NetworkScript.ConnectionData connectionData, string buffer)
    {
        string[] command = buffer.Split(";".ToCharArray());
        if (command[0] == "Ready")
        {
            if (players.ContainsKey(connectionData.ipAddress))
            {
                var p = players[connectionData.ipAddress].GetComponent<PlayerScript>();

                Debug.Log("Player ready" + p.ready);
                p.ready = !p.ready;
                Debug.Log("Player ready" + p.ready);
                if (OnPlayerReady != null)
                    OnPlayerReady(p);
                Debug.Log("Player " + p.playerName + " Ready : " + p.ready);
            }
            else
            {
                Debug.Log("Player don't exists");
            }
        }
    }

    public void Back()
    {
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
        if(netDiscovery.running)
            netDiscovery.StopBroadcast();
        yield return new WaitForSeconds(0.5f);
        if(!netDiscovery.running)
            netDiscovery.StartAsServer();
        yield return new WaitForSeconds(0.5f);
        if (playersCount == maxPlayers)
        {
            netDiscovery.StopBroadcast();
        }
    }

}
