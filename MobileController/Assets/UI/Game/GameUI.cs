using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class GameUI : FlowStep {

    const string START_GAME = "Win";
    const string PLAYER_UPDATE = "Loose";

    public delegate void DashObserver(bool isLoading);
    public event DashObserver OnDash;
    public delegate void shakeObserver(bool isShaking);
    public event DashObserver OnShake;


    bool ready = false;
    public Network.NetworkClientScript net;
    private GameStateManager gameStateManager;

    Vector3 lastAcceleration;
    bool dashLoading = true;

    float accelerometerUpdateInterval = 1.0f / 60.0f;
    // The greater the value of LowPassKernelWidthInSeconds, the slower the
    // filtered value will converge towards current input sample (and vice versa).
    float lowPassKernelWidthInSeconds = 1.0f;
    // This next parameter is initialized to 2.0 per Apple's recommendation,
    // or at least according to Brady! ;)
    float shakeDetectionThreshold = 2.0f;

    float lowPassFilterFactor;
    Vector3 lowPassValue;
    bool shaking = false;

    void Start()
    {
        gameStateManager = GetComponentInParent<GameStateManager>();
        net.OnMessageReceived += ParseServerMessage;
        net.OnConnect += OnReconnect;
        net.OnDisconnect += OnDisconnect;
        OnDash += DashHandler;
        lowPassFilterFactor = accelerometerUpdateInterval / lowPassKernelWidthInSeconds;
        shakeDetectionThreshold *= shakeDetectionThreshold;
        lowPassValue = Input.acceleration;
    }



    void Update()
    {
       
    }


    public void Show()
    {
        if (!gameObject.activeSelf)
        {
            gameObject.SetActive(true);
            StartCoroutine("SendAccelerationData");
        }
    }

    void Disable()
    {
    }

    void OnReconnect()
    {
        StartCoroutine("SendAccelerationData");
    }

    void OnDisconnect()
    {
        StopCoroutine("SendAccelerationData");
    }

    IEnumerator SendAccelerationData()
    {
        
        while (enabled)
        {
            if (Input.touches.Length > 0)
            {
                dashLoading = true;
                if (OnDash != null)
                    OnDash(dashLoading);
            }
            else if (dashLoading)
            {
                dashLoading = false;
                if(OnDash != null)
                    OnDash(dashLoading);
            }

            byte[] b = BitConverter.GetBytes(dashLoading);
             
            Vector3 acceleration = Input.acceleration;
            lowPassValue = Vector3.Lerp(lowPassValue, acceleration, lowPassFilterFactor);
            //Compute if it was a shaking or a regular movement.
            Vector3 deltaAcceleration = acceleration - lowPassValue;

            if (deltaAcceleration.sqrMagnitude >= shakeDetectionThreshold)
            {
                shaking = true;
                if (OnShake != null)
                    OnShake(true);
            }
            else
            {
                shaking = false;
                if (OnShake != null)
                    OnShake(false);
            }

            byte[] s = BitConverter.GetBytes(shaking);

            string accelString = Network.NetworkClientScript.VectorToString(acceleration);

            if (Input.acceleration == Vector3.zero)
            {
                accelString = Network.NetworkClientScript.VectorToString(new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0f));
            }

            var netErr = net.SendStateUpdate("State;" + Encoding.ASCII.GetString(b) + ";" + accelString + ";" + Encoding.ASCII.GetString(s));
            
            if(netErr == NetworkError.WrongConnection)
            {
                net.Reconnect();
                yield break;
            }
            yield return new WaitForSeconds(0.05f);
        }
    }

    void DashHandler(bool isLoading)
    {
    }
    void ParseServerMessage(string data)
    {
        var command = data.Split(";".ToCharArray());
        if (command[0] == "GameState")
        {
            //TODO Transit to results screen
            if (command[1] == "Win")
            {
            }
            else if(command[1] == "Loose")
            {
            }
            gameStateManager.TransitToLobby();
        }
    }

}
