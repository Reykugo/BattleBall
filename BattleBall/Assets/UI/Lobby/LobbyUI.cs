using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class LobbyUI : MonoBehaviour {

    public NetworkScript net;
    public GameObject playerNet;
    public List<GameObject> playersUI;

    private bool ready = false;

    private Dictionary<int, GameObject> players = new Dictionary<int, GameObject>(); // Empty go that will instantiate balls and containData.
	// Use this for initialization
	void Start () {
        net.OnConnect += OnPlayerEnter;
        net.OnDisconnect += OnPlayerExit;
        net.OnMessageReceived += ParseMessage;
	}
	
    void OnPlayerEnter(int connectionId)
    {
        if (!players.ContainsKey(connectionId))
        {
            Debug.Log("player Enter " + connectionId);
            players.Add(connectionId, Instantiate<GameObject>(playerNet));
        }
        else { Debug.Log("Error or reconnect " + connectionId); }
    }

    void OnPlayerExit(int connectionId)
    {
        if (players.ContainsKey(connectionId))
        {
            Destroy(players[connectionId]); //Destroy Gameobject is better;
            players.Remove(connectionId);
        }
        else { Debug.Log("Error or reconnect " + connectionId); }
    }

    void ParseMessage(int connectionId, string buffer)
    {
        string[] command = buffer.Split(";".ToCharArray());
        if (command[0] == "Ready")
        { 
            if (players.ContainsKey(connectionId))
            {
                ready = !ready;
                Debug.Log(ready);
                playersUI[connectionId - 1].GetComponent<RectTransform>().Translate(new Vector3((ready)? 150f : -150f, 0f,0f));
            }
        }
    }

    void OnPlayerReady()
    {

    }
}
