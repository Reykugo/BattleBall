using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerConnexionScript : MonoBehaviour {
    const string ALIVE_COMMAND = "Alive";
    const string PLAYER_UPDATE = "PlayerUpdate";
    //PlayerData represent the player states, score, a lot of things. 
    public struct ConnectionData//TODO make a class REFACTOR;
    {
        public int connexionId;
        public int hostId;
        public string ipAddress;
        public int port;
    }

    public delegate void NetMessageObserver(string buffer);
    public NetMessageObserver OnMessageReceived;
    public ConnectionData clientData;
    public NetworkScript net;

    private byte[] receivedBuff = new byte[1024];
    private int channelId;
    private int connectionId;
    private float disconnectTimer = 0f;
    private bool connected = true;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        byte error;
        receivedBuff.Initialize();
        int receivedSize;
        NetworkEventType ev= NetworkTransport.ReceiveFromHost(clientData.hostId, out connectionId, out channelId, receivedBuff, 1024, out receivedSize, out error);
        switch (ev)
        {
            case NetworkEventType.DataEvent:
                string data = Encoding.ASCII.GetString(receivedBuff);
                OnDataReceived(data);
                break;
            case NetworkEventType.ConnectEvent:
                //OnReconnect;
                break;
            case NetworkEventType.DisconnectEvent:
                //OnDisconnectUnwanted;
                break;
            case NetworkEventType.Nothing:
                break;
        }
        if (connected)
        {
            disconnectTimer += Time.deltaTime;
            if(disconnectTimer > 5.0f)// 5.0 seconds.
            {
                Debug.Log("Disconnect no alive in " + disconnectTimer);
                NetworkTransport.Disconnect(clientData.hostId, clientData.connexionId, out error);
                //OnDisconnect;
                //
                connected = false;
                net.Disconnect(clientData.connexionId);
            }
        }

    }

    private void OnDataReceived(string data)
    {
        var command = data.Split(";".ToCharArray());
        if (command[0] == ALIVE_COMMAND)
        {
            Debug.Log("Alive : " + disconnectTimer);
            disconnectTimer = 0f;
            return;
        }

        if (OnMessageReceived != null)
            OnMessageReceived(data);
    }

    public void SendColorUpdate(Color color)
    {
        string colorCommand = PLAYER_UPDATE + ";" + Encoding.ASCII.GetString(NetworkScript.ColorToByte(color));
        Debug.Log(colorCommand);
        Send(colorCommand);
    }
    private void Send(string message)
    {
        byte error;
        byte[] buffer = Encoding.ASCII.GetBytes(message);
        NetworkTransport.Send(clientData.hostId, clientData.connexionId, channelId, buffer, buffer.Length, out error);
        //Check error;
    }
}
