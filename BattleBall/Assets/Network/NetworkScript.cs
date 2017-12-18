using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.Types;
using System.Text;
using System;


public class NetworkScript : MonoBehaviour {

    public struct ConnectionData//TODO make a class REFACTOR;
    {
        public int connexionId;
        public int hostId;
        public string ipAddress;
        public int port;
    }

    public GlobalConfig GConfig = new GlobalConfig();
    public ConnectionConfig ConnectConfig = new ConnectionConfig();

    public delegate void NetObserver(ConnectionData connectionData);
    public delegate void MessageObserver(ConnectionData data, string buffer);
    public delegate void ErrorNetObserver(NetworkError error);

    public event NetObserver OnConnect;
    public event ErrorNetObserver OnConnectTimeOut;

    public event NetObserver OnDisconnect;

    public event MessageObserver OnMessage;

    int hostId;

    public bool connectionAuthorized = true;
    //TODO identify with local IP address, more reliable than abstract connectionID;
    private Dictionary<int, ConnectionData> clients = new Dictionary<int, ConnectionData>();

    private HostTopology topology;
    private int myUnreliableChannelId;
    private int myReiliableChannelId;
    private int myStateChannelId;

    void OnDestroy()
    {
        NetworkTransport.Shutdown();
    }
    // Use this for initialization
    void Start () { 
        myReiliableChannelId = ConnectConfig.AddChannel(QosType.Reliable);
        myUnreliableChannelId = ConnectConfig.AddChannel(QosType.Unreliable);
        myStateChannelId = ConnectConfig.AddChannel(QosType.ReliableStateUpdate);
        NetworkTransport.Init(GConfig);
        topology = new HostTopology(ConnectConfig, 5);
        hostId = NetworkTransport.AddHost(topology, 8080);
    }


    private byte[] buffer = new byte[1025];
    void Update()
    {
        
            buffer.Initialize();
            int remoteHostId;
            int hConnectionId;
            int channelId;
            int dataSize;
            byte error;
            NetworkEventType recData = NetworkTransport.Receive(out remoteHostId, out hConnectionId, out channelId, buffer, buffer.Length, out dataSize, out error);
            Debug.Log(recData);
            switch (recData)
            {
                case NetworkEventType.Nothing:         //1
                    break;
                case NetworkEventType.ConnectEvent:
                    OnConnectReceived(hConnectionId, remoteHostId, channelId);
                    break;
                case NetworkEventType.DisconnectEvent:
                    break;
                case NetworkEventType.DataEvent:
                    if (OnMessage != null)
                    {
                        var data = Encoding.ASCII.GetString(buffer);
                    
                        OnMessage(clients[hConnectionId], data);
                    }
                    break;
            }
    }

    public void Disconnect(int connectionId)
    {
        if(OnDisconnect != null)
        {
            OnDisconnect(clients[connectionId]);
        }
        clients.Remove(connectionId);
    }

    void OnConnectReceived(int receivedConnectionId, int receivedHostId, int receivedChannelId)
    {
        if (!clients.ContainsKey(receivedConnectionId))
        {
            ConnectionData clientData = new ConnectionData();
            clientData.connexionId = receivedConnectionId;
            clientData.hostId = receivedHostId;
            Debug.Log(clientData.hostId);

            NetworkID networkId;
            NodeID dstNode;
            byte error;
            NetworkTransport.GetConnectionInfo(receivedHostId, receivedConnectionId, out clientData.ipAddress, out clientData.port, out networkId, out dstNode, out error);
            clients.Add(receivedConnectionId, clientData);
            if (OnConnect != null)
                OnConnect(clientData);
        }
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
        Debug.Log(BitConverter.ToString(bytes));


        float x = BitConverter.ToSingle(bytes, startIndex);
        Debug.Log(x + " : " +  BitConverter.ToString(bytes, startIndex, 4));

        float y = BitConverter.ToSingle(bytes, startIndex + 4);
        Debug.Log(y + " : " + BitConverter.ToString(bytes, startIndex + 4, 4));

        float z = BitConverter.ToSingle(bytes, startIndex + 8);
        Debug.Log(z + " : " + BitConverter.ToString(bytes, startIndex + 8, 4));

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
