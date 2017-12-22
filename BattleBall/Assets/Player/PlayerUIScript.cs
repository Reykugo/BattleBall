using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUIScript : MonoBehaviour {

    public Text LifeUI;
    public GameObject avatar;
    private Text playerNameUI;
    private AvatarScript avatarScript;

    private int playerLife;
	// Use this for initialization
	void Start () {
        //playerNameUI = GetComponent<Text>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnDestroy()
    {
        //avatar.GetComponent<AvatarScript>().OnAvatarFall -= OnPlayerLostLife;
    }

    public void Init()
    {
        avatarScript = avatar.GetComponent<AvatarScript>();
        playerNameUI = GetComponent<Text>();
        playerNameUI.text = avatarScript.PlayerName;
        playerNameUI.color = avatarScript.AvatarColor;
        avatarScript.OnAvatarFall += OnPlayerLostLife;
        playerLife = avatarScript.life;
        LifeUI.text = "X" + playerLife;
    }

    public void OnPlayerLostLife(GameObject avatar)
    {
        playerLife -= 1;
        LifeUI.text = "X" + playerLife;
    }
}
