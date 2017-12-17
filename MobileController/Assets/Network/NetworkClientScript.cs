using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Text;
using System;
using UnityEngine.UI;
using System.Text.RegularExpressions;

namespace Network
{
    public class NetworkClientScript : MonoBehaviour {

        public GlobalConfig GConfig = new GlobalConfig();
        public ConnectionConfig connecConfig = new ConnectionConfig();
        public int port = 8080;
        public int myPort = 8081;

        public delegate void NetObserver();
        public delegate void ErrorNetObserver(NetworkError error);
        public delegate void MessageObserver(string data);

        public event NetObserver OnConnect;
        public event ErrorNetObserver OnConnectTimeOut;

        public event NetObserver OnDisconnect;
        public event MessageObserver OnMessageReceived;

        private int connectionId;
        private int hostId;
        private bool connected = false;
        bool IsConnected() {
            return connected;
        }

        private HostTopology topology;
        private int myReiliableChannelId;

        private int myUnreliableChannelId;
        private int myStateChannelId;

        private ClientNetworkDiscovery networkDiscoveryManager;

        private int remoteHostId;
        private int remoteConnectionId;
        private int remoteChannelId;

        private byte[] receivedBuffer;

        // Use this for initialization
        void Start () {
            Init();
        }

        public void Init()
        {
            receivedBuffer = new byte[1024];

            myReiliableChannelId = connecConfig.AddChannel(QosType.Reliable);
            myUnreliableChannelId = connecConfig.AddChannel(QosType.Unreliable);
            myStateChannelId = connecConfig.AddChannel(QosType.ReliableStateUpdate);

            topology = new HostTopology(connecConfig, 1);

            /// Init
            NetworkTransport.Init(GConfig);
            hostId = NetworkTransport.AddHost(topology, myPort);
        }

        // Update is called once per frame
        public void Update () {
            if (connected)
            {
                int receivedConnectionId;
                int receivedChannelId;
                int receivedDataSize;
                byte error;
                receivedBuffer.Initialize();

                var response = NetworkTransport.ReceiveFromHost(remoteHostId, out receivedConnectionId, out receivedChannelId, receivedBuffer, 1024, out receivedDataSize, out error);
                switch (response)
                {
                    case NetworkEventType.Nothing:         //3
                        break;
                    case NetworkEventType.DisconnectEvent:
                        break;
                    case NetworkEventType.ConnectEvent:
                        break;
                    default:
                        var str = Encoding.ASCII.GetString(receivedBuffer);
                        if (OnMessageReceived != null)
                        {
                            OnMessageReceived(str);
                        }
                        break;
                }
            }
            /* Logic
             * 
             * Server is setup on localhost : 8080
             * 
             * client is detected by local discovery and reaction on a broadcast on 8080 port.
             * client receive response and make a connect request.
             * host receive connect request and approuve
             * then client send every Inputs of the Smartphone.
             * Inputs are : 
             * Pressed / Release Events with a coroutine on pressed that compute force of the pression
             * Release Event, take the coroutines values and apply them.
             * 
             * Continually, client send to host gyroscopic coordinates.
             * 
             */
        }

        public NetworkError Send(string data)
        {
            byte error;
            byte[] buffer = Encoding.ASCII.GetBytes(data);
            Debug.Log(BitConverter.ToString(buffer));
            if(!NetworkTransport.Send(remoteHostId, connectionId, myReiliableChannelId, buffer, buffer.Length, out error))
            {
                Debug.Log("Error : " + (NetworkError)(error));
            }
            
            return (NetworkError)(error);
        }

        public NetworkError SendCommandAndData(byte[] data, int dataSize)
        {
            byte err;
            NetworkTransport.Send(remoteHostId, connectionId, myReiliableChannelId, data, dataSize, out err);
            return (NetworkError)err;
        }

        public void Connect(string ipv4)
        {
            StartCoroutine(ConnectAndWaitResponse(ipv4, port, 60));
        }

        public void Disconnect()
        {
            StopCoroutine("SendAck");
            byte error;
            NetworkTransport.Disconnect(hostId, connectionId, out error);
            if(OnDisconnect != null)
                OnDisconnect();
        }
        IEnumerator ConnectAndWaitResponse(string ip, int port, int retryTime = 5)
        {
            const string CONNECT_COROUTINE = "ConnectAndWaitResponse";
            const string ACK_COROUTINE = "SendAck";

            byte error;

            connectionId = NetworkTransport.Connect(hostId, ip, port, 0, out error);
            NetworkError netErr = (NetworkError)error;
            if (netErr != NetworkError.Ok)
            {
                
                StopCoroutine(CONNECT_COROUTINE);
                yield break;
            }
            
            int buffLen = 1024;
            byte[] buffer = new byte[buffLen];
            int receivedHostId;
            int receivedConnectionId;
            int receivedChannelId;
            int receivedDataSize;
           
            for (int i = 0; i < retryTime; i += 1)
            {
                buffer.Initialize();
                NetworkEventType response = NetworkTransport.Receive(out receivedHostId, out receivedConnectionId, out receivedChannelId, buffer, buffLen, out receivedDataSize, out error);
                Debug.Log(i + " " + retryTime + " " + response);
                switch (response)
                {
                    case NetworkEventType.Nothing:         //3
                        break;
                    case NetworkEventType.ConnectEvent:
                        if (hostId == receivedHostId 
                            && connectionId == receivedConnectionId 
                            && (NetworkError)error == NetworkError.Ok)// response to a connect Event.
                        {
                            connected = true;
                            remoteHostId = receivedHostId;
                            remoteChannelId = receivedChannelId;
                            
                            StopCoroutine(CONNECT_COROUTINE);
                            StartCoroutine(ACK_COROUTINE);
                            OnConnect();
                            yield break;
                        }
                        else
                        {
                            Debug.Log("Connect received but don't match");
                        }
                        break;
                    default:
                        break;
                }
                yield return new WaitForSeconds(0.1f);
            }
            if (!connected)
            {
                if(OnConnectTimeOut != null)
                {
                    OnConnectTimeOut((NetworkError)error);
                }
            }
        }

        IEnumerator SendAck()
        {
            byte error;
            while (connected)
            {
                if (Send("Alive;") == NetworkError.WrongConnection)
                {
                    connected = false;
                    break;
                }
                else
                {
                    Debug.Log("Alive Sended");
                }
                yield return new WaitForSeconds(0.5f);
            }
            Disconnect();
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
            System.Buffer.BlockCopy(y, 0, final, w.Length+x.Length, y.Length);
            System.Buffer.BlockCopy(z, 0, final, w.Length+x.Length+y.Length, z.Length);

            return final;
        }

        public static string VectorToString(Vector3 v)
        {
            string str = v.x + ";" + v.y + ";" + v.z;
            return str;
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
          
            //Int32 size == 4 bytes;
            Color c = Color.black;
            c.r = BitConverter.ToInt32(data,0) / 255.0f;
            c.g = BitConverter.ToInt32(data, 4) / 255.0f;
            c.b = BitConverter.ToInt32(data, 8) / 255.0f;
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
}
