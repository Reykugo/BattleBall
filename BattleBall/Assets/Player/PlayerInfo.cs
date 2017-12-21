using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInfo : MonoBehaviour {

    int playerIndex;
    
    public Color playerColor;
    public string playerName;
    public Lobby.PLAYERS_ENUM playerEnum;

    public PlayerConnexionScript playerConnexion;
    public GameObject avatar;
    public bool ready;//TODO move outside;

    public void SetUp(GameObject avatar)
    {
        var avatarScript = avatar.GetComponent<AvatarScript>();
        avatarScript.OnAvatarFall += SendAvatarFall;
        avatarScript.OnAvatarDie += SendAvatarDead;
    }

    public void SendAvatarFall(GameObject avatar)
    {
        playerConnexion.SendAvatarFall();
    }

    public void SendAvatarDead(GameObject avatar)
    {
        playerConnexion.SendAvatarDead();
    }

}
