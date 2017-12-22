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
        this.avatar = avatar;
        var avatarScript = avatar.GetComponent<AvatarScript>();
        avatarScript.SetPlayerName(playerName);
        avatarScript.OnAvatarFall += SendAvatarFall;
        avatarScript.OnAvatarDie += SendAvatarDead;
        var effectManager = avatar.GetComponent<EffectManager>();
        effectManager.OnPowerAcquired += playerConnexion.SendNewPower;
    }

    public void SendAvatarFall(GameObject avatar)
    {
        playerConnexion.SendAvatarFall();
    }

    public void SendAvatarDead(GameObject avatar)
    {
        playerConnexion.SendAvatarDead();
        var avatarScript = avatar.GetComponent<AvatarScript>();
        avatarScript.OnAvatarFall -= SendAvatarFall;
        avatarScript.OnAvatarDie -= SendAvatarDead;
        var effectManager = avatar.GetComponent<EffectManager>();
        effectManager.OnPowerAcquired -= playerConnexion.SendNewPower;
    }

}
