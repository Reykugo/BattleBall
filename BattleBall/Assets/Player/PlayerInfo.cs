using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInfo : MonoBehaviour {

    int playerIndex;
    
    public Color playerColor;
    public string playerName;
    public Lobby.PLAYERS_ENUM playerEnum;

    public PlayerConnexionScript playerConnexion;
    public bool ready;//TODO move outside;
}
