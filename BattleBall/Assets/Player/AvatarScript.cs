using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Text;
using System;

public class AvatarScript : MonoBehaviour
{
    public int life = 0;
    public float StunDuration = 2;

    [SerializeField]
    private GameObject playerIdentity;

    [SerializeField]
    Text playerIdentityText;

    private Material playerMaterial;


    public AreaConfig areaConfig;
    public delegate void AvatarObserver(GameObject avatar);
    public event AvatarObserver OnAvatarDie;
    public event AvatarObserver OnAvatarFall;

    private ChargeScript dashScript;
    private MovingScript movingScript;

    private void Awake()
    {
        dashScript = this.GetComponent<ChargeScript>();
        movingScript = GetComponent<MovingScript>();

        playerMaterial = this.GetComponent<MeshRenderer>().material;
    }
    // Use this for initialization
    void Start()
    {
        GetComponent<ParticleSystem>().Stop();
    }

    public void SetInputHandler(PlayerNetHandler handler) //TODO make it cleaner.
    {
        handler.OnMoving += movingScript.SetMovement;
        handler.DashStarted += dashScript.StartDash;
        handler.DashStopped += dashScript.StopDash;
    }

    public void SetPlayerName(string name)
    {
        this.name = "player-" + name;
        playerIdentityText.text = name;
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
        if (life == 0 && OnAvatarDie != null)
        {
            OnAvatarDie(this.gameObject);
        }
        Invoke("PlayerRespawn", 3);
    }

    public void PlayerRespawn()
    {
        this.transform.position = areaConfig.RespawnPoint.position;
    }


    //Move in a stunOnCollideScript;
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
        if(collision.gameObject.tag == "Stunable" && dashScript.IsCharge)
        {
            DisabledPlayerCapacity();
            Invoke("EnabledPlayerCapacity", StunDuration);
        }
    }
}
