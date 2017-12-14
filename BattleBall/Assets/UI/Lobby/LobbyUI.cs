using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

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
        //net.OnMessageReceived += ParseMessage;
	}
	
    void OnPlayerEnter(int connectionId)
    {
        GameObject go = net.GetPlayer(connectionId);
        Debug.Log("PlayerEntered");
        if (!players.ContainsKey(connectionId) && go)
        {
            players.Add(connectionId, go);
            var tmp = go.GetComponent<PlayerConnexionScript>();
            tmp.OnMessageReceived += (buffer) => { ParseMessage(connectionId, buffer); };
            var image = playersUI[connectionId - 1].GetComponentInChildren<Image>();
            image.color = tmp.playerData.color;
            image.transform.GetComponent<RectTransform>().localPosition = new Vector3(0f, 200 - 100 * (connectionId-1), 0f);
            playersUI[connectionId-1].GetComponentInChildren<Text>().text = tmp.playerData.playerName + " - " +  tmp.playerData.ipAddress;
            
        }
        else { Debug.Log("Error or reconnect " + connectionId); }
    }

    void OnPlayerExit(int connectionId)
    {
        if (players.ContainsKey(connectionId))
        {
            var image = playersUI[connectionId - 1].GetComponentInChildren<Image>();
            image.color = new Color(57f/255f,57f/255f,57f/255f);
            image.transform.GetComponent<RectTransform>().localPosition = new Vector3(0f, 200 - 100 * (connectionId - 1), 0f);
            playersUI[connectionId - 1].GetComponent<RectTransform>().localPosition = Vector3.zero;
            playersUI[connectionId - 1].GetComponentInChildren<Text>().text = "....";
            //
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
