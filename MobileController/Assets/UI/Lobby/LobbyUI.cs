using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text;
using System;
using UnityEngine.Networking;

public class LobbyUI : FlowStep {

    // Use this for initialization
    const string START_GAME = "StartGame";
    const string PLAYER_UPDATE = "PlayerUpdate";
    Color playerColor = Color.black;


    bool ready = false;
    public Network.NetworkClientScript net;
    public Button readyButton;
    public Image background;
    private GameStateManager gameStateManager;
    private ColorBlock buttonStartColor;
    
	void Start (){
        buttonStartColor = readyButton.colors;
        buttonStartColor.highlightedColor = buttonStartColor.normalColor;
        readyButton.colors = buttonStartColor;
        gameStateManager = GetComponentInParent<GameStateManager>();
        net.OnMessageReceived += ParseServerMessage;
        net.OnDisconnect += OnDisconnect;
        readyButton.onClick.AddListener(SendReady);
    }

    void OnEnable()
    {

    }

    void OnDisable()
    {

    }

    void OnDisconnect()
    {
        Debug.Log("Disconnecting");
        ready = false; // TODO try to reconnect Few times;
        gameStateManager.TransitToGameSelection();
    }

    void SendReady()
    {
        if(ready)
        {
            readyButton.colors = buttonStartColor;
            readyButton.GetComponentInChildren<Text>().color = Color.white;
        }
        else
        {
            ColorBlock c = buttonStartColor;
            c.normalColor = c.pressedColor;
            c.highlightedColor = c.normalColor;
            readyButton.colors = c;
            readyButton.GetComponentInChildren<Text>().color = Color.grey;
#if UNITY_IOS || UNITY_ANDROID
            Handheld.Vibrate();
#endif
        }
        ready = !ready;
        NetworkError netErr = net.Send("Ready;");
        if (netErr == NetworkError.WrongConnection)
        {
            net.Disconnect();
        }
    }

    void ParseServerMessage(string data)
    {
        var command = data.Split(";".ToCharArray());
        if (command[0] == START_GAME)
        {
            gameStateManager.TransitToGame(playerColor);
        }
        else if(command[0] == PLAYER_UPDATE)
        {
           playerColor = Network.NetworkClientScript.ByteToColor(Encoding.ASCII.GetBytes(command[1]));
           background.color = playerColor;
        }
    }
}
