using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerConfigureScript : MonoBehaviour
{

    public enum PlayerName {Player1 = 0, Player2, Player3, Player4};

    public PlayerName Player;

    [SerializeField]
    private GameObject playerIdentity;

    [SerializeField]
    Text playerIdentityText;

    private Material playerMaterial;

    // Use this for initialization
    void Start()
    {
        playerMaterial = this.GetComponent<MeshRenderer>().material;
        Configure();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void Configure()
    {
        switch(Player)
        {
            case PlayerName.Player1:
                playerIdentityText.text = "J1";
                playerIdentityText.color = Color.red;
                this.name = "Player1";
                playerMaterial.color = Color.red;
                break;
            case PlayerName.Player2:
                playerIdentityText.text = "J2";
                playerIdentityText.color = Color.blue;
                this.name = "Player2";
                playerMaterial.color = Color.blue;
                break;
            case PlayerName.Player3:
                playerIdentityText.text = "J3";
                playerIdentityText.color = Color.yellow;
                this.name = "Player3";
                playerMaterial.color = Color.yellow;
                break;
            case PlayerName.Player4:
                playerIdentityText.text = "J4";
                playerIdentityText.color = Color.green;
                this.name = "Player4";
                playerMaterial.color = Color.green;
                break;
            default:
                Debug.Log(Player);
                break;
        }
        
    }
}
