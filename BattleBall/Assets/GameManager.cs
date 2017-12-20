 using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public enum TypeOfGame { TIME, LIFE };

    public GameObject avatarPrefab;

    public TypeOfGame GameType;
    public int GameDuration;
    public int StartingLife;

    public List<GameObject> players {
        get;
        set;
    }

    private List<GameObject> avatars;//Object

    private AreaConfig areaConfig; //TerrainConfiguration (spawners...)
    public PopUp reconnectPopUp;
    void Start()
    {

        avatars = new List<GameObject>();
        SceneManager.sceneLoaded += OnTerrainLoaded;
    }


    private void OnTerrainLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.buildIndex == 1) //==Lobby
        {


        }
        else if (scene.buildIndex >= 2)//We are loading a game;
        {
            areaConfig = GameObject.Find("Area").GetComponent<AreaConfig>();
            if (areaConfig.spawners.Count < players.Count)
                return;//Error too much players for the spawners;
            GenerateAvatars();
        }
    }
    public void PlayerReconnected(NetworkScript.ConnectionData connectionData)
    {
        ResumeGame();
        reconnectPopUp.Close();
    }

    public void PlayerDisconnected(NetworkScript.ConnectionData connectionData)
    {
        PauseGame();
        reconnectPopUp.Open();
    }


    public void ResumeGame()
    {
        Time.timeScale = 1f;
    }

    public void PauseGame()
    {
        //UI display Waiting for player truc to reconnect
        // Pause the game
        Time.timeScale = 0f;

        //Display Pause or waiting popUp.
    }

    public void PlayerIsDead(GameObject avatar)
    {
        avatars.Remove(avatar);
        Destroy(avatar);
        if (avatars.Count == 1)
        {
            PlayerWin(avatars[0]);
        }
    }

    void PlayerWin(GameObject avatar)
    {
        Debug.Log(avatar.GetComponent<PlayerInfo>().playerName);

        //TODO Be able to restart a new fresh game from the lobby

        OnLeaving();
    }

    void OnLeaving()
    {
        players.Clear();
    }

    private void GenerateAvatars()
    {
        var spawners = areaConfig.spawners;

        foreach (var p in players)
        {
            Transform spawner = spawners[Random.Range(0, spawners.Count)];
            GameObject avatar = Instantiate(avatarPrefab, spawner.position, Quaternion.identity);
            avatars.Add(avatar);
            spawners.Remove(spawner);

            PlayerInfo playerScript = p.GetComponent<PlayerInfo>();
            //Setting up avatar
            AvatarScript avatarScript = avatar.GetComponent<AvatarScript>();
            avatarScript.OnAvatarDie += PlayerIsDead;
            avatarScript.areaConfig = areaConfig;
            avatarScript.AvatarColor = playerScript.playerColor;
            avatarScript.SetPlayerColor(playerScript.playerColor);
            avatarScript.SetPlayerName(playerScript.playerName);
            //Setting up avatar inputs.
            PlayerInputHandler inputHandler = avatar.GetComponent<PlayerInputHandler>();
            inputHandler.InitNet(playerScript.playerConnexion);
        }
    }
}
