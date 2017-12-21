using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class LobbyUI : MonoBehaviour {
    private Lobby lobby;
    public List<GameObject> playersUI;
	// Use this for initialization
	void Start () {
        lobby = GameObject.Find("Lobby").GetComponent<Lobby>();
        lobby.OnPlayerEnter += SetPlayerLayout;
        lobby.OnPlayerLeave += SetWaitingLayout;
        lobby.OnPlayerReady += SetReadyLayout;
        int i = 0;
        foreach (var playerInfo in lobby.GetPlayerInfos())
        {
            i++;
            SetPlayerLayout(playerInfo, i); 
        }
	}

    void OnDestroy()
    {
        lobby.OnPlayerEnter -= SetPlayerLayout;
        lobby.OnPlayerLeave -= SetWaitingLayout;
        lobby.OnPlayerReady -= SetReadyLayout;
    }

    void SetPlayerLayout(PlayerInfo playerData, int playerCount)
    {
        var image = playersUI[(int)playerData.playerEnum].GetComponentInChildren<Image>();
        image.color = playerData.playerColor;
        
        playersUI[(int)playerData.playerEnum].GetComponentInChildren<WaitingScript>().enabled = false;
        playersUI[(int)playerData.playerEnum].GetComponentInChildren<Text>().text = playerData.playerName + " - " + playerData.playerConnexion.clientData.ipAddress;
    }

    void SetReadyLayout(PlayerInfo playerData)
    {
        //Tmp change message
        if (playerData.ready)
        {
            playersUI[(int)playerData.playerEnum].GetComponentInChildren<Text>().text = playerData.playerName + " - " + "Ready";
        }
        else
        {
            playersUI[(int)playerData.playerEnum].GetComponentInChildren<Text>().text = playerData.playerName + " - " + playerData.playerConnexion.clientData.ipAddress;
        }
    }

    void SetWaitingLayout(PlayerInfo playerData, int playerCount)
    {
        var image = playersUI[(int)playerData.playerEnum].GetComponentInChildren<Image>();
        image.color = new Color(57f / 255f, 57f / 255f, 57f / 255f);
        playersUI[(int)playerData.playerEnum].GetComponentInChildren<WaitingScript>().enabled = true;
    }

    public void StartGame(int index)
    {
        lobby.LoadSceneByIndexIfReady(index);
    }
}
