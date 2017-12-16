using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerScript : MonoBehaviour
{

    public string Name;
    public int life;

    public float StunDuration = 2;

    [SerializeField]
    private GameObject playerIdentity;

    [SerializeField]
    Text playerIdentityText;

    private Material playerMaterial;

    private GameManager gameManager;

    private ChargeScript charge;
    public ParticleSystem particleSystem;

    private void Awake()
    {
        charge = this.GetComponent<ChargeScript>();
        playerMaterial = this.GetComponent<MeshRenderer>().material;
        particleSystem = this.GetComponent<ParticleSystem>();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }
    // Use this for initialization
    void Start()
    {
        particleSystem.Stop();
    }


    public void InitPlayer(Lobby.PlayerData playerData)
    {
        SetPlayerName(playerData);
        SetPlayerColor(playerData.playerColor);
        var main = particleSystem.main;
        main.startColor = playerData.playerColor;
        this.GetComponent<Light>().color = playerData.playerColor;
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

    public void SetPlayerCapacityState(bool state = true)
    {
        this.GetComponent<MovingScript>().enabled = state;
        this.GetComponent<ChargeScript>().enabled = state;
        this.GetComponent<Light>().enabled = state;

    }

    public void DisabledPlayerCapacity()
    {
        SetPlayerCapacityState(false);
    }
    public void EnabledPlayerCapacity()
    {
        SetPlayerCapacityState(true);
    }

    public void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Stunable" && charge.IsCharge)
        {
            DisabledPlayerCapacity();
            Invoke("EnabledPlayerCapacity", StunDuration);
        }
    }
}
