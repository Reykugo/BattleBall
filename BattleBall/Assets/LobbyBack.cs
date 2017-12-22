using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyBack : MonoBehaviour {

    Lobby lobby;
	// Use this for initialization
	void Start () {
        lobby = GameObject.Find("Lobby").GetComponent<Lobby>();
	}

    public void Back()
    {
        lobby.Back();
    }
}
