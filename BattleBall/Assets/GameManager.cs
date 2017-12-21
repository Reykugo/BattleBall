 using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System;

public class GameManager : MonoBehaviour
{
    [Serializable]
    public class PowerReference
    {
        public Power.PowerType power;
        public GameObject prefab;
    }

    public PowerReference[] ListOfPower;
    //
    private Dictionary<Power.PowerType, GameObject> powerByType = new Dictionary<Power.PowerType, GameObject>();


    public enum TypeOfGame { TIME, LIFE };

    public GameObject avatarPrefab;

    public TypeOfGame GameType;
    public int GameDuration;
    public int StartingLife;

    public List<GameObject> players {
        get;
        set;
    }

    public List<GameObject> avatars;//Object

    private AreaConfig areaConfig; //TerrainConfiguration (spawners...)
    private List<PlayerInfo> disconnectedPlayers = new List<PlayerInfo>();
    public PopUp reconnectPopUp;
    void Start()
    {
        //For editor use.
        foreach (var power in ListOfPower)
        {
            powerByType.Add(power.power, power.prefab);
        }
        SceneManager.sceneLoaded += OnTerrainLoaded;
    }


    private void OnTerrainLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.buildIndex == 1) //==Lobby
        {
            //Unload 
            powerByType.Clear();
            avatars.Clear();
            areaConfig = null;
        }
        else if (scene.buildIndex >= 2)//We are loading a game;
        {
            areaConfig = GameObject.Find("Area").GetComponent<AreaConfig>();
            if (areaConfig.spawners.Count < players.Count)
                return;//Error too much players for the spawners
            avatars = new List<GameObject>();
            foreach(var power in ListOfPower)
            {
                powerByType.Add(power.power, power.prefab);
            }
            GenerateAvatars();
        }
    }
    public void PlayerReconnected(PlayerInfo playerInfo)
    {
        disconnectedPlayers.Remove(playerInfo);

        if(disconnectedPlayers.Count == 0)
        {
            ResumeGame();
            reconnectPopUp.Close();
        }
        else
        {
            var str = "";
            foreach (var p in disconnectedPlayers)
            {
                str += " ";
                str += p.playerName;
            }
            str += " ha";
            str += (disconnectedPlayers.Count > 1) ? "ve" : "s";
            str += " disconnected";
        }
    }

    public void PlayerDisconnected(PlayerInfo playerInfo)
    {
        disconnectedPlayers.Add(playerInfo);
        
        var str = "";
        foreach(var p in disconnectedPlayers)
        {
            str += " ";
            str += p.playerName;
        }
        str += "ha";
        str += (disconnectedPlayers.Count > 1) ? "ve" : "s";
        str += "disconnected";
        reconnectPopUp.UpdateTitle(str);

        if(disconnectedPlayers.Count == 1)
        {
            PauseGame();
            reconnectPopUp.Open();
        }
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
            Transform spawner = spawners[UnityEngine.Random.Range(0, spawners.Count)];
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

    public void ActivatePower(GameObject activator, Power.PowerType powerType)
    {
        GameObject powerPrefab = powerByType[powerType];
        var power = powerPrefab.GetComponent<Power>();
        GameObject powerGo = null;
        Power powerScript;
        bool other = false;
        switch (power.powerTarget)
        {
            case Power.PowerTarget.OTHERS:
                other = true;
                goto case Power.PowerTarget.ALL;
            case Power.PowerTarget.ALL:
                foreach(var avatar in avatars)
                {
                    if (other && avatar == activator)//ignore the activator if the powerTarget is Others
                        continue;
                    powerGo = Instantiate(powerByType[powerType], avatar.transform.position, Quaternion.identity, avatar.transform);
                    powerScript = powerGo.GetComponent<Power>();
                    avatar.GetComponent<EffectManager>().SetPower(powerScript);
                }
                break;
            case Power.PowerTarget.SELF:
                powerGo = Instantiate(powerByType[powerType], activator.transform.position, Quaternion.identity, activator.transform);
                powerScript = powerGo.GetComponent<Power>();
                activator.GetComponent<EffectManager>().SetPower(powerScript);
                break;
            default:
                Debug.LogAssertion("Unrecognized power");
                break;
        }
    }
}
