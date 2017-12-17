using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour {

    int playerIndex;
    
    public Color playerColor;
    public string playerName;
    public Lobby.PLAYERS_ENUM playerEnum;

    public PlayerConnexionScript playerConnexion;
    public bool ready;//TODO move outside;

    private GameObject avatar; //Dynamically filled by gameManager, but can be set in the editor

    public void SetUpAvatar(GameObject avatar)
    {
        var avatarScript = avatar.GetComponent<AvatarScript>();
        avatarScript.SetPlayerColor(playerColor);
        avatarScript.SetPlayerName(playerName);
        
        avatarScript.SetInputHandler(new PlayerNetHandler(playerConnexion));
    }


}
