﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerConnexionScript : MonoBehaviour {

    [Serializable]
    public class PowerReference
    {
        public Power.PowerType power;
        public GameObject prefab;
    }

    public PowerReference[] ListOfPower;
    //
    private Dictionary<Power.PowerType, GameObject> powerByType = new Dictionary<Power.PowerType, GameObject>();





    const string ALIVE_COMMAND = "Alive";
    const string PLAYER_UPDATE = "PlayerUpdate";
    //PlayerData represent the player states, score, a lot of things. 

    public delegate void NetMessageObserver(NetworkScript.ConnectionData connectionData,string buffer);
    public NetMessageObserver OnMessageReceived;
    public NetworkScript.ConnectionData clientData;
    public NetworkScript net;

    private byte[] receivedBuff = new byte[1024];
    private int channelId;
    private float disconnectTimer = 0f;
    private bool connected = true;
   
	// Use this for initialization
	void Start () {
        
    }
	
	// Update is called once per frame
	void Update () {

       /* if (connected)
        {
            disconnectTimer += Time.deltaTime;
            if(disconnectTimer > 5.0f)// 5.0 seconds.
            {
                byte error;
                Debug.Log("Disconnect no alive in " + disconnectTimer);
                NetworkTransport.Disconnect(clientData.hostId, clientData.connexionId, out error);
                //OnDisconnect;
                //
                connected = false;
                net.Disconnect(clientData.connexionId);
            }
        }*/

    }
    public void ParseMessage(string data)
    {
        OnDataReceived(data);
    }

    private void OnDataReceived(string data)
    {
        disconnectTimer = 0f;
        if (OnMessageReceived != null)
            OnMessageReceived(clientData, data);
    }

    public void SendColorUpdate(Color color)
    {
        string colorCommand = PLAYER_UPDATE + ";" + Encoding.ASCII.GetString(NetworkScript.ColorToByte(color));
        Send(colorCommand);
    }

    public void SendStartGame(int lifes)
    {
        Send("StartGame;" + Encoding.ASCII.GetString(BitConverter.GetBytes(lifes)) + ";");
    }

    public void SendEndGame(bool winner)
    {
        var winState = (winner) ? "Win" : "Loose";
        Debug.Log("player " + clientData.ipAddress + winState);
        Send("EndGame;" + winState + ";");
    }

    public void SendGoToLobby()
    {
        Send("GoToLobby;");
    }

    public void SendAvatarDead()
    {
        Send("AvatarDied;");
    }

    public void SendAvatarFall()
    {
        Send("AvatarFelt;");
    }

    public void SendNewPower(Power.PowerType powerType)
    {
        Send("NewPower;" + Encoding.ASCII.GetString(BitConverter.GetBytes((int)powerType)) + ";");
    }

    private void Send(string message)
    {
        byte error;
        byte[] buffer = Encoding.ASCII.GetBytes(message);
        NetworkTransport.Send(clientData.hostId, clientData.connexionId, channelId, buffer, buffer.Length, out error);
        //Check error;
    }

    IEnumerator NetGameHandler()
    {
        while (true)
        {
            int channelId;
            byte error;
            int receivedSize;
            Debug.Log(clientData.connexionId);

            NetworkEventType eventType = NetworkTransport.ReceiveFromHost(clientData.hostId, out clientData.connexionId, out channelId, receivedBuff, receivedBuff.Length, out receivedSize, out error);
            switch (eventType)
            {
                case NetworkEventType.ConnectEvent:
                    break;
                case NetworkEventType.DisconnectEvent:
                    break;
                case NetworkEventType.DataEvent:
                    string data = Encoding.ASCII.GetString(receivedBuff);
                    Debug.Log(data);
                    OnDataReceived(data);
                    break;
            }
            yield return new WaitForSeconds(Time.deltaTime);
        }
    }
}
