using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Text;
using System;

public class AvatarScript : MonoBehaviour
{
    public int life = 0;
    public float StunDuration = 2;
    public Color AvatarColor;

    [SerializeField]
    private GameObject playerIdentity;

    [SerializeField]
    Text playerIdentityText;

    private Material playerMaterial;

    
    public AreaConfig areaConfig;
    public delegate void AvatarObserver(GameObject avatar);
    public event AvatarObserver OnAvatarDie;
    public event AvatarObserver OnAvatarFall;

    private DashScript dashScript;
    private MovingScript movingScript;

    private PlayerInputHandler handler;
    private ColorUnifier colorUnifier;

    private void Awake()
    {
        colorUnifier = GetComponent<ColorUnifier>();
        dashScript = GetComponent<DashScript>();
        movingScript = GetComponent<MovingScript>();
        playerMaterial = GetComponent<MeshRenderer>().material;
        handler = GetComponent<PlayerInputHandler>();
    }
    // Use this for initialization
    void Start()
    {
        colorUnifier.OnColorChanged += (color) =>
        {
            //Make all that seek for a color unifier script.
            playerIdentityText.color = color;
            playerMaterial.color = color;
            GetComponent<Light>().color = color;
        };

        //Subscribe to standard Inputs.
        handler.OnMoving += OnMoving;
        handler.OnShaking += OnShaking;
        handler.OnTouch += OnTouch;
        handler.OnTouchStarted += OnTouchStarted;
        handler.OnTouchStopped += OnTouchReleased;


        SetPlayerColor(AvatarColor);
    }

    private void OnMoving(Vector3 movement)
    {
        //Apply moving effect if any..
        movement *= -1;
        movingScript.SetMovement(movement);
    }

    private void OnShaking()
    {
        //Call Effect activation if any effect and can be shaking activated.
    }

    private void OnTouch()
    {
        
    }

    private void OnTouchStarted()
    {
        dashScript.StartDash();
    }

    private void OnTouchReleased()
    {
        dashScript.ReleaseDash();
    }

    public void SetPlayerName(string name)
    {
        this.name = "player-" + name;
        playerIdentityText.text = name;
    }

    //TODO better to centralize color in a different script. Avoiding AvatarScript to be composed of everything.
    public void  SetPlayerColor(Color color)
    {
        colorUnifier.SetColor(color);
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
        this.GetComponent<DashScript>().enabled = state;
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

    public void StunPlayer()
    {
        this.GetComponent<DashScript>().StopDash();
        DisabledPlayerCapacity();
        
        Invoke("EnabledPlayerCapacity", StunDuration);
    }

    public void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Stunable" && dashScript.IsOnDash)//TODO try to check the current velocity of the player.
        {
            StunPlayer();
        }
    }
}
