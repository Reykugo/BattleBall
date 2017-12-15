using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameStateManager : UnDestroyable {

    enum State { MAIN_MENU, LOBBY, GAME, }

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void TransitToMainMenu()
    {

    }

    public void TransitToLoby()
    {
       
    }

    public void TransitToGame()
    {
        //TODO transit player go to the new scene
        SceneManager.LoadScene("EDEN");
    }

    public void TransitToEndGame()
    {
        SceneManager.LoadScene("Menu");
        //Player truc win / Player Machin loose etc..
    }
}
