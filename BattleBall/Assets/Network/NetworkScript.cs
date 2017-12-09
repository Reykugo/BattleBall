using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Text;
using System;


public class NetworkScript : MonoBehaviour { 

    public GlobalConfig GConfig = new GlobalConfig();
    public ConnectionConfig ConnectConfig = new ConnectionConfig();

    int hostId;


    private List<HostData> clients = new List<HostData>();

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
        int recHostId;
        int hConnectionId;
        int channelId;
        byte[] recBuffer = new byte[1024];
        int bufferSize = 1024;
        int dataSize;
        byte error;
        NetworkEventType recData = NetworkTransport.Receive(out recHostId, out hConnectionId, out channelId, recBuffer, bufferSize, out dataSize, out error);
        switch (recData)
        {
            case NetworkEventType.Nothing:         //1
                break;
            case NetworkEventType.ConnectEvent:    //2
                OnConnectReceived(hConnectionId, recHostId, channelId);
                break;
            case NetworkEventType.DataEvent:       //3
                Vector3 acceleration = BytesToVector(recBuffer, 0);
                Debug.Log("ReceivedDataEvent " + acceleration);
                break;
            case NetworkEventType.DisconnectEvent: //4
                OnDisconectReceived(hConnectionId, recHostId, channelId);
                break;
        }
    }


    void OnDataReceived(int receivedConnectionId, int receivedHostId, int receivedChannelId)
    {
        //Check game states.
    }

    void OnConnectReceived(int receivedConnectionId, int receivedHostId, int receivedChannelId)
    {//If party has not started.
        Debug.Log("Connect Received");
        byte error;
        byte[] buffer = Encoding.ASCII.GetBytes("test");
        var bufferLength = 5;
        NetworkTransport.Send(receivedHostId, receivedConnectionId, receivedChannelId, buffer, bufferLength, out error);
    }

    void OnDisconectReceived(int receivedConnectionId, int receivedHostId, int receivedChannelId)
    {
        Debug.Log("Disconnect event");
        Debug.Log(receivedConnectionId + " : " + receivedHostId);
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


}
