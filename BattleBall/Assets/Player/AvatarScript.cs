using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Text;
using System;

public class AvatarScript : MonoBehaviour
{
    public int life = 0;
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

    private EffectManager effectManager;//StateManager.


    //Stun TODO move.
    private Rigidbody rb;
    public float stunDuration = 2;
    public int stunTreshold = 10;
    //End stun

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        colorUnifier = GetComponent<ColorUnifier>();
        dashScript = GetComponent<DashScript>();
        movingScript = GetComponent<MovingScript>();
        playerMaterial = GetComponent<MeshRenderer>().material;
        handler = GetComponent<PlayerInputHandler>();
        effectManager = GetComponent<EffectManager>();
    }
    // Use this for initialization
    void Start()
    {
        colorUnifier.OnColorChanged += (color) =>
        {
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
        //movement = currentEffect.ApplyMovementModifier(movement);
        movingScript.SetMovement(movement);
    }

    private void OnShaking()
    {
        effectManager.ActivateCurrentPower();
        //
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
        name = "player-" + name;
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
        if (life > 0)
        {
            Invoke("PlayerRespawn", 3);
        }
        else
        {
            if(OnAvatarDie != null)
                OnAvatarDie(gameObject);
        } 
    }

    public void PlayerRespawn()
    {
        transform.position = areaConfig.RespawnPoint.position;
    }


    //TODO move in a AvatarCapacity.
    public void SetPlayerCapacityState(bool state = true)
    {
        GetComponent<MovingScript>().enabled = state;
        GetComponent<DashScript>().enabled = state;
        GetComponent<Light>().enabled = state;
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
        GetComponent<DashScript>().StopDash();//Better to react on the state change.
        DisabledPlayerCapacity();
        //Todo use modular stun duration.
        Invoke("EnabledPlayerCapacity", stunDuration);
    }

    public void OnCollisionEnter(Collision collision)
    {
        //If the magnitude of the collider is greater than the treshold we stun the IStunnable;
        var OtherDashScript = collision.gameObject.GetComponent<DashScript>();
        if (collision.gameObject.tag == "Stunable" && dashScript.IsOnDash ///Wall Case TODO better to compute stunDuration based on force when colliding.
            || collision.gameObject.tag == "Player" && dashScript.IsOnDash && OtherDashScript.IsOnDash)//Player Case
        {
            StunPlayer();
        }
    }
}
