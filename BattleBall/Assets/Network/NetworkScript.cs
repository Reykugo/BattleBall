﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.Types;
using System.Text;
using System;


public class NetworkScript : MonoBehaviour { 

    public GlobalConfig GConfig = new GlobalConfig();
    public ConnectionConfig ConnectConfig = new ConnectionConfig();

    public delegate void NetObserver(int connectionId);
    public delegate void ErrorNetObserver(NetworkError error);
    public delegate void MessageObserver(int connectionId, string data);

    public event NetObserver OnConnect;
    public event ErrorNetObserver OnConnectTimeOut;

    public event NetObserver OnDisconnect;
    public event MessageObserver OnMessageReceived;

    public GameObject playerNetworkPrefab;
    int hostId;

    //TODO identify with local IP address, more reliable than abstract connectionID;
    private Dictionary<int, GameObject> clients = new Dictionary<int, GameObject>();

    private HostTopology topology;
    private int myUnreliableChannelId;
    private int myReiliableChannelId;

    // Use this for initialization
    void Start () { 
        myReiliableChannelId = ConnectConfig.AddChannel(QosType.Reliable);
        myUnreliableChannelId = ConnectConfig.AddChannel(QosType.Unreliable);
        NetworkTransport.Init(GConfig);
        topology = new HostTopology(ConnectConfig, 4);
        int hostId = NetworkTransport.AddHost(topology, 8080);
    }

    void Update()
    {
        int remoteHostId;
        int hConnectionId;
        int channelId;
        byte[] recBuffer = new byte[1024];
        int bufferSize = 1024;
        int dataSize;
        byte error;
        NetworkEventType recData = NetworkTransport.Receive(out remoteHostId, out hConnectionId, out channelId, recBuffer, bufferSize, out dataSize, out error);
        Debug.Log(recData);
        switch (recData)
        {
            case NetworkEventType.Nothing:         //1
                break;
            case NetworkEventType.ConnectEvent:
                OnConnectReceived(hConnectionId, remoteHostId, channelId);
                break;
            case NetworkEventType.DataEvent:
                var data = Encoding.ASCII.GetString(recBuffer);
                //OnDataReceived(hConnectionId, remoteHostId, myReiliableChannelId, data);//3
                break;
            case NetworkEventType.DisconnectEvent: //4
                
                //OnClientDisconnected(receivedConnectionId, data);
                break;
        }
    }

    public void Disconnect(int connectionId)
    {
        Debug.Log(clients.Count);
        Destroy(clients[connectionId]);
        clients.Remove(connectionId);
        if(OnDisconnect != null)
        {
            OnDisconnect(connectionId);
        }
        Debug.Log(clients.Count);
    }

    void OnConnectReceived(int receivedConnectionId, int receivedHostId, int receivedChannelId)
    {//If party has not started.
        PlayerConnexionScript.PlayerData playerData = new PlayerConnexionScript.PlayerData();
        Color[] colors = { Color.red, Color.blue, Color.green, Color.yellow };
        playerData.connexionId = receivedConnectionId;
        playerData.hostId = receivedHostId;
        NetworkID networkId;
        NodeID dstNode;
        byte error;
        Debug.Log("Connect Player");
        NetworkTransport.GetConnectionInfo(receivedHostId, receivedConnectionId, out playerData.ipAddress, out playerData.port, out networkId, out dstNode, out error);
        playerData.playerName = "J" + receivedConnectionId;
        playerData.color = colors[receivedConnectionId-1];

        var go = Instantiate(playerNetworkPrefab, transform);
        clients.Add(receivedConnectionId, go);
        clients[receivedConnectionId].name = "player " + receivedConnectionId + " @"+playerData.ipAddress;
        var s = clients[receivedConnectionId].GetComponent<PlayerConnexionScript>();
        s.playerData = playerData;
        s.net = this;

        byte[] buffer = Encoding.ASCII.GetBytes("PlayerUpdate;");
        byte[] color = ColorToByte(playerData.color);
        byte[] final = new byte[buffer.Length + color.Length];
        System.Buffer.BlockCopy(buffer, 0, final, 0, buffer.Length);
        System.Buffer.BlockCopy(color, 0, final, buffer.Length, color.Length);
        
        if (!NetworkTransport.Send(receivedHostId, receivedConnectionId, receivedChannelId, final, final.Length, out error))
        {
            Debug.Log("Error");
        }
        else
        {
            if(OnConnect != null)
                OnConnect(receivedConnectionId);
            StartCoroutine(ListenToClient(receivedConnectionId, receivedHostId, playerData));
        }
    }

