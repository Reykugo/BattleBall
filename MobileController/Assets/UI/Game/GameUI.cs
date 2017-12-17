using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class GameUI : FlowStep {

    const string START_GAME = "Win";
    const string PLAYER_UPDATE = "Loose";

    public delegate void DashObserver(bool isLoading);
    public event DashObserver OnDash;

    bool ready = false;
    public Network.NetworkClientScript net;
    private GameStateManager gameStateManager;

    bool dashLoading = true;

    void Start()
    {
        gameStateManager = GetComponentInParent<GameStateManager>();
        net.OnMessageReceived += ParseServerMessage;
        net.OnDisconnect += Quit;
        OnDash += DashHandler;
    }

	// Update is called once per frame
	void Update () {
        if (Input.touches.Length > 0)
        {
            dashLoading = true;
            OnDash(dashLoading);
        }
        else if (dashLoading)
        {
            dashLoading = false;
            OnDash(dashLoading);
        }

        byte[] b = BitConverter.GetBytes(dashLoading);
        
        var accelString = Network.NetworkClientScript.VectorToString(Input.acceleration);
       // Debug.Log(BitConverter.ToString(accelBytes));
       // Debug.Log(BitConverter.ToString(b));

        net.Send("State;" + Encoding.ASCII.GetString(b) + ";" + accelString);
	}

    void DashHandler(bool isLoading)
    {
    }
    void ParseServerMessage(string data)
    {
        var command = data.Split(";".ToCharArray());
        if (command[0] == "GameState")
        {
            //TODO Transit to results screen
            if (command[1] == "Win")
            {
            }
            else if(command[1] == "Loose")
            {
            }
            gameStateManager.TransitToLobby();
        }
    }

}
