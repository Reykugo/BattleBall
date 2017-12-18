using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameStateManager : MonoBehaviour {
    //Transit from => to
    //gameSelection => Lobby
    //Lobby => Game
    //Game => Lobby
    //Lobby => gameSelection
    private enum GameState
    {
        GAME_SELECTION,
        LOBY,
        GAME
    }

    bool inGame = false;
    bool inLobby = false;
    private Network.NetworkClientScript networkClientScript;
    private GameSelectionUI gameSelection;
    private LobbyUI lobby;
    private GameUI game;

    private FlowStep current;

    
	// Use this for initialization
	void Start () {
        gameSelection = GetComponentInChildren<GameSelectionUI>();
        lobby = GetComponentInChildren<LobbyUI>();
        game = GetComponentInChildren<GameUI>();
        current = gameSelection;
        lobby.gameObject.SetActive(false);
        game.gameObject.SetActive(false);

    }

    public void TransitToLobby()
    {
        Debug.Log("Transit to loby ");
        current.gameObject.SetActive(false);
        lobby.gameObject.SetActive(true);
        current = lobby;
        Screen.sleepTimeout = SleepTimeout.NeverSleep;

    }

    public void TransitToGame(Color color)
    {
        current.gameObject.SetActive(false);
        game.Show();//TODO move show in flowStep;
        current = game;
        game.GetComponentInChildren<Image>().color = color;
    }

    public void TransitToGameSelection()
    {
        current.gameObject.SetActive(false);
        gameSelection.gameObject.SetActive(true);
        current = gameSelection;
        //TODO networkClientScript.Disconnect();
        Screen.sleepTimeout = SleepTimeout.SystemSetting;
    }
}
