using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class PlayerNetHandler {


    public delegate void DashObserver(bool isDashing);
    public delegate void MovementObserver(Vector3 movement);
    public event DashObserver DashStarted;
    public event DashObserver DashStopped;
    public event MovementObserver OnMoving;

    //State of dash;
    private PlayerConnexionScript connexionScript;
    private bool isDashing;

    // Use this for initialization
    public PlayerNetHandler(PlayerConnexionScript connexionScript)
    {
        connexionScript.OnMessageReceived += ParseMessage;
    }
	
	// Update is called once per frame
	void ParseMessage(NetworkScript.ConnectionData connection, string data)
    {
        var command = data.Split(";".ToCharArray());
        if (command[0] == "State")
        {
            byte[] buff = Encoding.ASCII.GetBytes(command[1]);

            bool dashing = BitConverter.ToBoolean(buff, 0);

            Debug.Log("Dashing : " + dashing);
            Vector3 acceleration = new Vector3(float.Parse(command[2]), float.Parse(command[3]), float.Parse(command[4]));
            Debug.Log("Acceleration" + acceleration);

            if (OnMoving != null)
                OnMoving(acceleration);
            if(dashing != isDashing)
            {
                isDashing = dashing;
                if (isDashing && DashStarted != null)
                {
                    DashStarted(isDashing);//true
                }
                else if(DashStopped != null)
                {
                    DashStopped(isDashing);//false
                }
            }
        }
    }
}
