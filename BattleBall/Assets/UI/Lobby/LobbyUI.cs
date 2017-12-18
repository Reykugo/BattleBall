using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class LobbyUI : MonoBehaviour {
    public Lobby lobby;
    public List<GameObject> playersUI;
	// Use this for initialization
	void Start () {
        
        lobby.OnPlayerEnter += SetPlayerLayout;
        lobby.OnPlayerLeave += SetWaitingLayout;
        lobby.OnPlayerReady += SetReadyLayout;
	}

    void SetPlayerLayout(PlayerScript playerData, int playerCount)
    {
        var image = playersUI[(int)playerData.playerEnum].GetComponentInChildren<Image>();
        image.color = playerData.playerColor;
        
        //image.transform.GetComponent<RectTransform>().localPosition = new Vector3(0f, 200 - 100 * ((int)playerData.playerEnum), 0f);
        playersUI[(int)playerData.playerEnum].GetComponentInChildren<WaitingScript>().enabled = false;
        playersUI[(int)playerData.playerEnum].GetComponentInChildren<Text>().text = playerData.playerName + " - " + playerData.playerConnexion.clientData.ipAddress;
    }

    void SetReadyLayout(PlayerScript playerData)
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
        //TODO Add Ready Icon;
    }

    void SetWaitingLayout(PlayerScript playerData, int playerCount)
    {
        var image = playersUI[(int)playerData.playerEnum].GetComponentInChildren<Image>();
        image.color = new Color(57f / 255f, 57f / 255f, 57f / 255f);
        //image.transform.GetComponent<RectTransform>().localPosition = new Vector3(0f, 200 - 100 * ((int)playerData.playerEnum), 0f);
        playersUI[(int)playerData.playerEnum].GetComponent<RectTransform>().localPosition = Vector3.zero;
        playersUI[(int)playerData.playerEnum].GetComponentInChildren<WaitingScript>().enabled = true;
    }
}
