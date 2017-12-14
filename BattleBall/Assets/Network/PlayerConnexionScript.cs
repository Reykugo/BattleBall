using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerConnexionScript : MonoBehaviour {
    const string ALIVE_COMMAND = "Alive";
    //PlayerData represent the player states, score, a lot of things. 
    public struct PlayerData//TODO make a class REFACTOR;
    {
        public int connexionId;
        public int hostId;
        public string ipAddress;
        public int port;

        public string playerName;
        public Color color;

        public bool ready;
    }

    public delegate void NetMessageObserver(string buffer);
    public NetMessageObserver OnMessageReceived;
    public PlayerData playerData;
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
        NetworkEventType ev= NetworkTransport.ReceiveFromHost(playerData.hostId, out connectionId, out channelId, receivedBuff, 1024, out receivedSize, out error);
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
                NetworkTransport.Disconnect(playerData.hostId, playerData.connexionId, out error);
                //OnDisconnect;
                //
                connected = false;
                net.Disconnect(playerData.connexionId);
            }
        }

    }

    private void OnDataReceived(string data)
    {
        Debug.Log(data);
        var command = data.Split(";".ToCharArray());
        Debug.Log(command[0]);
        Debug.Log(command[0].Length + " / " + ALIVE_COMMAND.Length);
        if (command[0] == ALIVE_COMMAND)
        {
            Debug.Log("Alive : " + disconnectTimer);
            disconnectTimer = 0f;
            return;
        }else
        {
            Debug.LogWarning("Not Alive");
        }

        if (OnMessageReceived != null)
            OnMessageReceived(data);
    }
}
