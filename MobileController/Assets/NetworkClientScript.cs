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

        public Text text;

        private int connectionId;
        private int hostId;
        private bool connected = false;
        private HostTopology topology;
        private int myReiliableChannelId;

        private int myUnreliableChannelId;

        private ClientNetworkDiscovery networkDiscoveryManager;
        private bool connectionAsked = false;

        private int remoteHostId;
        private int remoteConnectionId;
        private int remoteChannelId;

        // Use this for initialization
        void Start () {
            //Set Up
            networkDiscoveryManager = GetComponent<ClientNetworkDiscovery>();
            myReiliableChannelId = connecConfig.AddChannel(QosType.Reliable);
            myUnreliableChannelId = connecConfig.AddChannel(QosType.Unreliable);

            topology = new HostTopology(connecConfig, 2);

            /// Init
            NetworkTransport.Init(GConfig);
            hostId = NetworkTransport.AddHost(topology, 8081);

            networkDiscoveryManager.OnHostDiscovered += OnReceivedBroadcast;
        }

        // Update is called once per frame
        public void Update () {
            if (connected)
            { 
                //byte[] buffer = QuaternionToBytes(Input.gyro.attitude);
                //byte[] buffer = QuaternionToBytes(transform.rotation);
                byte[] buffer = VectorToBytes(Input.acceleration);
                //byte[] buffer = VectorToBytes(new Vector3());
                byte error;
                if (!NetworkTransport.Send(remoteHostId, connectionId, remoteChannelId, buffer, buffer.Length, out error))
                {
                    text.text = "Failed to send";
                }
                else
                {
                    text.text = "Send " + Input.acceleration + " to " + remoteHostId;
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

        private void OnReceivedBroadcast(string fromAddress, string data)
        {
            //Make a connection to the server.
            String pattern = @"::.*:(.*)";
            string ipV4 = Regex.Matches(fromAddress, pattern)[0].Groups[1].Value;
            text.text = "Broadcast Received on " + ipV4 + "\n";
            networkDiscoveryManager.StopBroadcast();
            StartCoroutine(ConnectAndWaitResponse(ipV4, 8080, 60));
        }

        void OnConnectTimeout()
        {
            text.text = "Connect Timeout";
            //Code to handle timeout.
        }

        IEnumerator ConnectAndWaitResponse(string ip, int port, int retryTime = 5)
        {
            const string CONNECT_COROUTINE = "ConnectAndWaitResponse";

            byte error;
            connectionId = NetworkTransport.Connect(hostId, ip, port, 0, out error);
            NetworkError netErr = (NetworkError)error;
            if (netErr != NetworkError.Ok)
            {
                Debug.Log("Failed to connect " + netErr);
                text.text += "Failed to connect to " + ip;
                Debug.Log(hostId);
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
                Debug.Log(response);
                switch (response)
                {
                    case NetworkEventType.Nothing:         //1
                        text.text = "Waiting ";
                        int t = i % 3;
                        for(int j =0; j < t; j++)
                        {
                            text.text += ".";
                        }
                        break;
                    case NetworkEventType.ConnectEvent:
                        if (hostId == receivedHostId 
                            && connectionId == receivedConnectionId 
                            && (NetworkError)error == NetworkError.Ok)// response to a connect Event.
                        {
                            text.text = "Connected";
                            connected = true;
                            remoteHostId = receivedHostId;
                            remoteChannelId = receivedChannelId;
                            
                            StopCoroutine(CONNECT_COROUTINE);
                            yield break;
                        }
                        break;
                    default:
                        text.text = "Received Something Else";
                        break;
                }
                yield return new WaitForSeconds(0.1f);
            }
            if (!connected)
            {
                networkDiscoveryManager.StartAsClient();
                OnConnectTimeout();
            }
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
    }
}
