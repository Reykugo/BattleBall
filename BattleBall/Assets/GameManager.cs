using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public int NumberOfPlayers;

    public enum TypeOfGame { TIME, LIFE };

    public TypeOfGame GameType;
    public int GameDuration;

    public int PlayerLife;

    public GameObject PlayerPrefab;

    private List<GameObject> players;

    public List<Transform> Spawners;


    public Transform RespawnPoint;

    private Lobby lobby;

    //useing for test
    private Dictionary<string, Color> playersConfig = new Dictionary<string, Color> { { "J1",Color.red }, { "J2", Color.blue},
        { "J3", Color.yellow }, { "J4", Color.green}  };
    private string[] keys = { "J1", "J2", "J3", "J4" };
    int index = 0;
    // Use this for initialization
    void Start()
    {
        players = new List<GameObject>();
        GeneratePlayers();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void GeneratePlayers()
    {
        for (int i = 0; i < NumberOfPlayers; i++)
        {
            int index = Random.Range(0, Spawners.Count);
            Transform spawner = Spawners[index];
            Spawners.Remove(spawner);
            GameObject player = Instantiate<GameObject>(PlayerPrefab);
            player.transform.position = spawner.position;
            PlayerScript playerConf = player.GetComponent<PlayerScript>();
            playerConf.life = PlayerLife;
            players.Add(player);
            //useFortest
            Lobby.PlayerData playerData = TestPlayer(i);
            playerConf.InitPlayer(playerData);
            playerConf.Name = playerData.playerName;
            Debug.Log("test");
        }
    }


    //using for test
    public Lobby.PlayerData TestPlayer(int index)
    {
        Lobby.PlayerData playerData = new Lobby.PlayerData();
        string key = keys[index];
        playerData.playerColor = playersConfig[key];
        playerData.playerName = key;
        index += 1;
        return playerData;
    }

    public void PlayerIsDead(GameObject player)
    {
        players.Remove(player);
        Destroy(player);
        if(players.Count == 1)
        {
            PlayerWin(players[0]);
        }
    }

    void PlayerWin(GameObject player)
    {
        Debug.Log(player.GetComponent<PlayerScript>().Name);
    }
}
