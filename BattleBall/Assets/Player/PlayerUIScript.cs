using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUIScript : MonoBehaviour {

    public Text LifeUI;
    public GameObject avatar;

    private AvatarScript avatarScript;

    private int playerLife;
	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Init()
    {
        avatarScript = avatar.GetComponent<AvatarScript>();
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
