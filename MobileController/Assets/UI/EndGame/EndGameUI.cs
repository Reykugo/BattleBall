using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndGameUI : FlowStep {

    public GameStateManager gameStateManager;
    public Network.NetworkClientScript net;

    bool victoryState = false;

    public Text StateText; 

	// Use this for initialization
	void Start () {
        gameStateManager = GetComponentInParent<GameStateManager>();
        net.OnMessageReceived += ParseServerMessage;
    }

    public void SetState(bool winning)
    {
        victoryState = winning;
        Debug.Log("winning");
        if (victoryState)
        {
            StateText.text = "YOU WIN ! GG !";
        }
        else
        {
            StateText.text = "YOU LOOSE ! NOOB !";
        }
    }


    void ParseServerMessage(string data)
    {
        var command = data.Split(";".ToCharArray());
        if (command[0] == "GoToLobby")
        {
            gameStateManager.TransitToLobby();
        }
    }
}
