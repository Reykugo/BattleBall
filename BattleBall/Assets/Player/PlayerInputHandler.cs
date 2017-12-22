using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class PlayerInputHandler : MonoBehaviour {

    public delegate void TouchObserver();
    public delegate void StateObserver();
    public delegate void MovementObserver(Vector3 movement);
    public event StateObserver OnTouchStarted;//Called uppon touching the screen.
    public event StateObserver OnTouch;//Called when touching the screen
    public event StateObserver OnTouchStopped;//Called when touch end.
    public event StateObserver OnShaking;//Called when shaking the phone.
    public event MovementObserver OnMoving;//Called when moving the phone.

    private bool isTouching;
    private PlayerConnexionScript _PlayerConnexion;

    //Use for testing purposes.
    public KeyCode touchKeyCode;
    public KeyCode shakeKeyCode;


	// Use this for initialization
	void Start () {
		
	}
    public void InitNet(PlayerConnexionScript playerConnexion)
    {
        _PlayerConnexion = playerConnexion;
        _PlayerConnexion.OnMessageReceived += HandleNetInputs;
    }
	
	// Update is called once per frame
	void Update () {

        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        if(OnMoving != null && (h !=0  || v != 0))
            OnMoving(new Vector3(h,v, 0f));

        if (OnTouch != null && isTouching && Input.GetKey(touchKeyCode))
        {
            OnTouch();
        }
        if (OnTouchStarted != null && Input.GetKeyDown(touchKeyCode))
        {
            isTouching = true;
            OnTouchStarted();
        }
        if (OnTouchStopped != null && Input.GetKeyUp(touchKeyCode))
        {
            isTouching = false;
            OnTouchStopped();
        }

        if (OnShaking != null && Input.GetKey(shakeKeyCode))
        {
            OnShaking();
        }
    }

    private void HandleNetInputs(NetworkScript.ConnectionData connectionData, string data)
    {
        var command = data.Split(";".ToCharArray());
        if (command[0] == "State")
        {
            bool touching = BitConverter.ToBoolean(Encoding.ASCII.GetBytes(command[1]), 0);
            bool shaking = BitConverter.ToBoolean(Encoding.ASCII.GetBytes(command[5]), 0);

            if (!shaking)//Don't take in account movement when shaking.
            {
                float x = 0;
                float y = 0;
                float z = 0;
                float.TryParse(command[2], out x);
                float.TryParse(command[3], out y);
                float.TryParse(command[4], out z);

                Vector3 acceleration = new Vector3(x, y, z);
                if (OnMoving != null)
                    OnMoving(acceleration);
            }
            else
            {
                if (OnShaking != null)
                    OnShaking();
            }

            if (touching != isTouching)//Touching Changed.
            {
                isTouching = touching;
                if (isTouching && OnTouchStarted != null)
                {
                    OnTouchStarted();//true
                }
                else if (OnTouchStopped != null)
                {
                    OnTouchStopped();//
                }
            }
            else if (touching && OnTouch != null)
            {
                OnTouch();
            }
        }
    }
}
