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

    public List<GameObject> players{
        get;
        set;
     }

    private List<GameObject> avatars;//Object
   
    private AreaConfig areaConfig; //TerrainConfiguration (spawners...)
    void Start()
    {
        avatars = new List<GameObject>();
        SceneManager.sceneLoaded += OnTerrainLoaded;
    }

    
    private void OnTerrainLoaded(Scene scene, LoadSceneMode mode)
    {
        if(scene.buildIndex == 1) //==Lobby
        {
            //Deinit
        }
        else //We are loading a game;
        {
            areaConfig = GameObject.Find("Area").GetComponent<AreaConfig>();
            if (areaConfig.spawners.Count < players.Count)
                return;//Error too much players for the spawners;
            GeneratePlayers();
        }
    }

    public void GeneratePlayers()
    {
        var spawners = areaConfig.spawners;
        
        foreach(var p in players)
        {
            PlayerScript playerScript = p.GetComponent<PlayerScript>();

            Transform spawner = spawners[Random.Range(0, spawners.Count)];
            GameObject avatar = Instantiate(avatarPrefab);
            AvatarScript avatarConf = avatar.GetComponent<AvatarScript>();

            avatar.transform.position = spawner.position;
            avatarConf.OnAvatarDie += PlayerIsDead;//TODO in playerScript.
            avatarConf.areaConfig = areaConfig;

            playerScript.SetUpAvatar(avatar);

            avatars.Add(avatar);
            spawners.Remove(spawner);
        }
    }


    public void PlayerIsDead(GameObject avatar)
    {
        avatars.Remove(avatar);
        Destroy(avatar);
        if(avatars.Count == 1)
        {
            PlayerWin(avatars[0]);
        }
    }

    void PlayerWin(GameObject avatar)
    {
        Debug.Log(avatar.GetComponent<PlayerScript>().playerName);
    }
}
