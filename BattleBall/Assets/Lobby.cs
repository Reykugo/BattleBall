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
    public delegate void DoorObserver (PlayerInfo player, int playerCount);
    public event DoorObserver OnPlayerEnter;
    public event DoorObserver OnPlayerLeave;
    public delegate void PlayerObserver(PlayerInfo player);
    public event PlayerObserver OnPlayerReady;

    //Local data
    private ServerNetworkDiscoveryScript netDiscovery;
    private Dictionary<string, GameObject> players = new Dictionary<string, GameObject>();
    private List<GameObject> disconnectedPlayers = new List<GameObject>();

    public int playersCount = 0;//Todo use get/set restrictions;

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
        if (players.Count == maxPlayers && !gameManager.gameStarted)
            return;


        if (!players.ContainsKey(connectionData.ipAddress))
        {
            var go = Instantiate(playerPrefab, transform);

            go.name = "player " + connectionData.connexionId + " @" + connectionData.ipAddress;
            var s = go.GetComponent<PlayerConnexionScript>();
            s.clientData = connectionData;
            s.net = net;
            Color[] playerColors = { Color.red, Color.blue, Color.yellow, Color.green };

            var p = go.GetComponent<PlayerInfo>();
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
        else if (gameManager.gameStarted && players.ContainsKey(connectionData.ipAddress))
        {
            var go = players[connectionData.ipAddress];
            disconnectedPlayers.Remove(go);
            if(disconnectedPlayers.Count == 0)
            {
                netDiscovery.StopBroadcast();
            }
            
            //If there is no other player to wait.
            var s = go.GetComponent<PlayerConnexionScript>();
            var tmp = s.clientData;
            tmp.connexionId = connectionData.connexionId;
            s.clientData = tmp;
            var p = go.GetComponent<PlayerInfo>();
            //Player asked to reconnect.
            //Update his Color
            s.SendColorUpdate(p.playerColor);//Be sure that is appropriate color.
            s.SendStartGame(gameManager.StartingLife);//If needed, might be ignored by the client.
            //SendHim
            gameManager.PlayerReconnected(p);
        }
    }

    void DestroyPlayer(NetworkScript.ConnectionData connectionData)
    {
        if(gameManager.gameStarted && players.ContainsKey(connectionData.ipAddress))
        {
            //PlayerDisconnected after game started.
            disconnectedPlayers.Add(players[connectionData.ipAddress]);
            var p = players[connectionData.ipAddress].GetComponent<PlayerInfo>();

            if(disconnectedPlayers.Count == 1)
            {
                netDiscovery.Initialize();
                netDiscovery.StartAsServer();
            }
            gameManager.PlayerDisconnected(p);
        }
        else if( !gameManager.gameStarted && players.ContainsKey(connectionData.ipAddress))
        {
            GameObject p = players[connectionData.ipAddress];
            players.Remove(connectionData.ipAddress);
            playersCount--;
            if (p != null)
            {
                StartCoroutine(UpdateBroadcastData(lobbyName + ";" + playersCount + "/" + maxPlayers));
                if (OnPlayerLeave != null)
                    OnPlayerLeave(p.GetComponent<PlayerInfo>(), playersCount);
                Destroy(p);
            }
        }
        else
        {
            Debug.Log("Player already disconnected error");
        }
    }

    public List<PlayerInfo> GetPlayerInfos()
    {
        List<PlayerInfo> playerInfos = new List<PlayerInfo>();
        if (players.Count > 0)
        {
            var vals = players.Values;
            foreach (var v in vals)
            {
                playerInfos.Add(v.GetComponent<PlayerInfo>());
            }
        }

        return playerInfos;
    }

    public void LoadSceneByIndexIfReady(int index)
    {
        bool run = true;
        if (playersCount < minPlayers || playersCount > maxPlayers)
            return;

        foreach (var playerMap in players)
        {
            if (!playerMap.Value.GetComponent<PlayerInfo>().ready)
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
                player.GetComponent<PlayerConnexionScript>().SendStartGame(gameManager.StartingLife);
            }
            gameManager.players = playersGO;
            if(netDiscovery.running)
                netDiscovery.StopBroadcast();
            SceneManager.LoadScene(index);
        }
        else
        {
            //Players are not ready. Fade.DisplayError;
        }
    } 

    void DispatchToPlayer(NetworkScript.ConnectionData connectionData, string buffer)
    {
        players[connectionData.ipAddress].GetComponent<PlayerConnexionScript>().ParseMessage(buffer);  
    }

    void ParseMessage(NetworkScript.ConnectionData connectionData, string buffer)
    {
        string[] command = buffer.Split(";".ToCharArray());
        if (command[0] == "Ready" && !gameManager.gameStarted)
        {
            if (players.ContainsKey(connectionData.ipAddress))
            {
                var p = players[connectionData.ipAddress].GetComponent<PlayerInfo>();
                p.ready = !p.ready;
                if (OnPlayerReady != null)
                    OnPlayerReady(p);
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


    //Should live in the ServerDiscoveryScript.
    IEnumerator UpdateBroadcastData(string data)
    {
        netDiscovery.broadcastData = data;
        if(netDiscovery.running)
            netDiscovery.StopBroadcast();
        yield return new WaitForSeconds(0.5f);
        if(!netDiscovery.running)
            netDiscovery.StartAsServer();
        yield return new WaitForSeconds(0.5f);
        if (playersCount == maxPlayers || gameManager.gameStarted)
        {
            netDiscovery.StopBroadcast();
        }
    }

}