    void OnDisconectReceived(int receivedConnectionId, int receivedHostId, int receivedChannelId)
    {
        Debug.Log("Disconnect event");
        Debug.Log(receivedConnectionId + " : " + receivedHostId);
    }

    IEnumerator ListenToClient(int receivedConnectionId, int receivedHostId, PlayerConnexionScript.PlayerData client)
    {
        bool connected = true;
        while (connected)
        {
            int hConnectionId;
            int channelId;
            byte[] recBuffer = new byte[1024];
            int bufferSize = 1024;
            int dataSize;
            byte error;
            NetworkEventType recData = NetworkTransport.ReceiveFromHost(receivedHostId, out hConnectionId, out channelId, recBuffer, bufferSize, out dataSize, out error);
            switch (recData)
            {
                case NetworkEventType.Nothing:         //1
                    break;
                case NetworkEventType.DisconnectEvent: //4
                    connected = false;
                    //OnClientDisconnected(receivedConnectionId, data);
                    break;
            }
            yield return null;
        }
    }

    // Coul'd be null Check the return.
    public GameObject GetPlayer(int connexionId)
    {
        return clients.ContainsKey(connexionId) ? clients[connexionId] : null;
    }

    public static Quaternion BytesToQuaternion(byte[] bytes, int startIndex)
    {
        float w = BitConverter.ToSingle(bytes, startIndex);
        float x = BitConverter.ToSingle(bytes, startIndex+4);
        float y = BitConverter.ToSingle(bytes, startIndex+8);
        float z = BitConverter.ToSingle(bytes, startIndex+12);

        return new Quaternion(x, y, z, w);
    }

    public static Vector3 BytesToVector(byte[] bytes, int startIndex)
    {
        float x = BitConverter.ToSingle(bytes, startIndex);
        float y = BitConverter.ToSingle(bytes, startIndex + 4);
        float z = BitConverter.ToSingle(bytes, startIndex + 8);

        return new Vector3(x, y, z);
    }

    public static byte[] QuaternionToBytes(Quaternion q)
    {
        Debug.Log(q);
        byte[] w = BitConverter.GetBytes(q.w);
        byte[] x = BitConverter.GetBytes(q.x);
        byte[] y = BitConverter.GetBytes(q.y);
        byte[] z = BitConverter.GetBytes(q.z);
        byte[] final = new byte[w.Length + x.Length + y.Length + z.Length];
        System.Buffer.BlockCopy(w, 0, final, 0, w.Length);
        System.Buffer.BlockCopy(x, 0, final, w.Length, x.Length);
        System.Buffer.BlockCopy(y, 0, final, w.Length + x.Length, y.Length);
        System.Buffer.BlockCopy(z, 0, final, w.Length + x.Length + y.Length, z.Length);

        return final;
    }

    public static byte[] VectorToBytes(Vector3 v)
    {
        byte[] x = BitConverter.GetBytes(v.x);
        byte[] y = BitConverter.GetBytes(v.y);
        byte[] z = BitConverter.GetBytes(v.z);
        byte[] final = new byte[x.Length + y.Length + z.Length];

        System.Buffer.BlockCopy(x, 0, final, 0, x.Length);
        System.Buffer.BlockCopy(y, 0, final, x.Length, y.Length);
        System.Buffer.BlockCopy(z, 0, final, x.Length + y.Length, z.Length);

        return final;
    }

    public static byte[] VectorToBytes(Vector2 v)
    {
        byte[] x = BitConverter.GetBytes(v.x);
        byte[] y = BitConverter.GetBytes(v.y);
        byte[] final = new byte[x.Length + y.Length];

        System.Buffer.BlockCopy(x, 0, final, 0, x.Length);
        System.Buffer.BlockCopy(y, 0, final, x.Length, y.Length);

        return final;
    }

    public static Color ByteToColor(byte[] data)
    {
        Color c = Color.black;
        c.r = data[0] / 255.0f;
        c.g = data[1] / 255.0f;
        c.b = data[2] / 255.0f;
        c.a = 1f;

        return c;
    }

    public static byte[] ColorToByte(Color c)
    {
        byte[] rbuf = BitConverter.GetBytes(c.r *255.0f);
        byte[] gbuf = BitConverter.GetBytes(c.g * 255.0f);
        byte[] bbuf = BitConverter.GetBytes(c.b * 255.0f);
        byte[] final = new byte[rbuf.Length + gbuf.Length + bbuf.Length];
        System.Buffer.BlockCopy(rbuf, 0, final, 0, rbuf.Length);
        System.Buffer.BlockCopy(gbuf, 0, final, rbuf.Length, gbuf.Length);
        System.Buffer.BlockCopy(bbuf, 0, final, rbuf.Length + bbuf.Length, bbuf.Length);
        return final;
    }

}
