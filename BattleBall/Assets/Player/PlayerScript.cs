using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerScript : MonoBehaviour
{

    public string Name;
    public int life;

    [SerializeField]
    private GameObject playerIdentity;

    [SerializeField]
    Text playerIdentityText;

    private Material playerMaterial;

    private GameManager gameManager;

    private void Awake()
    {
        playerMaterial = this.GetComponent<MeshRenderer>().material;
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }
    // Use this for initialization
    void Start()
    {
        
    }


    public void InitPlayer(Lobby.PlayerData playerData)
    {
        SetPlayerName(playerData);
        SetPlayerColor(playerData.playerColor);
    }

    public void SetPlayerName(Lobby.PlayerData playerData)
    {
        this.name = "Player" + (int)playerData.playerEnum;
        playerIdentityText.text = playerData.playerName;
    }

    public void  SetPlayerColor(Color color)
    {
        playerIdentityText.color = color;
        playerMaterial.color = color;
    }

    public void PlayerFall()
    {
        print("fall");
        life -= 1;
        if(life == 0)
        {
            gameManager.PlayerIsDead(this.gameObject);    
        }
        Invoke("PlayerRespawn", 3);
    }

    public void PlayerRespawn()
    {
        this.transform.position = gameManager.RespawnPoint.position;
    }
}
