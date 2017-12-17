using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerConnexionScript : MonoBehaviour {
    const string ALIVE_COMMAND = "Alive";
    const string PLAYER_UPDATE = "PlayerUpdate";
    //PlayerData represent the player states, score, a lot of things. 

    public delegate void NetMessageObserver(NetworkScript.ConnectionData connectionData,string buffer);
    public NetMessageObserver OnMessageReceived;
    public NetworkScript.ConnectionData clientData;
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
        
        if (connected)
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
        }

    }

    public void ParseMessage(string data)
    {
        OnDataReceived(data);
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
            OnMessageReceived(clientData, data);
    }

    public void SendColorUpdate(Color color)
    {
        string colorCommand = PLAYER_UPDATE + ";" + Encoding.ASCII.GetString(NetworkScript.ColorToByte(color));
        Send(colorCommand);
    }

    public void SendStartGame()
    {
        Send("StartGame;");
    }
    private void Send(string message)
    {
        byte error;
        byte[] buffer = Encoding.ASCII.GetBytes(message);
        NetworkTransport.Send(clientData.hostId, clientData.connexionId, channelId, buffer, buffer.Length, out error);
        //Check error;
    }
}
